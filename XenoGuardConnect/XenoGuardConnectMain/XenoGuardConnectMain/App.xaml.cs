using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DevExpress.XamarinForms.Core.Themes;
using XenoGuardConnect.UtilClass;
using XenoGuardConnectMain.Interfaces;
using XenoGuardConnectMain.SubViews;
using Xamarin.Forms.Background;
using XenoGuardConnectMain.UtilClass;

namespace XenoGuardConnectMain
{
    public partial class App : Application
    {
        public App()
        {
            DevExpress.XamarinForms.Editors.Initializer.Init();
            App.Current.UserAppTheme = OSAppTheme.Dark;
            ThemeManager.ThemeName = Theme.Dark;
            InitializeComponent();
            MainPage = new NavigationPage(new XenoGuardConnectMain.MainPage());
        }

        protected override void OnStart()
        {
            try
            {
                Global.SetNotificationClickEvent(NavigateToMessages);
                //Register Background Tasks
                BackgroundAggregatorService.Add(() => new NotificationBackgroundWork());
                //Start the background service
                BackgroundAggregatorService.StartBackgroundService();
            }
            catch (Exception)
            {
                Global.PoolMessageFromSocket();
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
        public void NavigateToMessages(object sender, EventArgs e)
        {
            var evtData = (NotificationEventArgs)e;
            Device.BeginInvokeOnMainThread(async () =>
            {
                await MainPage.Navigation.PushAsync(new MessagePage());
            });
        }
    }
}
