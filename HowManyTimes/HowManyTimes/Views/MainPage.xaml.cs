using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using HowManyTimes.ViewModels;

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
