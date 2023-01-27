namespace VerificationService.Services.Interfaces
{
    public interface IVerificationService<T>
    {
        Task<bool> VerifyRequiredFields(T itemToVerify);
    }
}
