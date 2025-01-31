namespace Finacle.Core.Domain
{
    public class EodStatusResponse
    {
        public string FirstBranchStatus { get; set; }
        public string SecondBranchStatus { get; set; }
        public string BankStatus { get; set; }
        public string ErrorFlg { get; set; }
    }
}