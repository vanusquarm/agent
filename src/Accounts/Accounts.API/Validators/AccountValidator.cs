using Acccounts.API.Entities.DTOs;
using FluentValidation;

namespace Accounts.API.Validators
{
    public class AccountValidator : AbstractValidator<object>
    {
        public AccountValidator()
        {
            // For string account number validation
            When(x => x is string, () =>
            {
                RuleFor(x => x as string)
                    .NotEmpty().WithMessage("Account number is required.")
                    .Matches(@"^\d{10,20}$").WithMessage("Account number must be a numeric string between 10 and 20 characters.");
            });

            // For UnFreezeAccountRequest validation
            When(x => x is UnFreezeAccountRequest, () =>
            {
                RuleFor(x => (x as UnFreezeAccountRequest).CustomerId)
                    .NotEmpty().WithMessage("CustomerId is required")
                    .Matches("^[0-9]{9,25}$").WithMessage("CustomerId can only contain numbers or have a max length of 25");

                RuleFor(x => (x as UnFreezeAccountRequest).ReasonCode)
                    .NotEmpty().WithMessage("ReasonCode is required");

                RuleFor(x => (x as UnFreezeAccountRequest).Remarks)
                    .MaximumLength(500).WithMessage("Freeze remarks cannot exceed 500 characters.");
            });

            // For AccountStatementRequest validation
            When(x => x is AccountStatementRequest, () =>
            {
                RuleFor(x => (x as AccountStatementRequest).FromDate)
                    .NotEmpty().WithMessage("From date is required.")
                    .Must(BeAValidDate).WithMessage("From date must be a valid date in the format yyyy-MM-dd.");

                RuleFor(x => (x as AccountStatementRequest).ToDate)
                    .NotEmpty().WithMessage("To date is required.")
                    .Must(BeAValidDate).WithMessage("To date must be a valid date in the format yyyy-MM-dd.")
                    .GreaterThanOrEqualTo(x => (x as AccountStatementRequest).FromDate)
                    .WithMessage("To date must be greater than or equal to From date.");
            });

            // For FreezeAccountRequest validation
            When(x => x is FreezeAccountRequest, () =>
            {
                RuleFor(x => (x as FreezeAccountRequest).CustomerId)
                    .NotEmpty().WithMessage("Customer ID is required.")
                    .Matches(@"^\d{10,20}$").WithMessage("Customer ID must be a numeric string between 10 and 20 characters.");

                RuleFor(x => (x as FreezeAccountRequest).AccountNumber)
                    .NotEmpty().WithMessage("Account number is required.")
                    .Matches(@"^\d{10,20}$").WithMessage("Account number must be a numeric string between 10 and 20 characters.");

                RuleFor(x => (x as FreezeAccountRequest).ReasonType)
                    .NotEmpty().WithMessage("Reason type is required.")
                    .MaximumLength(50).WithMessage("Reason type cannot exceed 50 characters.");

                RuleFor(x => (x as FreezeAccountRequest).ReasonCode)
                    .NotEmpty().WithMessage("Reason code is required.")
                    .MaximumLength(50).WithMessage("Reason code cannot exceed 50 characters.");

                RuleFor(x => (x as FreezeAccountRequest).FreezeRemarks)
                    .MaximumLength(500).WithMessage("Freeze remarks cannot exceed 500 characters.");
            });

            // For TerminalOfficeAcctRequest validation
            When(x => x is TerminalOfficeAcctRequest, () =>
            {
                RuleFor(x => (x as TerminalOfficeAcctRequest).TerminalId)
                    .NotEmpty().WithMessage("Terminal ID is required.")
                    .MaximumLength(50).WithMessage("Terminal ID cannot exceed 50 characters.");
            });
        }

        private bool BeAValidDate(string date)
        {
            return DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _);
        }
    }
}