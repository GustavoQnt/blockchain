using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Blockchain.Core.Abstractions;
using Blockchain.Core.Models;

namespace Blockchain.Core.Services;

public sealed class Sha256HashService : IHashService
{
    public string ComputeSha256(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public string ComputeTransactionHash(Transaction transaction)
    {
        var payload = string.Create(
            CultureInfo.InvariantCulture,
            $"{transaction.Id}|{transaction.From}|{transaction.To}|{transaction.Amount}|{transaction.Nonce}|{transaction.PublicKeyHex}|{transaction.SignatureHex}|{transaction.TimestampUtc:O}");

        return ComputeSha256(payload);
    }

    public string ComputeMerkleRoot(IReadOnlyCollection<Transaction> transactions)
    {
        if (transactions.Count == 0)
        {
            return new string('0', 64);
        }

        var layer = transactions.Select(ComputeTransactionHash).ToList();
        while (layer.Count > 1)
        {
            if (layer.Count % 2 != 0)
            {
                layer.Add(layer[^1]);
            }

            var nextLayer = new List<string>(layer.Count / 2);
            for (var i = 0; i < layer.Count; i += 2)
            {
                nextLayer.Add(ComputeSha256(layer[i] + layer[i + 1]));
            }

            layer = nextLayer;
        }

        return layer[0];
    }

    public string ComputeBlockHash(BlockHeader header)
    {
        var payload = string.Create(
            CultureInfo.InvariantCulture,
            $"{header.Height}|{header.PreviousHash}|{header.MerkleRoot}|{header.Nonce}|{header.Difficulty}|{header.TimestampUtc:O}|{header.MinerAddress}");

        return ComputeSha256(payload);
    }
}
