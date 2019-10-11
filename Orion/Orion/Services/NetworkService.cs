using Makaretu.Dns;
using Orion.Domain;
using System;
using System.Linq;

namespace Orion.Services
{
    public class NetworkService : INetworkService
    {
        private const string OrionDomain = "orion.p2p.app.local";

        private MulticastService MulticastService { get; set; } = new MulticastService();

        public delegate void FoundNode(Node node);

        public event FoundNode FoundNodeEvent;

        public void StartNetwork()
        {
            RegisterQueryListener();

            MulticastService.Start();
        }

        private void RegisterQueryListener()
        {
            MulticastService.QueryReceived += (sender, eventArgs) =>
            {
                var question = eventArgs.Message.Questions.First();
                var domain = string.Join(".", question.Name.Labels.ToArray());

                if (domain != OrionDomain) return;

                var ip = eventArgs.RemoteEndPoint.Address.ToString();

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
