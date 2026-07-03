namespace MediLabo_Solutions.Frontend.Services
{
    /// <summary>
    /// Service pour notifier les composants Blazor des changements sur les données des patients et notes.
    /// Permet de déclencher des rechargements automatiques après des opérations CRUD sur les notes.
    /// </summary>
    public class PatientDataRefreshService
    {
        private readonly IHttpCacheService _cacheService;

        public PatientDataRefreshService(IHttpCacheService cacheService)
        {
            _cacheService = cacheService;
        }

        /// <summary>
        /// Événement déclenché lorsqu'une note est créée, modifiée ou supprimée.
        /// </summary>
        public event Action? OnNoteChanged;

        /// <summary>
        /// Notifie tous les abonnés qu'une note a été modifiée et invalide le cache associé.
        /// </summary>
        /// <param name="patientId">ID du patient dont les données ont changé</param>
        public void NotifyNoteChanged(int patientId)
        {
            // Invalider le cache pour ce patient
            _cacheService.Remove($"riskassessment_{patientId}");
            _cacheService.Remove($"notes_{patientId}");
            
            // Notifier les composants
            OnNoteChanged?.Invoke();
        }
    }
}