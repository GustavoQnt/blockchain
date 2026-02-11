namespace Blockchain.Core.Models;

public sealed record AccountState(
    string Address,
    decimal Balance,
    long NextNonce);
