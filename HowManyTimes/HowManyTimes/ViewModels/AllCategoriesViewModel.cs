using System;
using System.Collections.Generic;
using System.Text;
using HowManyTimes.Models;
using HowManyTimes.Services;
using HowManyTimes.Shared;
using HowManyTimes.Views;
using Acr.UserDialogs;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HowManyTimes.ViewModels
{
    public class AllCategoriesViewModel : CategoryBaseViewModel
    {
        #region Constructor
        public AllCategoriesViewModel()
        {
            DelCmd = new Command(OnDeleteButtonCommandClicked1);
            FavCmd = new Command(OnFavCmdClicked);
            // Load all categories
            GetCategories();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when Delete button is clicked
        /// </summary>
        public async void OnDeleteButtonCommandClicked1(object o)
        {
            Category cat = (Category)o;
            await UserDialogs.Instance.AlertAsync($"DEL: {cat.Name}", "Ok");
        }

        /// <summary>
        /// Called when Favorite button is clicked
        /// </summary>
        public async void OnFavCmdClicked(object o)
        {
            Category cat = (Category)o;
            await UserDialogs.Instance.AlertAsync($"FAV: {cat.Name}", "Ok");
        }

        private async void GetCategories()
        {
            List<Category> tmpList = new List<Category>();

            // Load all categories
            try
            {
                LogService.Log(LogType.Info, "Loading all categories for all categories page");
                tmpList = await DBService.GetCategory(false);
            }
            catch (Exception ex)
            {
                LogService.Log(LogType.Error, ex.Message);
            }

            // convert them into Observable collection before binding
            AllCategories = new ObservableCollection<Category>(tmpList);
        }
        #endregion

        #region Properties
        /// <summary>
        /// List of all categories
        /// </summary>
        public ObservableCollection<Category> AllCategories { get; set; }

        /// <summary>
        /// Selected category in the list
        /// </summary>
        public Category CategorySelected
        {
            get { return categorySelected; }
            set
            {
                categorySelected = value;

                if (categorySelected != null)
                {
                    int cId = categorySelected.Id;

                    // redirect to detail page passing id of the category to show
                    LogService.Log(LogType.Info, $"Redirecting to category {categorySelected.Id}: {categorySelected.Name}");

                    // null selected category to let it be selected again (due to single selection mode for collection view)
                    categorySelected = null;

                    Application.Current.MainPage.Navigation.PushAsync(new DetailCategory(cId), true);
                }
            }
        }
        #endregion

        #region Private properties
        private Category categorySelected = null;
        public Command DelCmd { get; set; }
        public Command FavCmd { get; set; }
        #endregion
    }
}
