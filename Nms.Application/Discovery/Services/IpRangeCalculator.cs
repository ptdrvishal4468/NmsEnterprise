using System.Net;

namespace Nms.Application.Discovery.Services;

public static class IpRangeCalculator
{
    public const int MaxAllowedTargets = 1024;

    public static IReadOnlyList<string> CalculateIpAddresses(string ipRangeInput)
    {
        if (string.IsNullOrWhiteSpace(ipRangeInput))
            throw new ArgumentException("IP range input cannot be empty.", nameof(ipRangeInput));

        var input = ipRangeInput.Trim();
        var results = new List<string>();

        if (input.Contains('/'))
        {
            results.AddRange(ParseCidr(input));
        }
        else if (input.Contains('-'))
        {
            results.AddRange(ParseRange(input));
        }
        else if (IPAddress.TryParse(input, out var ip))
        {
            results.Add(ip.ToString());
        }
        else
        {
            throw new ArgumentException($"Invalid IP range specification: '{ipRangeInput}'");
        }

        if (results.Count > MaxAllowedTargets)
        {
            throw new InvalidOperationException($"Requested IP scan target count ({results.Count}) exceeds maximum allowed limit of {MaxAllowedTargets}.");
        }

        return results.Distinct().ToList();
    }

    private static List<string> ParseCidr(string cidr)
    {
        var parts = cidr.Split('/');
        if (parts.Length != 2 || !IPAddress.TryParse(parts[0], out var baseIp) || !int.TryParse(parts[1], out var prefixLength))
            throw new ArgumentException($"Invalid CIDR format: '{cidr}'");

        if (prefixLength < 16 || prefixLength > 32)
            throw new ArgumentException($"CIDR prefix /{prefixLength} is outside supported scanning range (/16 to /32).");

        var ipBytes = baseIp.GetAddressBytes();
        if (ipBytes.Length != 4)
            throw new ArgumentException("Only IPv4 addresses are currently supported for range scans.");

        uint ipUint = BitConverter.ToUInt32(ipBytes.Reverse().ToArray(), 0);
        uint mask = prefixLength == 0 ? 0 : 0xFFFFFFFF << (32 - prefixLength);

        uint networkUint = ipUint & mask;
        uint broadcastUint = networkUint | ~mask;

        var list = new List<string>();
        uint start = (prefixLength >= 31) ? networkUint : networkUint + 1;
        uint end = (prefixLength >= 31) ? broadcastUint : broadcastUint - 1;

        for (uint current = start; current <= end; current++)
        {
            var bytes = BitConverter.GetBytes(current).Reverse().ToArray();
            list.Add(new IPAddress(bytes).ToString());

            if (list.Count > MaxAllowedTargets) break;
        }

        return list;
    }

    private static List<string> ParseRange(string rangeStr)
    {
        var parts = rangeStr.Split('-');
        if (parts.Length != 2)
            throw new ArgumentException($"Invalid range format: '{rangeStr}'");

        if (!IPAddress.TryParse(parts[0].Trim(), out var startIp))
            throw new ArgumentException($"Invalid start IP address: '{parts[0]}'");

        var startBytes = startIp.GetAddressBytes();
        if (startBytes.Length != 4)
            throw new ArgumentException("Only IPv4 addresses are supported.");

        uint startUint = BitConverter.ToUInt32(startBytes.Reverse().ToArray(), 0);
        uint endUint;

        if (IPAddress.TryParse(parts[1].Trim(), out var endIp))
        {
            var endBytes = endIp.GetAddressBytes();
            endUint = BitConverter.ToUInt32(endBytes.Reverse().ToArray(), 0);
        }
        else if (byte.TryParse(parts[1].Trim(), out var endOctet))
        {
            var endBytes = (byte[])startBytes.Clone();
            endBytes[3] = endOctet;
            endUint = BitConverter.ToUInt32(endBytes.Reverse().ToArray(), 0);
        }
        else
        {
            throw new ArgumentException($"Invalid end specification in range: '{parts[1]}'");
        }

        if (startUint > endUint)
            throw new ArgumentException("Start IP address must be less than or equal to end IP address.");

        var list = new List<string>();
        for (uint current = startUint; current <= endUint; current++)
        {
            var bytes = BitConverter.GetBytes(current).Reverse().ToArray();
            list.Add(new IPAddress(bytes).ToString());

            if (list.Count > MaxAllowedTargets) break;
        }

        return list;
    }
}