using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace AntFlow.Core.Util;

public static class NetworkUtil
{
    /// <summary>
    ///     ??????????? IP ????��???? IPv4????
    /// </summary>
    /// <returns>???????????? IPv4 ??????��?</returns>
    public static List<string> GetNetworkIPList()
    {
        List<string>? ipList = new();

        try
        {
            // ?????????????
            NetworkInterface[]? networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface? networkInterface in networkInterfaces)
            {
                // ????????????
                if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                    (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                     networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                {
                    // ?????????? IP ????
                    IPInterfaceProperties? ipProperties = networkInterface.GetIPProperties();

                    foreach (UnicastIPAddressInformation? ipAddressInfo in ipProperties.UnicastAddresses)
                    {
                        // ???? IPv4 ???
                        if (ipAddressInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipList.Add(ipAddressInfo.Address.ToString());
                        }
                    }
                }
            }

            // ???��?????�� IP ???????????????
            if (ipList.Count == 0)
            {
                ipList.Add("127.0.0.1");
            }
        }
        catch (System.Exception ex)
        {
            Console.Error.WriteLine($"Error while retrieving network IPs: {ex.Message}");
            ipList.Add("127.0.0.1"); // ???????????????
        }

        return ipList;
    }

    /// <summary>
    ///     ???????????????
    /// </summary>
    /// <returns>??????</returns>
    public static string GetHostName()
    {
        try
        {
            return Dns.GetHostName();
        }
        catch (System.Exception ex)
        {
            Console.Error.WriteLine($"Error while retrieving host name: {ex.Message}");
            return string.Empty;
        }
    }
}