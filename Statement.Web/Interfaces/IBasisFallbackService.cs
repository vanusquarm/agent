namespace Statement.Web.Interfaces
{
    public interface IBasisFallbackService
    {
        Task<string> GetFinacleAccountFromBasisAccount(string basisAccount);
    }
}
