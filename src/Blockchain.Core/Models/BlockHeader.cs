namespace Blockchain.Core.Models;

public sealed record BlockHeader(
    int Height,
    string PreviousHash,
    string MerkleRoot,
    long Nonce,
    int Difficulty,
    DateTime TimestampUtc,
    string MinerAddress);
