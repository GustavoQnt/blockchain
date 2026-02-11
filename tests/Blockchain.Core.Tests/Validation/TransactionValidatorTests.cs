using Blockchain.Core.Abstractions;
using Blockchain.Core.Models;
using Blockchain.Core.Validation;
using FluentAssertions;

namespace Blockchain.Core.Tests.Validation;

public sealed class TransactionValidatorTests
{
    [Fact]
    public void Validate_WhenTransactionIsValid_ShouldSucceed()
    {
        var validator = new TransactionValidator(new StubSignatureService(isValid: true));
        var accounts = new Dictionary<string, AccountState>(StringComparer.Ordinal)
        {
            ["alice"] = new("alice", 10m, 2)
        };

        var transaction = new Transaction(
            Id: "tx-1",
            From: "alice",
            To: "bob",
            Amount: 3m,
            Nonce: 2,
            PublicKeyHex: "pub",
            SignatureHex: "sig",
            TimestampUtc: DateTime.UtcNow);

        var result = validator.Validate(transaction, accounts);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenNonceIsUnexpected_ShouldFail()
    {
        var validator = new TransactionValidator(new StubSignatureService(isValid: true));
        var accounts = new Dictionary<string, AccountState>(StringComparer.Ordinal)
        {
            ["alice"] = new("alice", 10m, 4)
        };

        var transaction = new Transaction(
            Id: "tx-1",
            From: "alice",
            To: "bob",
            Amount: 3m,
            Nonce: 3,
            PublicKeyHex: "pub",
            SignatureHex: "sig",
            TimestampUtc: DateTime.UtcNow);

        var result = validator.Validate(transaction, accounts);

        result.IsValid.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.NonceMismatch);
    }

    [Fact]
    public void Validate_WhenBalanceIsInsufficient_ShouldFail()
    {
        var validator = new TransactionValidator(new StubSignatureService(isValid: true));
        var accounts = new Dictionary<string, AccountState>(StringComparer.Ordinal)
        {
            ["alice"] = new("alice", 1m, 0)
        };

        var transaction = new Transaction(
            Id: "tx-1",
            From: "alice",
            To: "bob",
            Amount: 3m,
            Nonce: 0,
            PublicKeyHex: "pub",
            SignatureHex: "sig",
            TimestampUtc: DateTime.UtcNow);

        var result = validator.Validate(transaction, accounts);

        result.IsValid.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.InsufficientBalance);
    }

    [Fact]
    public void Validate_WhenSenderEqualsRecipient_ShouldFail()
    {
        var validator = new TransactionValidator(new StubSignatureService(isValid: true));
        var accounts = new Dictionary<string, AccountState>(StringComparer.Ordinal)
        {
            ["alice"] = new("alice", 10m, 0)
        };

        var transaction = new Transaction(
            Id: "tx-1",
            From: "alice",
            To: "alice",
            Amount: 1m,
            Nonce: 0,
            PublicKeyHex: "pub",
            SignatureHex: "sig",
            TimestampUtc: DateTime.UtcNow);

        var result = validator.Validate(transaction, accounts);

        result.IsValid.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.SameSenderAndRecipient);
    }

    [Fact]
    public void Validate_WhenCoinbaseNonceIsZero_ShouldSucceed()
    {
        var validator = new TransactionValidator(new StubSignatureService(isValid: false));
        var transaction = new Transaction(
            Id: "coinbase-1",
            From: ProtocolConstants.CoinbaseSender,
            To: "miner",
            Amount: 50m,
            Nonce: 0,
            PublicKeyHex: string.Empty,
            SignatureHex: string.Empty,
            TimestampUtc: DateTime.UtcNow);

        var result = validator.Validate(
            transaction,
            new Dictionary<string, AccountState>(StringComparer.Ordinal));

        result.IsValid.Should().BeTrue();
    }

    private sealed class StubSignatureService(bool isValid) : ISignatureService
    {
        public bool VerifyTransactionSignature(Transaction transaction) => isValid;
    }
}
