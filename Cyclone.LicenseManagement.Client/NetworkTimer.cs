using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cyclone.LicenseManagement.Client;

public class NetworkTimer
{
    public static DateTime? GetTime()
    {
        // 腾讯提供的 NTP 服务器
        const string NtpServer = "ntp.tencent.com";

        // NTP 数据包大小
        const int NtpDataSize = 48;

        // 创建一个字节数组来存储 NTP 数据包
        byte[] ntpData = new byte[NtpDataSize];

        // 设置 NTP 数据包的第一个字节为 0x1B (请求字节)
        ntpData[0] = 0x1B;

        try
        {
            // 使用 UdpClient 发送和接收数据
            using var udpClient = new UdpClient();
            udpClient.Send(ntpData, ntpData.Length, NtpServer, 123);

            // 接收 NTP 响应
            var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] result = udpClient.Receive(ref remoteEndPoint);
            ntpData = result;

            // 转换接收到的字节数组为时间
            ulong intPart = BitConverter.ToUInt32(ntpData, 40);
            ulong fractPart = BitConverter.ToUInt32(ntpData, 44);

            // 转换网络字节序到主机字节序
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            // 减去 70 年的时间（NTP 起始时间为 1900 年，而 .NET 的起始时间为 1970 年）
            ulong milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000UL);
            DateTime networkDateTime = new DateTime(1900, 1, 1).AddMilliseconds((long)milliseconds).ToLocalTime();

            return networkDateTime;
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"SocketException: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return null;
        }
    }

    // 辅助方法：转换字节序
    private static uint SwapEndianness(ulong x)
    {
        return (uint)(((x & 0x000000ff) << 24) +
                      ((x & 0x0000ff00) << 8) +
                      ((x & 0x00ff0000) >> 8) +
                      ((x & 0xff000000) >> 24));
    }
}