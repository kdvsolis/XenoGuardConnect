using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XenoGuardConnect.UtilClass;

namespace XenoGuardConnectMain.SubViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagePage : ContentPage
    {
        private Connection socketConnection;
        private ObservableCollection<Global.MessageStructure> messages = new ObservableCollection<Global.MessageStructure>() { };
        public MessagePage()
        {
            InitializeComponent();
            messageList.ItemsSource = messages;
            //var template = new DataTemplate(typeof(TextCell));
            //template.SetValue(TextCell.TextColorProperty, Color.White);
            //messageList.ItemTemplate = template;
            this.socketConnection = Connection.GetInstance(Global.getIPAddress(), Global.getPort());
            if (this.socketConnection.isConnected())
            {
                PoolMessage();
            }
        }

        public void ButtonDeleteSelected(Object sender, EventArgs e)
        {
            Global.removeSelectedMessages();
        }
        public async void ButtonNavigateBack(Object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        public void OnCheckedChanged(Object sender, CheckedChangedEventArgs e)
        {
            //Console.WriteLine(e.Value);
        }
        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            var itemDetails = (ListView)sender;
            var itemData = (Global.MessageStructure)itemDetails.SelectedItem;
            await Navigation.PushAsync(new MessageDetailPage(itemData.Time, itemData.Type, itemData.Message, itemData.id, itemData.Tag == "XENC_PUSH_MESSAGE"));
        }

        public async void PoolMessage()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    messageList.ItemsSource = Global.getMessages();
                }
            });
        }
    }
}