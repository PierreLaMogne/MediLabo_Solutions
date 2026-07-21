namespace MediLabo_Solutions.PatientService.Services
{
    public interface INoteServiceClient
    {
        Task<long> DeleteNotesByPatientIdAsync(int patientId);
    }
}
