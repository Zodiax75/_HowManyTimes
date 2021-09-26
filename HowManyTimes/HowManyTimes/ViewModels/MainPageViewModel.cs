using HowManyTimes.Models;
using HowManyTimes.Services;
using System.Collections.ObjectModel;


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
            // Load 10 favorite categories
            var tmpList = await DBService.GetCategory(true, 10);

            // convert them into Observable collection before binding
            FavoriteCategories = new ObservableCollection<Category>(tmpList);
        }
        #endregion

        #region Properties
        /// <summary>
        /// List of favourite categories (limited to 10)
        /// </summary>
        public ObservableCollection<Category> FavoriteCategories { get; set; }
        #endregion

        #region Private properties

        #endregion
    }
}
