using Orion.Domain;
using Orion.Services;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Orion.ViewModels
{
    public class SetupViewModel : BaseViewModel
    {
        public User User
        {
            get => _user;
            set { _user = value; OnPropertyChanged(); }
        }
        private User _user;

        public ObservableCollection<Node> Nodes { get; } = new ObservableCollection<Node>();

        public SetupViewModel(INetworkService networkService)
        {
            User = new User();

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
