using System;
using Xamarin.Forms;
using XenoGuardConnect.UtilClass;
using XenoGuardConnectMain.Interfaces;
using XenoGuardConnectMain.SubViews;

namespace XenoGuardConnectMain
{
    public partial class MainPage : ContentPage
    {

        private Connection socketConnection;

        public MainPage()
        {
            InitializeComponent();
        }
        public void OnLoginButtonClicked(object sender, EventArgs e)
        {
            DependencyService.Get<IToast>().Show("Toast Message");
        }

        #region Event Handlers
        /// <summary>
        /// Sends authentication to the TCP server.
        /// </summary>
        /// <param name="sender">The sender raising this event handler.</param>
        /// <param name="e">The example event arguments of this event handler.</param>
        public async void ButtonSendAuthentication(Object sender, EventArgs e)
        {
            try
            {
                this.socketConnection = Connection.GetInstance(Global.getIPAddress(), Global.getPort());
                this.socketConnection.ConnectToServer();
                if (this.socketConnection.isConnected())
                {
                    string [] response = new string[] { };
                    this.socketConnection.SendCommand(2021, "User", "XenoGuard", Global.getServerPassword(), "XENC_LOGIN", usernameEntry.Text + "|" + passwordEntry.Text);
                    response = this.socketConnection.ReadResponse();

                    if (response[0] == "ACKNOWLEDGE")
                    {
                        Global.setUsername(usernameEntry.Text);
                        Global.setPassword(passwordEntry.Text);
                        await Navigation.PushAsync(new MainMenuPage());
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show(response[0] + ":" + Global.getErrorResponse(response[1]));
                    }
                }
                else
                {
                    DependencyService.Get<IToast>().Show("ERROR: Server not reachable");
                }
            }
            catch (Exception x)
            {
                DependencyService.Get<IToast>().Show("ERROR: Unknown Error");
                Console.WriteLine("" + x);
            }
        }

        #endregion
    }
}
