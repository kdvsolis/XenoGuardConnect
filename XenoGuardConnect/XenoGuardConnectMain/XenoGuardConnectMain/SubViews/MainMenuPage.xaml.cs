using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XenoGuardConnectMain.SubViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenuPage : ContentPage
    {
        public MainMenuPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if (Navigation.NavigationStack.Count > 1 && Navigation.NavigationStack[Navigation.NavigationStack.Count - 2].GetType().ToString().EndsWith("MainPage"))
            {
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
            }
        }

        public async void ButtonNavigateToMessages(Object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MessagePage());
        }
        public async void ButtonNavigateToSettings(Object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }
}