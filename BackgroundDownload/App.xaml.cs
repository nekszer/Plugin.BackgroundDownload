using LightForms;
using LightForms.Services;
using BackgroundDownload.ViewModels;
using BackgroundDownload.Views;

namespace BackgroundDownload
{
    public partial class App : LightFormsApplication
    {

        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
            InitializeComponent();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Initialize<MainPage, MainViewModel>("/main");
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnResume()
        {
            base.OnSleep();
        }

        protected override void Routes(IRoutingService routingservice)
        {
            // set routes 
            // routingservice.Route<View,ViewModel>("/routename");
        }
    }
}
