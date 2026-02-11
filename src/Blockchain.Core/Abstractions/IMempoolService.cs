using Blockchain.Core.Models;
using Blockchain.Core.Validation;

namespace Blockchain.Core.Abstractions;

public interface IMempoolService
{
    IReadOnlyCollection<Transaction> GetPending();
    ValidationResult TryAdd(Transaction transaction);
    void RemoveById(IEnumerable<string> transactionIds);
}
