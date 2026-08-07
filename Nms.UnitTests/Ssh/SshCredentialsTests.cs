using Nms.Domain.Models;

namespace Nms.UnitTests.Ssh;

public class SshCredentialsTests
{
    [Fact]
    public void FromPassword_ValidInput_CreatesCredentials()
    {
        var creds = SshCredentials.FromPassword("admin", "secret123");

        Assert.Equal("admin", creds.Username);
        Assert.Equal("secret123", creds.Password);
        Assert.Null(creds.PrivateKey);
    }

    [Fact]
    public void FromPrivateKey_ValidInput_CreatesCredentials()
    {
        var creds = SshCredentials.FromPrivateKey("admin", "---BEGIN KEY---", "passphrase");

        Assert.Equal("admin", creds.Username);
        Assert.Equal("---BEGIN KEY---", creds.PrivateKey);
        Assert.Equal("passphrase", creds.Passphrase);
        Assert.Null(creds.Password);
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData(null, "password")]
    public void FromPassword_InvalidUsername_ThrowsArgumentException(string? username, string password)
    {
        Assert.Throws<ArgumentException>(() => SshCredentials.FromPassword(username!, password));
    }
}