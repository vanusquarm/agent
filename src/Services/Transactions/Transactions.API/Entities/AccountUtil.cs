using Transactions.API.Entities.DTOs;

namespace Transactions.API.Entities
{
    public class AccountUtil
    {
        public bool IsAccountTheSame(string DebitAccount, string CreditAccount)
        {
            if (DebitAccount == CreditAccount)
            {
                return true;
            }
            return false;
        }

        public bool IsAccountBalanceLessThanAmountToTransfer(double DebitAccountBalance, double amount, string AccountNumber, LiensList[]? DebitlienDetailsList)
        {
            double lienAmount = 0;
            if (DebitlienDetailsList != null)
            {
                foreach (var lien in DebitlienDetailsList)
                {
                    lienAmount += Convert.ToDouble(lien.lienReleaseAmount);
                }
            }
            double totalBalance = lienAmount + DebitAccountBalance;

            if (AccountNumber.Length > 10) { return false; }
            else
            {
                if (totalBalance < amount) { return true; }
                return false;
            }
        }

        public bool IsAccountCurrencyTheSame(string DebitAccountCurrency, string CreditAccountCurrency, string TransactionCurrency, string RequestId)
        {
            bool iscurrencySame = false;

            //_logger.LogInformation("RequestId: {RequestId}. Currencies are DebitCurrency: {DebitCurrency}, CreditCurrency: {CreditCurrency}, and TransactionCurrency: {TransactionCurrency}", RequestId, DebitAccountCurrency, CreditAccountCurrency, TransactionCurrency);
            if (DebitAccountCurrency == CreditAccountCurrency)
            { iscurrencySame = true; }

            else { iscurrencySame = false; }

            if (!string.IsNullOrEmpty(TransactionCurrency) && iscurrencySame)
            {
                if (DebitAccountCurrency == TransactionCurrency && CreditAccountCurrency == TransactionCurrency)
                { iscurrencySame = true; }

                else { iscurrencySame = false; }
            }
            return iscurrencySame;
        }
    }

}
