using Orion.Services;
using Orion.ViewModels;
using Xamarin.Forms.Xaml;

namespace Orion.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetupPage
    {
        public SetupPage(INetworkService networkService)
        {
            BindingContext = new SetupViewModel(networkService);
            InitializeComponent();
        }
    }
}