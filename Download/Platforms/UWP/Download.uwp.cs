using System;
using System.IO;
using System.Threading.Tasks;

namespace Plugin.Download
{
    /// <summary>
    /// Interface for Download
    /// </summary>
    public class DownloadImplementation : IDownload
    {

        public IDownload SetListener(Action<DownloadBytesProgress> action)
        {
            return this;
        }

        public IDownload SetUrl(string url)
        {
            return this;
        }

        public Task<byte[]> Start()
        {
            throw new Exception();
        }

    }
}
