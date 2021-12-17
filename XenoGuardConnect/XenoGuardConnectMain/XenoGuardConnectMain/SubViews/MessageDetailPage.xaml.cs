using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XenoGuardConnect.UtilClass;

namespace XenoGuardConnectMain.SubViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageDetailPage : ContentPage
    {
        int id = -1;
        private static Connection socketConnection;
        public MessageDetailPage(string _dateTime, string _type, string _message, int _id, bool isPush = true)
        {
            InitializeComponent();
            DateTime.Text = _dateTime;
            Type.Text = _type;
            Message.Text = _message;
            id = Global.getMessages().IndexOf(Global.getMessages().Where(x => x.id == _id).FirstOrDefault());
            socketConnection = Connection.GetInstance(Global.getIPAddress(), Global.getPort());

            if (isPush)
            {
                btnOK.IsVisible = false;
                btnCancel.IsVisible = false;
            }
            if (Global.getMessages()[id].isLocked)
            {
                btnOK.IsEnabled = false;
                btnCancel.IsEnabled = false;
            }
        }
        public async void ButtonSendYes(Object sender, EventArgs e)
        {
            socketConnection.SendCommand(2021, "User", "XenoGuard", Global.getServerPassword(), "XENC_REQUEST_MESSAGE_RESPONSE", Global.getMessages()[id].reqId + "|YES");
            Global.getMessages()[id].isLocked = true;
            await Navigation.PopAsync();
        }
        public async void ButtonSendNo(Object sender, EventArgs e)
        {
            socketConnection.SendCommand(2021, "User", "XenoGuard", Global.getServerPassword(), "XENC_REQUEST_MESSAGE_RESPONSE", Global.getMessages()[id].reqId + "|NO");
            await Navigation.PopAsync();
        }
        public async void ButtonNavigateBack(Object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}