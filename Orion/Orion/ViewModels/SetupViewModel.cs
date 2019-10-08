using Orion.Domain;
using Orion.Services;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Orion.ViewModels
{
    public class SetupViewModel : BaseViewModel
    {
        public ObservableCollection<Node> Nodes { get; set; } = new ObservableCollection<Node>();


        public SetupViewModel(INetworkService networkService)
        {
            networkService.FoundNodeEvent += OnFoundNode;
        }

        private void OnFoundNode(Node node)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Nodes.Add(node);
            });
        }
    }
}
