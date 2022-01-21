using HowManyTimes.Models;
using HowManyTimes.Services;
using HowManyTimes.Shared;
using HowManyTimes.Views;
using HowManyTimes.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using System.Linq;
using Acr.UserDialogs;

namespace HowManyTimes.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region Constructor
        public MainPageViewModel()
        {
            // Bind commands
            NewCounterCommand = new Command(OnNewCounterCommandClicked);
            ShowAllCommand = new Command(OnShowAllCommandClicked);

            // Laod categiries at the beginning
            GetCategories();

            // subscribe to Category collection changes
            MessagingCenter.Subscribe<Category>(this, "AddNew", (category) =>
            {
                // add only if the category is favorite (main page shows only favorite categories ;)
                if (category.Favorite)
                    CategoryBaseViewModel.AddCategoryItem(FavoriteCategories, category);
            });

            MessagingCenter.Subscribe<Category>(this, "Update", (category) =>
            {
                CategoryBaseViewModel.UpdateCategoryItem(FavoriteCategories,category);
            });

            MessagingCenter.Subscribe<Category>(this, "UpdateFav", (category) =>
            {
                var tempc = CategoryBaseViewModel.LookupCategory(FavoriteCategories, category);

                if (tempc != null)
                {
                    // category already exists
                    if(!category.Favorite)
                        // favorite flag is false (not favorite) = delete from favorite collection
                        CategoryBaseViewModel.DeleteCategoryItem(FavoriteCategories, category);
                        // if favorite flag is true and the category is in the favorite collection, no action needed
                }
                else
                {
                    // category is not in favorites
                    if (category.Favorite)
                        // it is favorite category but is not in collection
                        CategoryBaseViewModel.AddCategoryItem(FavoriteCategories, category);
                        // if not favorite, no action needed since it is not in the collection
                }
            });

            MessagingCenter.Subscribe<Category>(this, "Delete", (category) =>
            {
                CategoryBaseViewModel.DeleteCategoryItem(FavoriteCategories, category);
            });
        }

        ~MainPageViewModel()
        {
            // unsubscribe event handlers
            MessagingCenter.Unsubscribe<Category>(this, "AddNew");
            MessagingCenter.Unsubscribe<Category>(this, "Update");
            MessagingCenter.Unsubscribe<Category>(this, "Delete");
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
                tmpList = await DBService.GetCategory(true, 10).ConfigureAwait(true);
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
        /// On Show all categories label click
        /// </summary>
        private void OnShowAllCommandClicked(object s)
        {
            switch(s.ToString())
            {
                case "Category":
                    Application.Current.MainPage.Navigation.PushAsync(new AllCategories(), true);
                    break;
                case "Counter":
                    throw new NotImplementedException();
                default:
                    LogService.Log(LogType.Error, "Show all pointing to unknown direction (neither category nor counter!");
                    break;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// List of favourite categories (limited to 10)
        /// </summary>
        public ObservableCollection<Category> FavoriteCategories { get; set; }

        /// <summary>
        /// Command to handle Show all categories click
        /// </summary>
        public ICommand ShowAllCommand { get; set; }
        /// <summary>
        /// Command to handle New counter click
        /// </summary>
        public ICommand NewCounterCommand { get; set; }

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
        #endregion
    }
}
