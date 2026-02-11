namespace Blockchain.Core.Validation;

public enum ValidationErrorCode
{
    Unknown = 0,
    InvalidAmount = 1,
    InvalidAddress = 2,
    SameSenderAndRecipient = 3,
    NonceMismatch = 4,
    InsufficientBalance = 5,
    InvalidSignature = 6,
    InvalidBlockHash = 7,
    InvalidDifficulty = 8,
    InvalidPreviousHash = 9,
    InvalidBlockHeight = 10,
    MissingCoinbase = 11,
    MultipleCoinbase = 12,
    InvalidMerkleRoot = 13,
    InvalidBlockTransactions = 14
}
