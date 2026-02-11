namespace Blockchain.Core.Validation;

public sealed record ValidationResult(
    bool IsValid,
    ValidationErrorCode? ErrorCode = null,
    string? ErrorMessage = null)
{
    public static ValidationResult Success() => new(true);

    public static ValidationResult Failure(ValidationErrorCode errorCode, string errorMessage) =>
        new(false, errorCode, errorMessage);
}
