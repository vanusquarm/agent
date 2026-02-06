namespace Accounts.API.Entities.Models
{
    public class Account
    {
        Account(string accountNumber)
        {
            AccountNumber = accountNumber;
        }

        public required string AccountNumber { get; set; }
    }
}