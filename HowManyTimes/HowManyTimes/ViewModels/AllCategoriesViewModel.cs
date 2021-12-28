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

namespace HowManyTimes.ViewModels
{
    public class AllCategoriesViewModel : BaseViewModel
    {
        #region Constructor
        public AllCategoriesViewModel()
        {
            // Load all categories
            GetCategories();
        }
        #endregion

        #region Methods
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
        #endregion

        #region Private properties
        #endregion
    }
}
