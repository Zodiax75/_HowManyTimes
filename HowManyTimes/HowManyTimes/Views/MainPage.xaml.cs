using HowManyTimes.ViewModels;
using System;
using Xamarin.Forms;
using System.Collections.Generic;
using HowManyTimes.Models;
using HowManyTimes.Services;
using System.Linq;

namespace HowManyTimes.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }

        /// <summary>
        /// Handler for PLUS button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnNewCounterClicked(object sender, EventArgs e)
        {
            // TODO: rework to use Command and bind it to ViewModel
            await Application.Current.MainPage.Navigation.PushAsync(new DetailPage(), false);
        }
    }
}
