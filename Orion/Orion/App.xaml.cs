using Autofac;
using Orion.Models;
using Orion.Services;
using Orion.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Orion
{
    public partial class App
    {
        private static IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        private readonly INetworkService _networkService;
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private static IContainer Container { get; set; }

        public App()
        {
            _networkService = new NetworkService();
            InitializeComponent();

            DependencyService.Register<DataStore>();

            MainPage = new NavigationPage(new SetupPage(_networkService));
        }

        protected override void OnStart()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(_networkService).As<INetworkService>().SingleInstance();

            _networkService.StartNetwork();

            Task.Run(async () =>
            {
                var user = await DataStore.GetUser();

                if (user.Name != null)
                {
                    MainPage = new NavigationPage(new MainPage());
                }
            });



            Container = builder.Build();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
