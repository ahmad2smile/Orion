using Makaretu.Dns;
using Orion.Models;
using System;
using System.Linq;

namespace Orion.Services
{
    public class NetworkService : INetworkService
    {
        private MulticastService MulticastService { get; set; } = new MulticastService();

        public delegate void FoundNode(Item newItem);

        public event FoundNode FoundNodeEvent;

        public NetworkService()
        {

            var service = new ServiceProfile("x", "example.local", 1024);
            var serviceDiscovery = new ServiceDiscovery(MulticastService);
            serviceDiscovery.Advertise(service);

            MulticastService.NetworkInterfaceDiscovered += (sender, args) =>
            {
                foreach (var nic in args.NetworkInterfaces)
                {
                    Console.WriteLine($"NIC '{nic.Name}'");
                }

                serviceDiscovery.QueryAllServices();
            };

            MulticastService.QueryReceived += (sender, serviceName) =>
            {
                Console.WriteLine($"service '{serviceName}'");
                var question = serviceName.Message.Questions.First();
                var ip = serviceName.RemoteEndPoint.Address.ToString();

                var domain = string.Join(".", question.Name.Labels.ToArray());
                var newItem = new Item
                {
                    Id = ip,
                    Text = ip,
                    Description = domain
                };

                FoundNodeEvent?.Invoke(newItem);
            };

        }

        public void StartNetwork()
        {
            MulticastService.Start();
        }
    }
}
