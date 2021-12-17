using System.Threading.Tasks;
using Xamarin.Forms.Background;

namespace XenoGuardConnectMain.UtilClass
{
    class NotificationBackgroundWork : IBackgroundTask
    {
        public NotificationBackgroundWork()
        {
        }

        public async Task StartJob()
        {
            await Task.Run(() =>
            {
                XenoGuardConnect.UtilClass.Global.PoolMessageFromSocket();
            });
        }
    }
}
