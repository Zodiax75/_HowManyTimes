using HowManyTimes.ViewModels;
using System;
using Xamarin.Forms;
using System.Collections.Generic;
using HowManyTimes.Models;
using HowManyTimes.Services;
using System.Linq;
using System.Threading.Tasks;

namespace HowManyTimes.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly double _scale; // save original scale of the button (applies for both, there are the same size)

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();

            mainHeader.BackgroundColor = Color.Transparent;

            _scale = newCounter.Scale;
        }

        /// <summary>
        /// Handler for PLUS button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnNewCounterClicked(object sender, EventArgs e)
        {
            // sub-buttons (new counter and new category) animation
            if (newCounter.IsVisible)
            {
                await newCategory.FadeTo(0, 250);
                newCategory.IsVisible = false;
                await newCounter.FadeTo(0, 250);
                newCounter.IsVisible = false;
            }
            else
            {
                newCounter.IsVisible = true;
                await newCounter.FadeTo(1, 100);
                await newCounter.ScaleTo(_scale * 1.5, 250);
                await newCounter.ScaleTo(_scale, 250);

                newCategory.IsVisible = true;
                await newCategory.FadeTo(1, 100);
                await newCategory.ScaleTo(_scale * 1.5, 250);
                await newCategory.ScaleTo(_scale, 250);
            }
        }

        public async void OnSwipped(object sender, EventArgs e)
        {
            // TODO: rework to use Command and bind it to ViewModel
            await Application.Current.MainPage.Navigation.PushAsync(new DetailPage(), true);
        }

        public async void OnSearchTapped(object sender, EventArgs e)
        {
            // hide if shown and show if hidden
            searchFrame.IsVisible = !searchFrame.IsVisible;

            // set icon color
            switch(searchFrame.IsVisible)
            {
                case true:
                    searchIcon.TextColor = Color.FromHex("#FF8920");
                    break;
                case false:
                    searchIcon.TextColor = Color.FromHex("#6F6F6F");
                    break;
            }
        }
    }
}
