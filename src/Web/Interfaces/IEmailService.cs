namespace Statement.Web.Interfaces
{
    public interface IEmailService
    {
        //Task<bool> SendStatementEmailAsync(string to, string subject, string htmlBody, string attachmentPath);
        //string SendMessage(string sendTo, string copyTo, string sendFrom, string sendSubject, string sendMessage);

        string SendMessage(
            string sendTo,
            string? copyTo,
            string sendFrom,
            string sendSubject,
            string sendMessage);
        bool SendIntroHtmlMessage(
        string sendTo,
        string sendFrom,
        string sendSubject,
        string customerName,
        string statementPeriod,
        string attachmentPath,
        string accountNumToEmail,
        string productToEmail);
    }
}
