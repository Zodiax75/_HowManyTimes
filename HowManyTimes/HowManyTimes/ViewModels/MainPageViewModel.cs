using HowManyTimes.Models;
using HowManyTimes.Services;
using HowManyTimes.Shared;
using HowManyTimes.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace HowManyTimes.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region Constructor
        public MainPageViewModel()
        {
            // Laod categiries at the beginning
            GetCategories();
        }
        #endregion

        #region Methods
        private async void GetCategories()
        {
            List<Category> tmpList = new List<Category>();

            // Load 10 favorite categories
            try
            {
                tmpList = await DBService.GetCategory(true, 10);
            }
            catch (Exception ex)
            {
                LogService.Log(LogType.Error, ex.Message);
            }

            // convert them into Observable collection before binding
            FavoriteCategories = new ObservableCollection<Category>(tmpList);
        }
        #endregion

        #region Properties
        /// <summary>
        /// List of favourite categories (limited to 10)
        /// </summary>
        public ObservableCollection<Category> FavoriteCategories { get; set; }

        /// <summary>
        /// Selected category in the list
        /// </summary>
        public Category CategorySelected
        {
            get { return (categorySelected); }
            set
            {
                categorySelected = value;

                if (categorySelected != null)
                {
                    // redirect to detail page passing id of the category to show
                    Application.Current.MainPage.Navigation.PushAsync(new DetailCategory(categorySelected.Id), false);
                }
            }
        }
        #endregion

        #region Private properties
        private Category categorySelected = null;
        #endregion
    }
}
