using Orion.Services;
using Orion.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Orion.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetupPage
    {

        private readonly SetupViewModel _viewModel;

        public SetupPage(INetworkService networkService)
        {
            BindingContext = _viewModel = new SetupViewModel(networkService);

            InitializeComponent();
        }

        private void SetupUserClicked(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                if (_viewModel.User.Name == null) return;

                _viewModel.User.Id = new Guid();
                var result = await _viewModel.DataStore.SetUser(_viewModel.User);

                if (!result) return;

                Device.BeginInvokeOnMainThread(NavigateToMain);
            });
        }



        private async void NavigateToMain()
        {
            Navigation.InsertPageBefore(new MainPage(), Navigation.NavigationStack[0]);
            await Navigation.PopToRootAsync();
        }
    }
}