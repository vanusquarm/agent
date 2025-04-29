using Finacle.Core;
using FluentValidation;
using Transactions.API.Entities.DTOs;

namespace Transactions.API.Services;

// Services/TransferValidator.cs
public class TransferValidator //: ITransferValidator
{
    private readonly FinacleClient _accountService;
    private readonly ILogger<TransferValidator> _logger;

    public TransferValidator(
        FinacleClient accountService,
        ILogger<TransferValidator> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    public async Task<TransferValidationResult> ValidateAsync(AccountTransferRequest request)
    {
        var result = new TransferValidationResult();

        // Basic field validation
        if (!ValidateFields(request, out var fieldValidationError))
        {
            result.ErrorMessage = fieldValidationError;
            return result;
        }

        // Account validation
        var accountValidation = await ValidateAccountAsync(request);
        if (!accountValidation.IsValid)
        {
            return accountValidation;
        }

        // Risk assessment

        result.IsValid = true;
        return result;
    }

    public bool isValid(PerformChargeInquiryDto request, out List<string> Errors)
    {
        PerformChargeInquiryDtoValidator accountValidator = new PerformChargeInquiryDtoValidator();
        var validationResult = accountValidator.Validate(request);
        Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return validationResult.IsValid;
    }

    public bool isValid(PerformChargeRequestDto request, out List<string> Errors)
    {
        PerformChargeRequestDtoValidator accountValidator = new PerformChargeRequestDtoValidator();
        var validationResult = accountValidator.Validate(request);
        Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return validationResult.IsValid;
    }

    private bool ValidateFields(AccountTransferRequest request, out string errorMessage)
    {
        errorMessage = null;
        var validator = new AccountTransferRequestValidator();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            errorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.SourceAccountId))
        {
            errorMessage = "Sender account number is required";
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.DestinationAccountId))
        {
            errorMessage = "Recipient account number is required";
            return false;
        }

        if (request.Amount <= 0)
        {
            errorMessage = "Transfer amount must be positive";
            return false;
        }

        return true;
    }

    private async Task<TransferValidationResult> ValidateAccountAsync(AccountTransferRequest request)
    {
        var result = new TransferValidationResult();

        var senderAccount = await _accountService.GetAccountAsync(request.SourceAccountId);
        var recipientAccount = await _accountService.GetAccountAsync(request.DestinationAccountId);
        double.TryParse(senderAccount.EffectiveAvailableBalanceAmount.ToString(), out var availableBalance);

        if (senderAccount?.AccountName == null)
        {
            result.ErrorMessage = "Sender account not found";
            return result;
        }

        if (recipientAccount?.AccountName == null)
        {
            result.ErrorMessage = "Recipient account not found";
            return result;
        }

        // Exclude office or internal accounts. These accounts are allowed to run into overdraft.
        if (availableBalance < (double)request.Amount && !senderAccount.AccountIdentification.StartsWith("0"))
        {
            result.ErrorMessage = "Insufficient funds";
            return result;
        }

        // Check currency match
        if (senderAccount.AccountCurrency != recipientAccount.AccountCurrency)
        {
            result.ErrorMessage = "Sender account currency does not match recipient's";
            return result;
        }

        // Check against account transfer limit

        // Check daily cumulative limit

        result.IsValid = true;
        return result;
    }
}

public class AccountTransferRequestValidator : AbstractValidator<AccountTransferRequest>
{
    public AccountTransferRequestValidator()
    {
        RuleFor(x => x.SourceAccountId)
            .NotEmpty().WithMessage("Source account ID is required.")
            .Matches(@"^\d{10,20}$").WithMessage("Source account ID must be a numeric string between 10 and 20 characters.");

        RuleFor(x => x.DestinationAccountId)
            .NotEmpty().WithMessage("Destination account ID is required.")
            .Matches(@"^\d{10,20}$").WithMessage("Destination account ID must be a numeric string between 10 and 20 characters.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Remarks)
            .MaximumLength(500).WithMessage("Remarks cannot exceed 500 characters.");

        RuleFor(x => x.ChannelReference)
            .NotEmpty().WithMessage("Channel reference is required.");

        RuleFor(x => x.ChannelId)
            .NotEmpty().WithMessage("Channel ID is required.");

        RuleFor(x => x.DocNum)
            .MaximumLength(50).WithMessage("Document number cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.DocNum));

        RuleFor(x => x.UserPartTransactionCode)
            .MaximumLength(50).WithMessage("User part transaction code cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.UserPartTransactionCode));

        RuleFor(x => x.ChargeEventId)
            .MaximumLength(50).WithMessage("Charge event ID cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.ChargeEventId));

        RuleFor(x => x.BatchId)
            .MaximumLength(50).WithMessage("Batch ID cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.BatchId));
    }
}

public class PerformChargeInquiryDtoValidator : AbstractValidator<PerformChargeInquiryDto>
{
    public PerformChargeInquiryDtoValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("AccountId is required.")
            .Matches(@"^\d{10,20}$").WithMessage("Account ID must be a numeric string between 10 and 20 characters.");

        RuleFor(x => x.TranAmt)
            .GreaterThan(0).WithMessage("TranAmt must be greater than zero.");

        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("EventId is required.");

        RuleFor(x => x.EventType)
            .NotEmpty().WithMessage("EventType is required.");
    }
}

public class PerformChargeRequestDtoValidator : AbstractValidator<PerformChargeRequestDto>
{
    public PerformChargeRequestDtoValidator()
    {
        RuleFor(x => x.TransactionReference)
            .NotEmpty().WithMessage("TransactionReference is required.");

        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("AccountId is required.")
            .Matches(@"^\d{10,20}$").WithMessage("Account ID must be a numeric string between 10 and 20 characters.");

        RuleFor(x => x.ChargeType)
            .NotEmpty().WithMessage("ChargeType is required.");

        RuleFor(x => x.ChargeAmount)
            .GreaterThan(0).WithMessage("ChargeAmount must be greater than zero.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Must(currency => currency == "GHS").WithMessage("Currency must be 'GHS'.");
    }
}