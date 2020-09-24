using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Plugin.Download
{
    /// <summary>
    /// Interface for Download
    /// </summary>
    public class DownloadImplementation : IDownload
    {
        
        private const string UrlKey = "url";
        private static Activity Activity { get; set; }
        private string Url { get; set; }
        internal static TaskCompletionSource<byte[]> StreamTaskCompletionSource { get; set; }
        internal static Action<DownloadBytesProgress> Action { get; set; }

        public IDownload SetUrl(string url)
        {
            Url = url;
            return this;
        }

        public IDownload SetListener(Action<DownloadBytesProgress> action)
        {
            Action = action;
            return this;
        }

        public static void Init(Activity activity)
        {
            Activity = activity;
        }

        public Task<byte[]> Start()
        {
            LocalNotification.ShowNotification(Activity, 0, false);
            StreamTaskCompletionSource = new TaskCompletionSource<byte[]>();
            var intent = new Intent(Activity, typeof(DownloaderService));
            intent.PutExtra(UrlKey, Url);
            Activity.StartService(intent);
            return StreamTaskCompletionSource.Task;
        }

        internal class LocalNotification
        {
            private const int NotificationId = 231;

            public static void ShowNotification(Context context, int progress, bool isfinished)
            {
                var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var channelNameJava = new Java.Lang.String("xamarindownloadchannel");
                    var channel = new NotificationChannel("defaultxamarindownloadchannel", channelNameJava, NotificationImportance.High)
                    {
                        Description = "xamarindownloadchannel"
                    };
                    notificationManager.CreateNotificationChannel(channel);
                }
                var notificationBuilder = new NotificationCompat.Builder(context, "defaultxamarindownloadchannel")
                    .SetSmallIcon(isfinished ? Android.Resource.Drawable.StatSysDownloadDone : Android.Resource.Drawable.StatSysDownload)
                    .SetAutoCancel(isfinished);

                if (!isfinished)
                    notificationBuilder.SetProgress(100, progress, false);
                else
                    notificationBuilder.SetProgress(0, progress, false);

                var notification = notificationBuilder.Build();
                notificationManager.Notify(NotificationId, notification);
            }
        }

        [Service]
        public class DownloaderService : Service
        {
            public override IBinder OnBind(Intent intent)
            {
                return null;
            }

            public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
            {
                var url = intent.GetStringExtra(UrlKey);
                var basecontext = BaseContext;

                Task.Run(async () => {
                    if (StreamTaskCompletionSource == null) return;
                    int receivedBytes = 0;
                    WebClient client = new WebClient();
                    var streamforwrite = new MemoryStream();
                    using var stream = await client.OpenReadTaskAsync(url);
                    int.TryParse(client.ResponseHeaders[HttpResponseHeader.ContentLength], out int totalBytes);
                    byte[] buffer = new byte[32768];
                    int read;
                    while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await streamforwrite.WriteAsync(buffer, 0, read);
                        receivedBytes += read;
                        DownloadBytesProgress args = new DownloadBytesProgress(receivedBytes, totalBytes);
                        Action?.Invoke(args);
                        try
                        {
                            LocalNotification.ShowNotification(basecontext, (int)(args.PercentComplete * 100), false);
                        }
                        catch { }
                    }
                    StreamTaskCompletionSource.TrySetResult(streamforwrite.ToArray());
                    LocalNotification.ShowNotification(basecontext, 0, false);
                    try { streamforwrite.Flush(); } catch { }
                    try { streamforwrite.Dispose(); } catch { }
                });
                return StartCommandResult.Sticky;
            }
        }
    }
}
