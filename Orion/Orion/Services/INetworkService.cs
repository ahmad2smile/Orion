namespace Orion.Services
{
    public interface INetworkService
    {
        event NetworkService.FoundNode FoundNodeEvent;
        void StartNetwork();
        void BroadcastSelfNode();
    }
}
