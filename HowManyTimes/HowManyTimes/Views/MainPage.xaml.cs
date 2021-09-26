using HowManyTimes.ViewModels;
using System;
using Xamarin.Forms;

namespace HowManyTimes.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }

        public async void OnNewCounterClicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new DetailPage(), false);
        }
    }
}
