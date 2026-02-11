namespace Blockchain.Core.Models;

public sealed record Transaction(
    string Id,
    string From,
    string To,
    decimal Amount,
    long Nonce,
    string PublicKeyHex,
    string SignatureHex,
    DateTime TimestampUtc)
{
    public bool IsCoinbase => string.Equals(From, ProtocolConstants.CoinbaseSender, StringComparison.Ordinal);
}
