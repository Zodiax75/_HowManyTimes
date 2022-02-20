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
            ShowAllCommand = new Command(OnShowAllCommandClicked);

            // Laod categories at the beginning
            GetCategories();

            // Load initial counters
            GetCounters();

            // MESSAGE CENTER FOR CATEGORIES
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


            // MESSAGE CENTER FOR COUNTERS
            MessagingCenter.Subscribe<BaseCounter>(this, "AddNewCounter", (counter) =>
            {
                // add only if the counter is favorite (main page shows only favorite counters ;)
                if (counter.Favorite)
                    CounterBaseViewModel.AddCounterItem(FavoriteCounters, counter);

                SortFavoriteCounters();
            });

            MessagingCenter.Subscribe<BaseCounter>(this, "UpdateCounterFav", (counter) =>
            {
                BaseCounter bc = CounterBaseViewModel.LookupCounter(FavoriteCounters, counter);

                if(bc != null)
                {
                    // counter already exists
                    if (!counter.Favorite)
                        // favorite flag is false (not favorite) = delete from favorite collection
                        CounterBaseViewModel.DeleteCounterItem(FavoriteCounters, counter);
                    else
                        // if favorite flag is true and the counter is in the favorite collection, no action needed
                        CounterBaseViewModel.UpdateCounterItem(FavoriteCounters, counter);
                }
                else
                {
                    // counter is not in favorites
                    if (counter.Favorite)
                        // it is favorite counter but is not in collection
                        CounterBaseViewModel.AddCounterItem(FavoriteCounters, counter);
                    // if not favorite, no action needed since it is not in the collection
                }

                // add only if the counter is favorite (main page shows only favorite counters ;)
                if (counter.Favorite)
                {

                }

                SortFavoriteCounters();
            });

            MessagingCenter.Subscribe<BaseCounter>(this, "DeleteCounter", (counter) =>
            {
                CounterBaseViewModel.DeleteCounterItem(FavoriteCounters, counter);
            });
        }

        ~MainPageViewModel()
        {
            // unsubscribe event handlers
            MessagingCenter.Unsubscribe<Category>(this, "AddNew");
            MessagingCenter.Unsubscribe<Category>(this, "Update");
            MessagingCenter.Unsubscribe<Category>(this, "Delete");
            MessagingCenter.Unsubscribe<Category>(this, "UpdateFav");
            MessagingCenter.Unsubscribe<BaseCounter>(this, "AddNewCounter");
            MessagingCenter.Unsubscribe<BaseCounter>(this, "UpdateCounter");
            MessagingCenter.Unsubscribe<BaseCounter>(this, "DeleteCounter");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sorts favorite counters Pinned => Favorite => rest
        /// </summary>
        private void SortFavoriteCounters()
        {
            FavoriteCounters = new ObservableCollection<BaseCounter>(FavoriteCounters.OrderByDescending(x => x.Pinned).ThenByDescending(x => x.Favorite));
        }
        /// <summary>
        /// Loads counters for favorite counters on main page
        /// </summary>
        private async void GetCounters()
        {
            List<BaseCounter> lstCounters = new List<BaseCounter>();

            try
            {
                LogService.Log(LogType.Info, "Loading favorite counters for main page");
                lstCounters = await DBService.GetCounter(true, 10).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                LogService.Log(LogType.Error, ex.Message);
            }

            // convert them into Observable collection before binding
             FavoriteCounters = new ObservableCollection<BaseCounter>(lstCounters);
        }

        /// <summary>
        /// Loads categories for favorite categories on main page
        /// </summary>
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
        /// List of favourite counters (limited to 10)
        /// </summary>
        public ObservableCollection<BaseCounter> FavoriteCounters { get; set; }

        /// <summary>
        /// Command to handle Show all categories click
        /// </summary>
        public ICommand ShowAllCommand { get; set; }

        /// <summary>
        /// Selected counter in the list
        /// </summary>
        public BaseCounter CounterSelected
        {
            get { return counterSelected; }
            set
            {
                counterSelected = value;

                if (counterSelected != null)
                {
                    int cId = counterSelected.Id;

                    // redirect to detail page passing id of the counter to show
                    LogService.Log(LogType.Info, $"Redirecting to counter {counterSelected.Id}: {counterSelected.Name}");

                    // null selected counter to let it be selected again (due to single selection mode for collection view)
                    counterSelected = null;

                    Application.Current.MainPage.Navigation.PushAsync(new DetailCounter(cId), true);
                }
            }
        }

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
        private BaseCounter counterSelected = null;
        #endregion
    }
}
