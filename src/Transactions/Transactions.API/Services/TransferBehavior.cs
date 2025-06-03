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

    public bool isValid(CashWithdrawalRequest request, out List<string> Errors)
    {
        CashWithdrawalRequestValidator accountValidator = new CashWithdrawalRequestValidator();
        var validationResult = accountValidator.Validate(request);
        Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return validationResult.IsValid;
    }

    public bool isValid(CashDepositRequest request, out List<string> Errors)
    {
        CashDepositRequestValidator accountValidator = new CashDepositRequestValidator();
        var validationResult = accountValidator.Validate(request);
        Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return validationResult.IsValid;
    }

    public bool isValid(PerformCashTransferRequest request, out List<string> Errors)
    {
        PerformCashTransferRequestValidator accountValidator = new PerformCashTransferRequestValidator();
        var validationResult = accountValidator.Validate(request);
        Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return validationResult.IsValid;
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

    public bool isValid(StatementChargeRquest request, out List<string> Errors)
    {
        StatementChargeRquestValidator chargeRequestValidator = new StatementChargeRquestValidator();
        var validationResult = chargeRequestValidator.Validate(request);
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

        if (request.SourceAccountId == request.DestinationAccountId)
        {
            errorMessage = "Source and destination account numbers must not be the same.";
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

    public async Task<string> GetTellerSolIdByIdAsync(string tellerId)
    {
        try
        {
            var tellerDetails = await _accountService.GetTellerDetailsAsync(tellerId);

            if (tellerDetails == null || string.IsNullOrEmpty(tellerDetails.SolId))
            {
                _logger.LogWarning($"Teller with ID {tellerId} not found.");
                return "";
            }

            var solId = tellerDetails.SolId;

            _logger.LogInformation($"Teller with ID {tellerId} is valid. SolId: {solId}");
            return solId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while validating teller with ID {tellerId}.");
            return "";
        }

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

public class CashWithdrawalRequestValidator : AbstractValidator<CashWithdrawalRequest>
{
    public CashWithdrawalRequestValidator()
    {
        RuleFor(x => x.TellerId)
            .NotEmpty().WithMessage("TellerID is required.")
            .MaximumLength(10).WithMessage("TellerID cannot exceed 10 characters.");

        RuleFor(x => x.CreditAccount)
            .NotEmpty().WithMessage("CreditAccount is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Remarks)
            .MaximumLength(50).WithMessage("Remarks cannot exceed 50 characters.");

        RuleFor(x => x.ChannelReference)
            .NotEmpty().WithMessage("ChannelReference is required.")
            .MaximumLength(80).WithMessage("ChannelReference cannot exceed 80 characters.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .When(x => x.Currency != null);

        RuleForEach(x => x.DenomDetails)
            .SetValidator(new DenominationDetailsCurrencyTypeValidator())
            .When(x => x.DenomDetails != null);
    }
}

public class CashDepositRequestValidator : AbstractValidator<CashDepositRequest>
{
    public CashDepositRequestValidator()
    {
        RuleFor(x => x.TellerId)
            .NotEmpty().WithMessage("TellerID is required.")
            .MaximumLength(10).WithMessage("TellerID cannot exceed 10 characters.");

        RuleFor(x => x.DebitAccount)
            .NotEmpty().WithMessage("DebitAccount is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Remarks)
            .MaximumLength(50).WithMessage("Remarks cannot exceed 50 characters.");

        RuleFor(x => x.ChannelReference)
            .NotEmpty().WithMessage("ChannelReference is required.")
            .MaximumLength(80).WithMessage("ChannelReference cannot exceed 80 characters.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .When(x => x.Currency != null);

        RuleForEach(x => x.DenomDetails)
            .SetValidator(new DenominationDetailsCurrencyTypeValidator())
            .When(x => x.DenomDetails != null);
    }
}

public class PerformCashTransferRequestValidator : AbstractValidator<PerformCashTransferRequest>
{
    public PerformCashTransferRequestValidator()
    {
        RuleFor(x => x.TellerId)
            .NotEmpty().WithMessage("TellerID is required.")
            .MaximumLength(10).WithMessage("TellerID cannot exceed 10 characters.");

        RuleFor(x => x.CustomerAccount)
            .NotEmpty().WithMessage("CustomerAccount is required.")
            .Matches(@"^\d{10,20}$").WithMessage("CustomerAccount must be a numeric string between 10 and 20 characters.");


        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.TransType)
            .Must(state => state == "D" || state == "C")
            .WithMessage("Transaction Type must be either 'D' or 'C'.");

        RuleFor(x => x.Remarks)
            .NotEmpty().WithMessage("Remarks is required.")
            .MaximumLength(80).WithMessage("Remarks cannot exceed 80 characters.");

        RuleFor(x => x.ChannelReference)
            .NotEmpty().WithMessage("ChannelReference is required.")
            .MaximumLength(80).WithMessage("ChannelReference cannot exceed 80 characters.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .When(x => x.Currency != null);
    }
}

public class DenominationDetailsCurrencyTypeValidator : AbstractValidator<DenominationDetailsCurrencyType>
{
    public DenominationDetailsCurrencyTypeValidator()
    {
        RuleFor(x => x.DenominationCurrency)
            .NotEmpty().WithMessage("DenominationCurrency is required.");

        RuleFor(x => x.Denomination)
            .GreaterThan(0).WithMessage("Denomination must be greater than zero.");

        RuleFor(x => x.StrapOrBundleSize)
            .GreaterThanOrEqualTo(0).WithMessage("StrapOrBundleSize must be non-negative.");

        RuleFor(x => x.NumberOfNotesOrCoins)
            .GreaterThanOrEqualTo(0).WithMessage("NumberOfNotesOrCoins must be non-negative.");

        RuleFor(x => x.NumberOfStrapsOrBundles)
            .GreaterThanOrEqualTo(0).WithMessage("NumberOfStrapsOrBundles must be non-negative.");
    }
}

internal class StatementChargeRquestValidator : AbstractValidator<StatementChargeRquest>
{
    public StatementChargeRquestValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Account Number is required.")
            .Matches(@"^\d{10,20}$").WithMessage("Account Number must be a numeric string between 10 and 20 characters.");
        RuleFor(x => x.NoOfPages)
            .NotEmpty().WithMessage("Number of pages is required.");
        RuleFor(x => x.Remarks)
            .MaximumLength(50).WithMessage("Remarks cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.Remarks));
    }
}