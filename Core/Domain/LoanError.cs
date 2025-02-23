using System.Collections.Generic;

namespace Finacle.Core.Domain
{

    public class ExceptionDetailsDto
    {
        public string ExceptionCode { get; set; }
        public string ExceptionMessage { get; set; }
    }

    public class WarningDetailsDto
    {
        public string WarningCode { get; set; }
        public string WarningMessage { get; set; }
    }

    public class ErrorDetailsDto
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }



    public class LoanError
    {
        public List<ErrorDetailsDto> ErrorDetails { get; set; } = new List<ErrorDetailsDto>();
        public List<WarningDetailsDto> WarningDetails { get; set; } = new List<WarningDetailsDto>();
        public List<ExceptionDetailsDto> ExceptionDetails { get; set; } = new List<ExceptionDetailsDto>();
    }
}
