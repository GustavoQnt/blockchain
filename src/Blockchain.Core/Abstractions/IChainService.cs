using Blockchain.Core.Models;
using Blockchain.Core.Validation;

namespace Blockchain.Core.Abstractions;

public interface IChainService
{
    IReadOnlyList<Block> GetChain();
    Block? GetTip();
    ValidationResult TryAddBlock(Block block);
}
