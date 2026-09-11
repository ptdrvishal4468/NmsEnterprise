namespace Nms.Domain.ValueObjects;

public sealed record SnmpV2Credentials
{
    public string CommunityString { get; }

    public SnmpV2Credentials(string communityString)
    {
        if (string.IsNullOrWhiteSpace(communityString))
        {
            throw new ArgumentException("Community string cannot be null or empty.", nameof(communityString));
        }

        CommunityString = communityString;
    }
}