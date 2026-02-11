using Blockchain.Core.Models;
using Blockchain.Core.Services;
using FluentAssertions;

namespace Blockchain.Core.Tests.Hash;

public sealed class Sha256HashServiceTests
{
    private readonly Sha256HashService _sut = new();

    [Fact]
    public void ComputeSha256_ShouldReturnExpectedHash()
    {
        var hash = _sut.ComputeSha256("abc");

        hash.Should().Be("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad");
    }

    [Fact]
    public void ComputeMerkleRoot_WhenTransactionsAreEmpty_ShouldReturnZeroHash()
    {
        var merkleRoot = _sut.ComputeMerkleRoot(Array.Empty<Transaction>());

        merkleRoot.Should().Be(new string('0', 64));
    }
}
