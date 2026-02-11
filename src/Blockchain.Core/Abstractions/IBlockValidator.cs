using Blockchain.Core.Models;
using Blockchain.Core.Validation;

namespace Blockchain.Core.Abstractions;

public interface IBlockValidator
{
    ValidationResult Validate(
        Block block,
        Block? previousBlock,
        IReadOnlyDictionary<string, AccountState> accounts);
}
