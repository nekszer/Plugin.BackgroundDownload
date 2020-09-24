using System;
using System.IO;
using System.Threading.Tasks;

namespace Plugin.Download
{
    public interface IDownload
    {

        IDownload SetUrl(string url);

        IDownload SetListener(Action<DownloadBytesProgress> action);

        Task<byte[]> Start();

    }
}
