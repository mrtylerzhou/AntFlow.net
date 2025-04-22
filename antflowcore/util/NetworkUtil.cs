using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace AntFlowCore.Util
{
    public static class NetworkUtil
    {
        /// <summary>
        /// 获取本地网络 IP 地址列表（仅 IPv4）。
        /// </summary>
        /// <returns>包含本地网络 IPv4 地址的列表</returns>
        public static List<string> GetNetworkIPList()
        {
            var ipList = new List<string>();

            try
            {
                // 获取所有网络接口
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var networkInterface in networkInterfaces)
                {
                    // 检查网络接口的状态
                    if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                        (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                         networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    {
                        // 获取网络接口的 IP 配置
                        var ipProperties = networkInterface.GetIPProperties();

                        foreach (var ipAddressInfo in ipProperties.UnicastAddresses)
                        {
                            // 只添加 IPv4 地址
                            if (ipAddressInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                ipList.Add(ipAddressInfo.Address.ToString());
                            }
                        }
                    }
                }

                // 如果未找到有效 IP 地址，则添加回环地址
                if (ipList.Count == 0)
                {
                    ipList.Add("127.0.0.1");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error while retrieving network IPs: {ex.Message}");
                ipList.Add("127.0.0.1"); // 添加回环地址作为默认值
            }

            return ipList;
        }

        /// <summary>
        /// 获取本机主机名。
        /// </summary>
        /// <returns>主机名</returns>
        public static string GetHostName()
        {
            try
            {
                return Dns.GetHostName();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error while retrieving host name: {ex.Message}");
                return string.Empty;
            }
        }
    }
}