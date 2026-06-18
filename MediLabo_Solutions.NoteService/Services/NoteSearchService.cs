using MediLabo_Solutions.Shared.Models;
using OpenSearch.Client;
using OpenSearch.Net;

namespace MediLabo_Solutions.NoteService.Services
{
    public class NoteSearchService(
        IOpenSearchClient openSearchClient,
        INoteAppService noteService,
        IConfiguration configuration) : INoteSearchService
    {
        private string indexName = configuration["OpenSearchSettings:IndexName"] ?? "notes";
        public async Task CreateIndexAsync()
        {
            var indexExists = await openSearchClient.Indices.ExistsAsync(indexName);
            if (indexExists.Exists)
                await openSearchClient.Indices.DeleteAsync(indexName);

            var createIndexResponse = await openSearchClient.Indices.CreateAsync(indexName, c => c
                .Settings(s => s
                    .NumberOfShards(1)
                    .NumberOfReplicas(0)
                    .Analysis(a => a
                        .TokenFilters(tf => tf
                           .Stemmer("french_stemmer", st => st
                                .Language("french")
                           )
                           .Elision("french_elision", e => e
                                .Articles("l", "m", "t", "qu", "n", "s", "j", "d", "c", "jusqu", "quoiqu", "lorsqu", "puisqu")
                           )
                           .Stop("french_stop", st => st
                                .StopWords("_french_")
                           )
                        )
                        .Analyzers(an => an
                            .Custom("french_medical_analyzer", ca => ca
                                .Tokenizer("standard")
                                .Filters(
                                    "lowercase",
                                    "french_elision",
                                    "french_stop",
                                    "french_stemmer",
                                    "asciifolding"
                                )
                            )
                            .Custom("keyword_lowercase_analyzer", ca => ca
                                .Tokenizer("keyword")
                                .Filters("lowercase", "asciifolding")
                            )
                        )
                    )
                )
                .Map<NoteDto>(m => m
                    .Properties(p => p
                        .Keyword(k => k
                            .Name(n => n.Id)
                        )
                        .Number(n => n
                            .Name(note => note.PatientId)
                            .Type(NumberType.Integer)
                        )
                        .Text(t => t
                            .Name(n => n.Contenu)
                            .Analyzer("french_medical_analyzer")
                            .Fields(f => f
                                .Keyword(k => k
                                    .Name("exact")
                                    .Normalizer("keyword_lowercase_analyzer")
                                )
                                .Text(txt => txt
                                    .Name("stemmed")
                                    .Analyzer("french_medical_analyzer")
                                )
                            )
                        )
                    )
                )
            );

            if (!createIndexResponse.IsValid)
            {
                throw new Exception($"Erreur de création de l'index: {createIndexResponse.ServerError?.Error?.Reason}");
            }
        }

        public async Task<HashSet<string>> SearchTriggerTermsAsync(int patientId, IEnumerable<string> triggerTerms)
        {
            var identifiedTriggers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var triggerTermsList = triggerTerms.ToList();

            foreach (var term in triggerTermsList)
            {
                var searchResponse = openSearchClient.Search<NoteDto>(s => s
                    .Index(indexName)
                    .Query(q => q
                        .Bool(b => b
                            .Must(m => m
                                .Term(t => t
                                    .Field(f => f.PatientId)
                                    .Value(patientId)
                                )
                            )
                            .Should(
                                sh => sh.Match(ma => ma
                                        .Field(f => f.Contenu)
                                        .Query(term)
                                        .Operator(Operator.And)
                                        .Boost(3.0)
                                ),
                                sh => sh.Match(ma => ma
                                    .Field("contenu.stemmed")
                                    .Query(term)
                                    .Boost(2.0)
                                ),
                                sh => sh.Fuzzy(fz => fz
                                    .Field(f => f.Contenu)
                                    .Value(term)
                                    .Fuzziness(Fuzziness.Auto)
                                    .PrefixLength(2)
                                    .MaxExpansions(50)
                                    .Boost(1.5)
                                ),
                                sh => sh.Wildcard(w => w
                                    .Field("contenu.exact")
                                    .Value($"*{term.ToLower()}*")
                                    .CaseInsensitive(true)
                                    .Boost(1.0)
                                )
                            )
                            .MinimumShouldMatch(1)
                        )
                    )
                    .Size(1000)
                    .TrackScores(true)
                );

                if (searchResponse.IsValid && searchResponse.Documents.Any())
                {
                    identifiedTriggers.Add(term);
                }
            }

            return identifiedTriggers;
        }

        public async Task IndexAllNotesAsync()
        {
            var allPatientsIds = await noteService.GetAllPatientIdsAsync();
            var allNotes = new List<NoteDto>();

            foreach (var patientId in allPatientsIds)
            {
                var notes = await noteService.GetNotesByPatientIdAsync(patientId);
                allNotes.AddRange(notes);
            }

            if (allNotes.Any())
            {
                var bulkIndexResponse = await openSearchClient.BulkAsync(b => b
                    .Index(indexName)
                    .IndexMany(allNotes)
                    .Refresh(Refresh.WaitFor)
                );
                if (!bulkIndexResponse.IsValid)
                {
                    throw new Exception($"Erreur lors de l'indexation en masse: {bulkIndexResponse.ServerError?.Error?.Reason}");
                }
            }
        }

        public async Task IndexNoteAsync(NoteDto note)
        {
            var indexResponse = await openSearchClient.IndexDocumentAsync(note);
            if (!indexResponse.IsValid)
            {
                throw new Exception($"Erreur lors de l'indexation de la note: {indexResponse.ServerError?.Error?.Reason}");
            }
        }

        public async Task DeleteNoteFromIndexAsync(string noteId)
        {
            var deleteResponse = await openSearchClient.DeleteAsync<NoteDto>(noteId, d => d.Index(indexName));
            if (!deleteResponse.IsValid)
            {
                throw new Exception($"Erreur lors de la suppression de la note de l'index: {deleteResponse.ServerError?.Error?.Reason}");
            }
        }
        
        public async Task DeleteIndexAsync()
        {
            var existsResponse = await openSearchClient.Indices.ExistsAsync(indexName);
            if (existsResponse.Exists)
            {
                await openSearchClient.Indices.DeleteAsync(indexName);
            }
        }
    }
}
