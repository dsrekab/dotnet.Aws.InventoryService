namespace VerificationService.Services.Interfaces
{
    public interface IVerificationService<T>
    {
        Task<bool> VerifyAllRequiredFields(T itemToVerify);
    }
}
