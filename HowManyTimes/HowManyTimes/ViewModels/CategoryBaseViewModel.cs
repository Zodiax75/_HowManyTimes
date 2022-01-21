using Acr.UserDialogs;
using HowManyTimes.Models;
using HowManyTimes.Services;
using HowManyTimes.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace HowManyTimes.ViewModels
{
    public class CategoryBaseViewModel :  BaseViewModel
    {
        #region Constructor
        public CategoryBaseViewModel()
        {
            // binding commands
            FavoriteButtonCommand = new Command(OnFavoriteButtonCommandClicked);
            DeleteButtonCommand = new Command(OnDeleteButtonCommandClicked);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds new item to the category collection
        /// </summary>
        /// <param name="col">Colection</param>
        /// <param name="c">category item</param>
        public static void AddCategoryItem(ObservableCollection<Category> col, Category c)
        {
            col.Add(c);
        }

        /// <summary>
        /// Updates specific category in collection
        /// </summary>
        /// <param name="col">Collection</param>
        /// <param name="c">Category with updated values</param>
        public static void UpdateCategoryItem(ObservableCollection<Category> col, Category c)
        {
            // lookup the category and update it
            var tempc = CategoryBaseViewModel.LookupCategory(col, c);

            if (tempc != null)
            {
                col[col.IndexOf(tempc)] = c;
            }
        }

        /// <summary>
        /// Lookups category in collection
        /// </summary>
        /// <param name="col">collection</param>
        /// <param name="c">category</param>
        /// <returns>null if not found</returns>
        public static Category LookupCategory(ObservableCollection<Category> col, Category c)
        {
            return(col.Where(x => x.Id == c.Id).FirstOrDefault());
        }

        /// <summary>
        /// Deletes category from collection
        /// </summary>
        /// <param name="col">collection</param>
        /// <param name="c">category</param>
        public static void DeleteCategoryItem(ObservableCollection<Category> col, Category c)
        {
            Category tempc = CategoryBaseViewModel.LookupCategory(col, c);

            if(tempc != null)
                _ = col.Remove(tempc);
        }

        /// <summary>
        /// Called when Favorite button is clicked
        /// </summary>
        public virtual async void OnFavoriteButtonCommandClicked()
        {
        }

        /// <summary>
        /// Called when Delete button is clicked
        /// </summary>
        public virtual async void OnDeleteButtonCommandClicked()
        {
        }

        /// <summary>
        /// Deletes category from database (with validations)
        /// </summary>
        /// <param name="c">Category to be deleted</param>
        protected async Task<bool> DeleteCategory(Category SelectedCategory)
        {
            // check if there are some counters connected
            if (SelectedCategory.Counters > 0)
            {
                await UserDialogs.Instance.AlertAsync("To delete a category, you must not have any counters active.", "Active counters!", "Ok");
                return false;
            }

            // delete from DB if existing category (with verification)
            var result = await UserDialogs.Instance.ConfirmAsync($"Are you sure to delete category {SelectedCategory.Name}?", "Confirm delete", "Yes", "No");

            if (!result)
                return false; // detetion was not confirmed

            LogService.Log(LogType.Info, $"Deleting category {SelectedCategory.Id}: {SelectedCategory.Name}");

            try
            {
                await DBService.DeleteData(SelectedCategory);

                // notify mainpage that catogory has been deleted
                MessagingCenter.Send<Category>(SelectedCategory, "Delete");

                UserDialogs.Instance.Toast($"Category {SelectedCategory.Name} successfully deleted.");

                return true;
            }
            catch (Exception e)
            {
                LogService.Log(LogType.Error, e.Message);
            }

            return false;
        }
        #endregion

            #region Properties
        public ICommand FavoriteButtonCommand { get; set; }
        public ICommand DeleteButtonCommand { get; set; }
        #endregion

        #region Private properties
        #endregion
    }
}
