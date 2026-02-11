using Blockchain.Core.Abstractions;
using Blockchain.Core.Models;

namespace Blockchain.Core.Validation;

public sealed class BlockValidator(
    IHashService hashService,
    ITransactionValidator transactionValidator) : IBlockValidator
{
    public ValidationResult Validate(
        Block block,
        Block? previousBlock,
        IReadOnlyDictionary<string, AccountState> accounts)
    {
        if (block.Transactions.Count == 0)
        {
            return ValidationResult.Failure(
                ValidationErrorCode.InvalidBlockTransactions,
                "Block must include at least one transaction.");
        }

        if (previousBlock is null)
        {
            if (block.Header.Height != 0)
            {
                return ValidationResult.Failure(
                    ValidationErrorCode.InvalidBlockHeight,
                    "Genesis block must have height 0.");
            }
        }
        else
        {
            if (block.Header.Height != previousBlock.Header.Height + 1)
            {
                return ValidationResult.Failure(
                    ValidationErrorCode.InvalidBlockHeight,
                    "Invalid block height.");
            }

            if (!string.Equals(block.Header.PreviousHash, previousBlock.Hash, StringComparison.Ordinal))
            {
                return ValidationResult.Failure(
                    ValidationErrorCode.InvalidPreviousHash,
                    "Previous hash does not match chain tip.");
            }
        }

        var merkleRoot = hashService.ComputeMerkleRoot(block.Transactions);
        if (!string.Equals(block.Header.MerkleRoot, merkleRoot, StringComparison.Ordinal))
        {
            return ValidationResult.Failure(
                ValidationErrorCode.InvalidMerkleRoot,
                "Block merkle root is invalid.");
        }

        var expectedHash = hashService.ComputeBlockHash(block.Header);
        if (!string.Equals(block.Hash, expectedHash, StringComparison.Ordinal))
        {
            return ValidationResult.Failure(
                ValidationErrorCode.InvalidBlockHash,
                "Block hash does not match header.");
        }

        if (!SatisfiesDifficulty(block.Hash, block.Header.Difficulty))
        {
            return ValidationResult.Failure(
                ValidationErrorCode.InvalidDifficulty,
                "Block hash does not satisfy difficulty.");
        }

        if (!block.Transactions[0].IsCoinbase)
        {
            return ValidationResult.Failure(
                ValidationErrorCode.MissingCoinbase,
                "First transaction must be coinbase.");
        }

        var coinbaseCount = block.Transactions.Count(transaction => transaction.IsCoinbase);
        if (coinbaseCount > 1)
        {
            return ValidationResult.Failure(
                ValidationErrorCode.MultipleCoinbase,
                "Block cannot contain more than one coinbase transaction.");
        }

        var workingState = CloneState(accounts);

        foreach (var transaction in block.Transactions)
        {
            var txValidation = transactionValidator.Validate(transaction, workingState);
            if (!txValidation.IsValid)
            {
                return txValidation;
            }

            Apply(workingState, transaction);
        }

        return ValidationResult.Success();
    }

    private static bool SatisfiesDifficulty(string hash, int difficulty)
    {
        if (difficulty <= 0)
        {
            return true;
        }

        if (hash.Length < difficulty)
        {
            return false;
        }

        for (var i = 0; i < difficulty; i++)
        {
            if (hash[i] != '0')
            {
                return false;
            }
        }

        return true;
    }

    private static Dictionary<string, AccountState> CloneState(
        IReadOnlyDictionary<string, AccountState> accounts)
    {
        var clone = new Dictionary<string, AccountState>(StringComparer.Ordinal);
        foreach (var (address, state) in accounts)
        {
            clone[address] = state with { };
        }

        return clone;
    }

    private static void Apply(
        IDictionary<string, AccountState> accounts,
        Transaction transaction)
    {
        if (transaction.IsCoinbase)
        {
            var recipient = GetOrDefault(accounts, transaction.To);
            accounts[transaction.To] = recipient with
            {
                Balance = recipient.Balance + transaction.Amount
            };
            return;
        }

        var sender = GetOrDefault(accounts, transaction.From);
        var recipientAccount = GetOrDefault(accounts, transaction.To);

        accounts[transaction.From] = sender with
        {
            Balance = sender.Balance - transaction.Amount,
            NextNonce = sender.NextNonce + 1
        };

        accounts[transaction.To] = recipientAccount with
        {
            Balance = recipientAccount.Balance + transaction.Amount
        };
    }

    private static AccountState GetOrDefault(
        IDictionary<string, AccountState> accounts,
        string address)
    {
        return accounts.TryGetValue(address, out var state)
            ? state
            : new AccountState(address, 0m, 0);
    }
}
