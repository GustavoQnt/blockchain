using Blockchain.Core.Abstractions;
using Blockchain.Core.Models;
using Blockchain.Core.Services;
using Blockchain.Core.Validation;
using FluentAssertions;

namespace Blockchain.Core.Tests.Validation;

public sealed class BlockValidatorTests
{
    private readonly IHashService _hashService = new Sha256HashService();

    [Fact]
    public void Validate_WhenBlockIsValid_ShouldSucceed()
    {
        var transactionValidator = new TransactionValidator(new StubSignatureService(isValid: true));
        var blockValidator = new BlockValidator(_hashService, transactionValidator);

        var previousBlock = BuildGenesisBlock();
        var coinbase = BuildCoinbaseTransaction("miner", 50m);
        var transfer = BuildTransferTransaction("alice", "bob", amount: 2m, nonce: 0);
        var transactions = new[] { coinbase, transfer };

        var header = new BlockHeader(
            Height: 1,
            PreviousHash: previousBlock.Hash,
            MerkleRoot: _hashService.ComputeMerkleRoot(transactions),
            Nonce: 0,
            Difficulty: 0,
            TimestampUtc: DateTime.UtcNow,
            MinerAddress: "miner");

        var block = new Block(header, transactions, _hashService.ComputeBlockHash(header));
        var accounts = new Dictionary<string, AccountState>(StringComparer.Ordinal)
        {
            ["alice"] = new("alice", 10m, 0)
        };

        var result = blockValidator.Validate(block, previousBlock, accounts);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenPreviousHashIsWrong_ShouldFail()
    {
        var transactionValidator = new TransactionValidator(new StubSignatureService(isValid: true));
        var blockValidator = new BlockValidator(_hashService, transactionValidator);

        var previousBlock = BuildGenesisBlock();
        var coinbase = BuildCoinbaseTransaction("miner", 50m);
        var transactions = new[] { coinbase };

        var header = new BlockHeader(
            Height: 1,
            PreviousHash: "wrong-hash",
            MerkleRoot: _hashService.ComputeMerkleRoot(transactions),
            Nonce: 0,
            Difficulty: 0,
            TimestampUtc: DateTime.UtcNow,
            MinerAddress: "miner");

        var block = new Block(header, transactions, _hashService.ComputeBlockHash(header));

        var result = blockValidator.Validate(
            block,
            previousBlock,
            new Dictionary<string, AccountState>(StringComparer.Ordinal));

        result.IsValid.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidPreviousHash);
    }

    [Fact]
    public void Validate_WhenDifficultyCannotBeSatisfied_ShouldFail()
    {
        var transactionValidator = new TransactionValidator(new StubSignatureService(isValid: true));
        var blockValidator = new BlockValidator(_hashService, transactionValidator);

        var coinbase = BuildCoinbaseTransaction("miner", 50m);
        var transactions = new[] { coinbase };

        var header = new BlockHeader(
            Height: 0,
            PreviousHash: "0",
            MerkleRoot: _hashService.ComputeMerkleRoot(transactions),
            Nonce: 0,
            Difficulty: 65,
            TimestampUtc: DateTime.UtcNow,
            MinerAddress: "miner");

        var block = new Block(header, transactions, _hashService.ComputeBlockHash(header));

        var result = blockValidator.Validate(
            block,
            previousBlock: null,
            new Dictionary<string, AccountState>(StringComparer.Ordinal));

        result.IsValid.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidDifficulty);
    }

    private Block BuildGenesisBlock()
    {
        var coinbase = BuildCoinbaseTransaction("genesis", 50m);
        var transactions = new[] { coinbase };

        var header = new BlockHeader(
            Height: 0,
            PreviousHash: "0",
            MerkleRoot: _hashService.ComputeMerkleRoot(transactions),
            Nonce: 0,
            Difficulty: 0,
            TimestampUtc: DateTime.UtcNow,
            MinerAddress: "genesis");

        var hash = _hashService.ComputeBlockHash(header);
        return new Block(header, transactions, hash);
    }

    private static Transaction BuildCoinbaseTransaction(string to, decimal amount)
    {
        return new Transaction(
            Id: $"coinbase-{Guid.NewGuid()}",
            From: ProtocolConstants.CoinbaseSender,
            To: to,
            Amount: amount,
            Nonce: 0,
            PublicKeyHex: string.Empty,
            SignatureHex: string.Empty,
            TimestampUtc: DateTime.UtcNow);
    }

    private static Transaction BuildTransferTransaction(string from, string to, decimal amount, long nonce)
    {
        return new Transaction(
            Id: $"tx-{Guid.NewGuid()}",
            From: from,
            To: to,
            Amount: amount,
            Nonce: nonce,
            PublicKeyHex: "pub",
            SignatureHex: "sig",
            TimestampUtc: DateTime.UtcNow);
    }

    private sealed class StubSignatureService(bool isValid) : ISignatureService
    {
        public bool VerifyTransactionSignature(Transaction transaction) => isValid;
    }
}
