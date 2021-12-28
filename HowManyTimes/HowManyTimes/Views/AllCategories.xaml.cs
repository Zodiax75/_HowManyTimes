using HowManyTimes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowManyTimes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllCategories : ContentPage
    {
        #region Constructor
        public AllCategories()
        {
            InitializeComponent();

            BindingContext = new AllCategoriesViewModel();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Handles click on search button to show/hide searchbar or title
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void searchButton_Clicked(object sender, EventArgs e)
        {
            if (titleLabel.IsVisible)
            {
                await titleLabel.FadeTo(0, 250);
                titleLabel.IsVisible = !titleLabel.IsVisible;
                await seachBar.FadeTo(1, 100);
                seachBar.IsVisible = !seachBar.IsVisible;
            }
            else
            {
                await seachBar.FadeTo(0, 250);
                seachBar.IsVisible = !seachBar.IsVisible;
                await titleLabel.FadeTo(1, 100);
                titleLabel.IsVisible = !titleLabel.IsVisible;
            }
        }

        /// <summary>
        /// Handles event for unfocusing search bar to hide it and show label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void seachBar_Unfocused(object sender, FocusEventArgs e)
        {
            if(seachBar.IsVisible)
            {
                await seachBar.FadeTo(0, 250);
                seachBar.IsVisible = !seachBar.IsVisible;
                await titleLabel.FadeTo(1, 100);
                titleLabel.IsVisible = !titleLabel.IsVisible;
            }
        }
        #endregion

        #region Properties

        #endregion

        #region Private properties

        #endregion


    }
}