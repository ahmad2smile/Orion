using Makaretu.Dns;
using Makaretu.Dns.Resolving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;
using Node = Orion.Domain.Node;

namespace Orion.Services
{
    public class NetworkService : INetworkService
    {
        private const string OrionDomain = "orion.p2p.app.local";
        private const int BroadcastingInterval = 2 * 1000;

        private const int MulticastPort = 5353;
        private static readonly IPAddress MulticastAddressIp4 = IPAddress.Parse("224.0.0.251");

        public delegate void FoundNode(Node node);

        public event FoundNode FoundNodeEvent;

        public event EventHandler<UdpReceiveResult> MessageReceived;

        private static readonly NameServer NameServer = new NameServer
        {
            Catalog = new Catalog()
        };

        public void StartNetwork()
        {
            RegisterQueryListener();

            var receiver = new UdpClient(AddressFamily.InterNetwork);

            receiver.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            receiver.Client.Bind(new IPEndPoint(IPAddress.Any, MulticastPort));

            var networkAddresses = GetNetworkInterfaceLocalAddresses();

            foreach (var ipAddress in networkAddresses)
            {
                receiver.Client.SetSocketOption(
                    SocketOptionLevel.IP,
                    SocketOptionName.AddMembership,
                    new MulticastOption(MulticastAddressIp4, ipAddress)
                );
            }

            ListenForDnsMessages(receiver);
        }

        public void BroadcastSelfNode()
        {
            var timer = new Timer { Interval = BroadcastingInterval };

            timer.Elapsed += async (sender, args) =>
            {
                var request = new Message();
                request.Questions.Add(new Question { Name = OrionDomain, Type = DnsType.AAAA });
                // In trying to resolve DNS record listening Node get the IP of Self Node
                var response = await NameServer.ResolveAsync(request);
            };

            timer.Start();
        }


        private static IEnumerable<NetworkInterface> GetNetworkInterfaces()
        {
            var allUpNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .ToArray();

            var nonLoopBackInterfaces = allUpNetworkInterfaces
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .ToArray();

            return nonLoopBackInterfaces.Length > 0 ? nonLoopBackInterfaces : allUpNetworkInterfaces;
        }

        private IEnumerable<IPAddress> GetNetworkInterfaceLocalAddresses()
        {
            return GetNetworkInterfaces().SelectMany(nic => nic
                    .GetIPProperties()
                    .UnicastAddresses
                    .Select(x => x.Address)
                    .Where(x => x.AddressFamily != AddressFamily.InterNetworkV6 || x.IsIPv6LinkLocal))
                .Where(a => a.AddressFamily == AddressFamily.InterNetwork);
        }

        private void ListenForDnsMessages(UdpClient receiver)
        {
            Task.Run(async () =>
            {
                var task = receiver.ReceiveAsync();

                await task.ContinueWith(x =>
                        ListenForDnsMessages(receiver),
                    TaskContinuationOptions.OnlyOnRanToCompletion |
                    TaskContinuationOptions.RunContinuationsAsynchronously
                );

                await task.ContinueWith(x =>
                        MessageReceived?.Invoke(this, x.Result),
                    TaskContinuationOptions.OnlyOnRanToCompletion |
                    TaskContinuationOptions.RunContinuationsAsynchronously
                );

                await task.ConfigureAwait(false);
            });
        }

        private void RegisterQueryListener()
        {
            MessageReceived += (sender, result) =>
            {
                var message = new Message();
                message.Read(result.Buffer, 0, result.Buffer.Length);
                if (message.Questions == null || message.Questions.Count == 0)
                {
                    return;
                }

                var question = message.Questions.First();

                var domain = string.Join(".", question.Name.Labels.ToArray());

                if (domain != OrionDomain) return;

                var ip = result.RemoteEndPoint.Address.ToString();

                var node = new Node
                {
                    Id = new Guid(),
                    Ip = ip,
                    Name = domain
                };

                FoundNodeEvent?.Invoke(node);
            };
        }
    }
}
