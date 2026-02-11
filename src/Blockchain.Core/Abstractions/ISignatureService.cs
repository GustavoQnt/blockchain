using Blockchain.Core.Models;

namespace Blockchain.Core.Abstractions;

public interface ISignatureService
{
    bool VerifyTransactionSignature(Transaction transaction);
}
