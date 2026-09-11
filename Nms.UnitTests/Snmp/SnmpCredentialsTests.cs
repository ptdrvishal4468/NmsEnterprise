using Nms.Domain.Enums;
using Nms.Domain.ValueObjects;
using Xunit;

namespace Nms.UnitTests.Snmp;

public class SnmpCredentialsTests
{
    [Fact]
    public void SnmpV2Credentials_ShouldThrow_WhenCommunityStringIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new SnmpV2Credentials(""));
    }

    [Fact]
    public void SnmpV3Credentials_ShouldCreateSuccessfully_WhenNoAuthNoPriv()
    {
        var creds = new SnmpV3Credentials("usr1", SnmpSecurityLevel.NoAuthNoPriv);
        Assert.Equal("usr1", creds.Username);
        Assert.Equal(SnmpSecurityLevel.NoAuthNoPriv, creds.SecurityLevel);
    }

    [Fact]
    public void SnmpV3Credentials_ShouldThrow_WhenAuthPrivMissingPasswords()
    {
        Assert.Throws<ArgumentException>(() => new SnmpV3Credentials(
            "usr1",
            SnmpSecurityLevel.AuthPriv,
            SnmpAuthProtocol.Sha256,
            null, // missing auth password
            SnmpPrivProtocol.Aes128,
            "privPass123"));
    }
}