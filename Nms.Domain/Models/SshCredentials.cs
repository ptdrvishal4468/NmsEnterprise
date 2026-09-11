namespace Nms.Domain.Models;

public sealed record SshCredentials
{
    public string Username { get; }
    public string? Password { get; }
    public string? PrivateKey { get; }
    public string? Passphrase { get; }

    private SshCredentials(string username, string? password, string? privateKey, string? passphrase)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));

        Username = username;
        Password = password;
        PrivateKey = privateKey;
        Passphrase = passphrase;
    }

    public static SshCredentials FromPassword(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));

        return new SshCredentials(username, password, null, null);
    }

    public static SshCredentials FromPrivateKey(string username, string privateKey, string? passphrase = null)
    {
        if (string.IsNullOrWhiteSpace(privateKey))
            throw new ArgumentException("Private key cannot be null or empty.", nameof(privateKey));

        return new SshCredentials(username, null, privateKey, passphrase);
    }
}