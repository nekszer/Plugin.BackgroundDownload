namespace BackgroundDownload.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            LoadApplication(new BackgroundDownload.App());
        }
    }
}