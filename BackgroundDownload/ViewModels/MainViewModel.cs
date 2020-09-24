using LightForms.Commands;
using LightForms.ViewModels;
using Plugin.Download;
using System.IO;
using System.Windows.Input;
using Xamarin.Essentials;

namespace BackgroundDownload.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        #region Notified Property Download
        /// <summary>
        /// Download
        /// </summary>
        private ICommand download;
        public ICommand Download
        {
            get { return download; }
            set { download = value; OnPropertyChanged(); }
        }
        #endregion

        #region Notified Property Image
        /// <summary>
        /// Image
        /// </summary>
        private Xamarin.Forms.ImageSource image;
        public Xamarin.Forms.ImageSource Image
        {
            get { return image; }
            set { image = value; OnPropertyChanged(); }
        }
        #endregion

        public override void Appearing(string route, object data)
        {
            base.Appearing(route, data);
            Download = new Command(Download_Command);
        }

        private async void Download_Command(object obj)
        {
            var bytes = await CrossDownload.Current.SetListener((progress) =>
            {
                System.Diagnostics.Debug.WriteLine(progress.PercentComplete);
            }).SetUrl("https://www.sample-videos.com/video123/mp4/720/big_buck_bunny_720p_10mb.mp4")
            .Start();
        }
    }
}