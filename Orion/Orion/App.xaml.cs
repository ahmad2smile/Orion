using Autofac;
using Orion.Services;
using Orion.Views;
using Xamarin.Forms;

namespace Orion
{
    public partial class App
    {
        private readonly INetworkService _networkService;
        private bool IsAlreadySetup { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private static IContainer Container { get; set; }

        public App()
        {
            _networkService = new NetworkService();
            InitializeComponent();

            DependencyService.Register<MockDataStore>();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(_networkService).As<INetworkService>().SingleInstance();

            _networkService.FoundNodeEvent += newItem =>
            {
                IsAlreadySetup = false;
            };
            _networkService.StartNetwork();

            if (!IsAlreadySetup)
            {
                MainPage = new SetupPage(_networkService);
            }

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
