using Blockchain.Core.Abstractions;
using Blockchain.Core.Models;

namespace Blockchain.Core.Validation;

public sealed class TransactionValidator(ISignatureService signatureService) : ITransactionValidator
{
    public ValidationResult Validate(
        Transaction transaction,
        IReadOnlyDictionary<string, AccountState> accounts)
    {
        if (string.IsNullOrWhiteSpace(transaction.To))
        {
            return ValidationResult.Failure(ValidationErrorCode.InvalidAddress, "Recipient address is required.");
        }

        if (transaction.Amount <= 0)
        {
            return ValidationResult.Failure(ValidationErrorCode.InvalidAmount, "Amount must be greater than zero.");
        }

        if (transaction.TimestampUtc == default)
        {
            return ValidationResult.Failure(ValidationErrorCode.Unknown, "Timestamp is required.");
        }

        if (transaction.IsCoinbase)
        {
            if (transaction.Nonce != 0)
            {
                return ValidationResult.Failure(ValidationErrorCode.NonceMismatch, "Coinbase transaction nonce must be zero.");
            }

            return ValidationResult.Success();
        }

        if (string.IsNullOrWhiteSpace(transaction.From))
        {
            return ValidationResult.Failure(ValidationErrorCode.InvalidAddress, "Sender address is required.");
        }

        if (string.Equals(transaction.From, transaction.To, StringComparison.Ordinal))
        {
            return ValidationResult.Failure(ValidationErrorCode.SameSenderAndRecipient, "Sender and recipient cannot be the same.");
        }

        var senderAccount = GetOrDefault(accounts, transaction.From);

        if (transaction.Nonce != senderAccount.NextNonce)
        {
            return ValidationResult.Failure(
                ValidationErrorCode.NonceMismatch,
                $"Expected nonce {senderAccount.NextNonce}, got {transaction.Nonce}.");
        }

        if (senderAccount.Balance < transaction.Amount)
        {
            return ValidationResult.Failure(ValidationErrorCode.InsufficientBalance, "Insufficient balance.");
        }

        if (string.IsNullOrWhiteSpace(transaction.PublicKeyHex) || string.IsNullOrWhiteSpace(transaction.SignatureHex))
        {
            return ValidationResult.Failure(ValidationErrorCode.InvalidSignature, "Signature and public key are required.");
        }

        if (!signatureService.VerifyTransactionSignature(transaction))
        {
            return ValidationResult.Failure(ValidationErrorCode.InvalidSignature, "Invalid transaction signature.");
        }

        return ValidationResult.Success();
    }

    private static AccountState GetOrDefault(
        IReadOnlyDictionary<string, AccountState> accounts,
        string address)
    {
        return accounts.TryGetValue(address, out var existing)
            ? existing
            : new AccountState(address, 0m, 0);
    }
}
