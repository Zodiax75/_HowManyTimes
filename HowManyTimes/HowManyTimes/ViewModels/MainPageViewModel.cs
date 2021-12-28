using HowManyTimes.Models;
using HowManyTimes.Services;
using HowManyTimes.Shared;
using HowManyTimes.Views;
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
            NewCategoryCommand = new Command(OnNewCategoryCommandClicked);
            NewCounterCommand = new Command(OnNewCounterCommandClicked);
            ShowAllCommand = new Command(OnShowAllCommandClicked);

            // Laod categiries at the beginning
            GetCategories();

            // subscribe to Category collection changes
            MessagingCenter.Subscribe<Category>(this, "AddNew", (category) =>
            {
                // add only if the category is favorite (main page shows only favorite categories ;)
                if (category.Favorite)
                    FavoriteCategories.Add(category);
            });

            MessagingCenter.Subscribe<Category>(this, "Update", (category) =>
            {
                // lookup the category and update it
                Category c = FavoriteCategories.Where(x => x.Id == category.Id).FirstOrDefault();
                if (c != null)
                {
                    FavoriteCategories[FavoriteCategories.IndexOf(c)] = category;
                }
            });

            MessagingCenter.Subscribe<Category>(this, "Delete", (category) =>
            {
                Category c = FavoriteCategories.Where(i => i.Id == category.Id).FirstOrDefault();
                var a = FavoriteCategories.Remove(c);
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
        /// Command to handle Show all categories click
        /// </summary>
        public ICommand ShowAllCommand { get; set; }
        /// <summary>
        /// Command to handle New counter click
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
