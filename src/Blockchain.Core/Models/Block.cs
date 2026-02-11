namespace Blockchain.Core.Models;

public sealed record Block(
    BlockHeader Header,
    IReadOnlyList<Transaction> Transactions,
    string Hash);
