using Blockchain.Core.Models;

namespace Blockchain.Core.Abstractions;

public interface IHashService
{
    string ComputeSha256(string value);
    string ComputeTransactionHash(Transaction transaction);
    string ComputeMerkleRoot(IReadOnlyCollection<Transaction> transactions);
    string ComputeBlockHash(BlockHeader header);
}
