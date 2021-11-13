using HowManyTimes.Models;
using HowManyTimes.Services;
using HowManyTimes.Shared;
using HowManyTimes.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace HowManyTimes.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region Constructor
        public MainPageViewModel()
        {
            // Bind commands
            NewCategoryCommand = new Command(OnNewCategoryCommandClicked);
            NewCounterCommand = new Command(OnNewCounterCommandClicked);

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
                LogService.Log(LogType.Info, "Loading favorite categories for main page");
                tmpList = await DBService.GetCategory(true, 10);
            }
            catch (Exception ex)
            {
                LogService.Log(LogType.Error, ex.Message);
            }

            // convert them into Observable collection before binding
            FavoriteCategories = new ObservableCollection<Category>(tmpList);
        }

        /// <summary>
        /// On New counter click function 
        /// </summary>
        private void OnNewCounterCommandClicked()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// On New category click function
        /// </summary>
        private void OnNewCategoryCommandClicked()
        {
            Application.Current.MainPage.Navigation.PushAsync(new DetailCategory(), true);
        }
        #endregion

        #region Properties
        /// <summary>
        /// List of favourite categories (limited to 10)
        /// </summary>
        public ObservableCollection<Category> FavoriteCategories { get; set; }

        /// <summary>
        /// Command to hand New counter click
        /// </summary>
        public ICommand NewCounterCommand { get; set; }

        /// <summary>
        /// Command to hand New category click
        /// </summary>
        public ICommand NewCategoryCommand { get; set; }

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
                    LogService.Log(LogType.Info, $"Redirecting to category {categorySelected.Id}: {categorySelected.Name}");
                    Application.Current.MainPage.Navigation.PushAsync(new DetailCategory(categorySelected.Id), true);
                }
            }
        }
        #endregion

        #region Private properties
        private Category categorySelected = null;
        #endregion
    }
}
