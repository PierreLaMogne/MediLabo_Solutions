using MediLabo_Solutions.Shared.Models;
using OpenSearch.Client;
using OpenSearch.Net;

namespace MediLabo_Solutions.NoteService.Services
{
    public class NoteSearchService(
        IOpenSearchClient openSearchClient,
        IConfiguration configuration) : INoteSearchService
    {
        private string indexName = configuration["OpenSearchSettings:IndexName"] ?? "notes";
        public async Task CreateIndexAsync()
        {
            var indexExists = await openSearchClient.Indices.ExistsAsync(indexName);
            if (indexExists.Exists)
                return;

            // Créer l'index uniquement s'il n'existe pas
            var createIndexResponse = await openSearchClient.Indices.CreateAsync(indexName, c => c
                .Settings(s => s
                    .NumberOfShards(1)
                    .NumberOfReplicas(0)
                    .Analysis(a => a
                        .TokenFilters(tf => tf
                           .Stemmer("french_stemmer", st => st.Language("french"))
                           .Elision("french_elision", e => e
                                .Articles("l", "m", "t", "qu", "n", "s", "j", "d", "c", "jusqu", "quoiqu", "lorsqu", "puisqu"))
                           .Stop("french_stop", st => st.StopWords("_french_"))
                        )
                        .Analyzers(an => an
                            .Custom("french_medical_analyzer", ca => ca
                                .Tokenizer("standard")
                                .Filters("lowercase", "french_elision", "french_stop","french_stemmer", "asciifolding")
                            )                            
                        )
                        .Normalizers(n => n
                            .Custom("lowercase_normalizer", cn => cn
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
                                    .Normalizer("lowercase_normalizer")
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
                throw new Exception($"Erreur de création de l'index: {createIndexResponse.ServerError?.Error?.Reason ?? createIndexResponse.OriginalException?.Message}");
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
                                .Term(t => t.Field(f => f.PatientId).Value(patientId)
                                )
                            )
                            .Should(
                                sh => sh.Match(ma => ma.Field(f => f.Contenu).Query(term).Operator(Operator.And).Boost(3.0)),
                                sh => sh.Match(ma => ma.Field("contenu.stemmed").Query(term).Boost(2.0)),
                                sh => sh.Fuzzy(fz => fz.Field(f => f.Contenu).Value(term).Fuzziness(Fuzziness.Auto).PrefixLength(2).MaxExpansions(50).Boost(1.5)),
                                sh => sh.Wildcard(w => w.Field("contenu.exact").Value($"*{term.ToLower()}*").CaseInsensitive(true).Boost(1.0))
                            )
                            .MinimumShouldMatch(1)
                        )
                    )
                    .Size(1000)
                );

                if (searchResponse.IsValid && searchResponse.Documents.Any())
                {
                    identifiedTriggers.Add(term);
                }
            }

            return identifiedTriggers;
        }

        public async Task IndexNoteAsync(NoteDto note)
        {
            var indexResponse = await openSearchClient.IndexDocumentAsync(note);
            if (!indexResponse.IsValid)
            {
                throw new Exception($"Erreur lors de l'indexation de la note: {indexResponse.ServerError?.Error?.Reason}");
            }
        }

        public async Task IndexNotesAsync(IEnumerable<NoteDto> notes)
        {
            var notesList = notes.ToList();
            if (!notesList.Any())
                return;

            var bulkIndexResponse = await openSearchClient.BulkAsync(b => b
                .Index(indexName)
                .IndexMany(notes, (descriptor, note) => descriptor.Id(note.Id))
                .Refresh(Refresh.WaitFor)
            );
            if (!bulkIndexResponse.IsValid)
            {
                throw new Exception($"Erreur lors de l'indexation en masse: {bulkIndexResponse.ServerError?.Error?.Reason}");
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

        public async Task<long> CountDocumentAsync()
        {
            var countResponse = await openSearchClient.CountAsync<NoteDto>(c => c.Index(indexName));
            if (!countResponse.IsValid)
            {
                throw new Exception($"Erreur lors du comptage des documents: {countResponse.ServerError?.Error?.Reason}");
            }
            Console.WriteLine($"Nombre de documents dans l'index '{indexName}': {countResponse.Count}");
            return countResponse.Count;
        }
    }
}
