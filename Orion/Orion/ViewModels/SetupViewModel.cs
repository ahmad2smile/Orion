using Orion.Services;
using Xamarin.Forms;

namespace Orion.ViewModels
{
    public class SetupViewModel : BaseViewModel
    {
        private string _nodeIp = "No IP";

        public string NodeIp { get => _nodeIp; set => SetProperty(ref _nodeIp, value, nameof(NodeIp)); }


        public SetupViewModel(INetworkService networkService)
        {
            networkService.FoundNodeEvent += OnFoundNode;
        }

        private void OnFoundNode(Models.Item newItem)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                NodeIp = newItem.Id;
            });
        }
    }
}
