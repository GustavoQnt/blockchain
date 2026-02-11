using Blockchain.Core.Models;
using Blockchain.Core.Validation;

namespace Blockchain.Core.Abstractions;

public interface ITransactionValidator
{
    ValidationResult Validate(
        Transaction transaction,
        IReadOnlyDictionary<string, AccountState> accounts);
}
