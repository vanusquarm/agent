using FluentValidation;

namespace Customers.API.Validators
{
    public class CustomerValidator 
    {
        private readonly ILogger<CustomerValidator> _logger;

        public CustomerValidator(
            ILogger<CustomerValidator> logger)
        {
            _logger = logger;
        }

        public static bool IsValid(string request, out List<string> Errors)
        {
            StringValidator accountValidator = new StringValidator(request);
            var validationResult = accountValidator.Validate(request);
            Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return validationResult.IsValid;
        }
    }

    internal class StringValidator : AbstractValidator<string>
    {
        public StringValidator(string request)
        {
             RuleFor(x => x)
                .NotEmpty().WithMessage($"{nameof(request)} is required.")
                .Matches(@"^[a-zA-Z0-9]{10,20}$")
                .WithMessage($"{nameof(request)} must be an alphanumeric string between 10 and 20 characters.");
        }
    }
}