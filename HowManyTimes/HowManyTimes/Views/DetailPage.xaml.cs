﻿using HowManyTimes.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowManyTimes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailPage : ContentPage
    {
        public DetailPage()
        {
            InitializeComponent();
            BindingContext = new DetailPageViewModel();
        }
    }
}