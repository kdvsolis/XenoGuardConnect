using Android.Widget;
using XenoGuardConnectMain.Droid.AndroidUtils;
using XenoGuardConnectMain.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(Toast_Android))]

namespace XenoGuardConnectMain.Droid.AndroidUtils
{
    public class Toast_Android : IToast
    {
        public void Show(string message)
        {
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
        }
    }
}