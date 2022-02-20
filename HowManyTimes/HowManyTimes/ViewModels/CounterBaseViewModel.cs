﻿using Acr.UserDialogs;
using HowManyTimes.Models;
using HowManyTimes.Services;
using HowManyTimes.Shared;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace HowManyTimes.ViewModels
{
    internal class CounterBaseViewModel : BaseViewModel
    {
        #region Constructor
        public CounterBaseViewModel()
        {
            // binding commands
            FavoriteButtonCommand = new Command(OnFavoriteButtonCommandClicked);
            PinnedButtonCommand = new Command(OnPinnedButtonCommandClicked);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds new item to the counter collection
        /// </summary>
        /// <param name="col">Colection</param>
        /// <param name="c">counter item</param>
        public static void AddCounterItem(ObservableCollection<BaseCounter> col, BaseCounter c)
        {
            col.Add(c);
        }

        /// <summary>
        /// Updates item in the counter collection
        /// </summary>
        /// <param name="col">Colection</param>
        /// <param name="c">counter item</param>
        public static void UpdateCounterItem(ObservableCollection<BaseCounter> col, BaseCounter c)
        {
            BaseCounter bc = LookupCounter(col, c);

            var i = col.IndexOf(bc);

            if(bc != null && i>-1)
            {
                col[i] = BaseCounter.CopyCounter(c);
            }
        }

        /// <summary>
        /// Deletes counter from collection
        /// </summary>
        /// <param name="col">collection</param>
        /// <param name="c">counter</param>
        public static void DeleteCounterItem(ObservableCollection<BaseCounter> col, BaseCounter c)
        {
            BaseCounter tempc = CounterBaseViewModel.LookupCounter(col, c);

            if (tempc != null)
                _ = col.Remove(tempc);
        }

        public async void ResetCounter(BaseCounter cnt)
        {
            var result = await UserDialogs.Instance.ConfirmAsync($"Are you sure to reset counter {cnt.Name}?", "Confirm reset", "Yes", "No");

            if (!result)
                return; // reset was not confirmed

            LogService.Log(LogType.Info, $"Reseting counter {cnt.Id}: {cnt.Name} to 0");

            cnt.ResetCounter();
        }

        /// <summary>
        /// Deletes category from database (with validations)
        /// </summary>
        /// <param name="c">Category to be deleted</param>
        protected async Task<bool> DeleteCounter(BaseCounter SelectedCounter)
        {
            // delete from DB if existing category (with verification)
            var result = await UserDialogs.Instance.ConfirmAsync($"Are you sure to delete counter {SelectedCounter.Name}?", "Confirm delete", "Yes", "No");

            if (!result)
                return false; // detetion was not confirmed

            LogService.Log(LogType.Info, $"Deleting counter {SelectedCounter.Id}: {SelectedCounter.Name}");

            try
            {
                await DBService.DeleteData(SelectedCounter);

                // update category counter if part of the category
                if(SelectedCounter.CounterCategory != null)
                {
                    SelectedCounter.CounterCategory.Counters--;

                    _ = DBService.UpdateData(SelectedCounter.CounterCategory);
                    LogService.Log(LogType.Info, $"Updated number of counters in category {SelectedCounter.CounterCategory.Id}: {SelectedCounter.CounterCategory.Name} to {SelectedCounter.CounterCategory.Counters}");

                    MessagingCenter.Send<Category>(SelectedCounter.CounterCategory, "Update");
                }

                // notify mainpage that catogory has been deleted
                MessagingCenter.Send<BaseCounter>(SelectedCounter, "DeleteCounter");

                UserDialogs.Instance.Toast($"Counter {SelectedCounter.Name} successfully deleted.");

                return true;
            }
            catch (Exception e)
            {
                LogService.Log(LogType.Error, e.Message);
            }

            return false;
        }

        /// <summary>
        /// Lookups counter in collection
        /// </summary>
        /// <param name="col">collection</param>
        /// <param name="c">counter</param>
        /// <returns>null if not found</returns>
        public static BaseCounter LookupCounter(ObservableCollection<BaseCounter> col, BaseCounter c)
        {
            return (col.Where(x => x.Id == c.Id).FirstOrDefault());
        }

        /// <summary>
        /// Called when Favorite button is clicked
        /// </summary>
        public virtual async void OnFavoriteButtonCommandClicked()
        {
        }

        /// <summary>
        /// Called when Pinned button is clicked
        /// </summary>
        public virtual async void OnPinnedButtonCommandClicked()
        {
        }
        #endregion

        #region Properties
        public ICommand FavoriteButtonCommand { get; set; }
        public ICommand PinnedButtonCommand { get; set; }
        public ICommand DeleteButtonCommand { get; set; }
        public ICommand ResetCounterCommand { get; set; }
        public ICommand IncreaseCounterCommand { get; set; }
        #endregion

        #region Private properties

        #endregion
    }
}