using System;
using System.Threading.Tasks;
using Installer.Core.FileSystem;
using ManagedWimLib;

namespace Installer.Core.FullFx
{
    public class WimlibImageService : ImageServiceBase
    {
        public override async Task ApplyImage(Volume volume, string imagePath, int imageIndex = 1, IObserver<double> progressObserver = null)
        {
            EnsureValidParameters(volume, imagePath, imageIndex);

            await Task.Run(() =>
            {
                using (var wim = Wim.OpenWim(imagePath, OpenFlags.DEFAULT, (msg, info, callback) => UpdatedStatusCallback(msg, info, callback, progressObserver)))
                {
                    wim.ExtractImage(imageIndex, volume.RootDir.Name, ExtractFlags.DEFAULT);
                }
            });
        }

        private static CallbackStatus UpdatedStatusCallback(ProgressMsg msg, object info, object progctx,
            IObserver<double> progressObserver)
        {
            if (info is ProgressInfo_Extract m)
            {
                ulong percentComplete = 0;

                switch (msg)
                {
                    case ProgressMsg.EXTRACT_FILE_STRUCTURE:
                        
                        if (m.EndFileCount > 0)
                        {
                            percentComplete = m.CurrentFileCount * 10 / m.EndFileCount;
                        }

                        break;
                    case ProgressMsg.EXTRACT_STREAMS:
                        
                        if (m.TotalBytes > 0)
                        {
                            percentComplete = 10 + m.CompletedBytes * 80 / m.TotalBytes;
                        }

                        break;
                    case ProgressMsg.EXTRACT_METADATA:
                        
                        if (m.EndFileCount > 0)
                        {
                            percentComplete = 90 + m.CurrentFileCount * 10 / m.EndFileCount;
                        }

                        break;
                }

                progressObserver.OnNext((double)percentComplete / 100);
            }

            return CallbackStatus.CONTINUE;
        }
    }
}
