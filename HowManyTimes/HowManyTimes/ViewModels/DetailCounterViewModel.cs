
using Acr.UserDialogs;
using HowManyTimes.Models;
using HowManyTimes.Services;
using HowManyTimes.Shared;
using HowManyTimes.Validators;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace HowManyTimes.ViewModels
{
    internal class DetailCounterViewModel : CounterBaseViewModel
    {
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public DetailCounterViewModel()
        {
            // bind commands to buttons
            SaveButtonCommand = new Command(OnSaveButtonCommandClicked);
            CancelButtonCommand = new Command(OnCancelButtonCommandClicked);
            ResetCounterCommand = new Command(OnResetCounterCommandClicked);
            IncreaseCounterCommand = new Command(OnIncreaseCounterCommandClicked);

            // get list of categories
            categories = DBService.GetCategory(false).Result;

            // set to not use steps at beggining
            UseSteps = false;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="counterId">Id of counter to edit</param>
        public DetailCounterViewModel(int? counterId = null) : this()
        {
            // load details for counter
            if (counterId != null)
            {
                LogService.Log(LogType.Info, $"Loading counter details for counter Id: {counterId}");

                try
                {
                    // load the specified counter
                    SelectedCounter = DBService.GetCounter(counterId.Value).Result;
                    LogService.Log(LogType.Info, $"Loaded details for counter {SelectedCounter.Id}: {SelectedCounter.Name}");

                    SelectedCategory = SelectedCounter.CounterCategory;
                    EditMode = false;
                }
                catch (Exception e)
                {
                    LogService.Log(LogType.Error, e.Message);
                }
            }
            else
            {
                // new counter
                SelectedCounter = new BaseCounter();
                LogService.Log(LogType.Info, "New counter details initialized");

                SelectedCategory = null;
                EditMode = true;
            }

            // store original value
            origCounter = BaseCounter.CopyCounter(SelectedCounter);

            CatFav = SelectedCounter.Favorite;
            CatPin = SelectedCounter.Pinned;
            Count = SelectedCounter.Counter;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when Reset counter button is clicked
        /// </summary>
        public void OnResetCounterCommandClicked()
        {
            ResetCounter(SelectedCounter);
            Count = SelectedCounter.Counter;
        }

        /// <summary>
        /// Called when Increase counter button is clicked
        /// </summary>
        public void OnIncreaseCounterCommandClicked()
        {
            SelectedCounter.IncreaseCounter();

            // save counter

            SaveCounterToDatabase(SelectedCounter);

            origCounter = BaseCounter.CopyCounter(SelectedCounter);
            Count = SelectedCounter.Counter;
        }

        /// <summary>
        /// Called when Cancel button is clicked
        /// </summary>
        public void OnCancelButtonCommandClicked()
        {
            if (SelectedCounter.Equals(origCounter))
                NavigateBack();

            // edit was cancelled, restore original data to bindings
            SelectedCounter = BaseCounter.CopyCounter(origCounter);

            EditMode = false;
        }

        /// <summary>
        /// Called when Save button is clicked
        /// </summary>
        public async void OnSaveButtonCommandClicked()
        {
            // New object should be created because changing just property doesnt fire OnPropertyChanged event!!!!!
            SelectedCounter.Favorite = CatFav;
            SelectedCounter.Pinned = CatPin;

            BaseCounter tmpC = BaseCounter.CopyCounter(SelectedCounter);

            // if category is selected, assign it
            if (SelectedCategory != null)
            {
                tmpC.CounterCategory = SelectedCategory;
            }

            // validate the input
            CounterDetailValidator counterValidator = new CounterDetailValidator(UseSteps);

            var validationResults = counterValidator.Validate(tmpC);
            if (!validationResults.IsValid)
            {
                ShowErrors(validationResults);
                return;
            }

            SelectedCounter = tmpC;

            SaveCounterToDatabase(SelectedCounter);

            // store new original baseline for cancellation
            origCounter = BaseCounter.CopyCounter(SelectedCounter);

            EditMode = false;
        }

        /// <summary>
        /// Inserts or updates counter in database
        /// </summary>
        /// <param name="cnt"></param>
        public async void SaveCounterToDatabase(BaseCounter cnt)
        {
            try
            {
                // insert or update decision
                if (cnt.Id == 0)
                {
                    // insert
                    LogService.Log(LogType.Info, $"Inserting new counter {cnt.Name}");

                    await DBService.InsertData(cnt);

                    // update category counter if needed
                    if (cnt.CounterCategory != null)
                    {
                        cnt.CounterCategory.Counters++;

                        LogService.Log(LogType.Info, $"Updating total counters counter ({cnt.CounterCategory.Counters}) for category {cnt.CounterCategory.Name}");
                        await DBService.UpdateData(cnt.CounterCategory);

                        MessagingCenter.Send<Category>(cnt.CounterCategory, "Update");
                    }

                    UserDialogs.Instance.Toast($"Counter {cnt.Name} successfully created.");
                    // notify mainpage that counter has been added
                    MessagingCenter.Send<BaseCounter>(cnt, "AddNewCounter");
                }
                else
                {
                    // update
                    LogService.Log(LogType.Info, $"Updating counter {cnt.Id}: {cnt.Name}");

                    await DBService.UpdateData(cnt);

                    UserDialogs.Instance.Toast($"Counter {cnt.Name} successfully updated.");

                    // notify mainpage that category has been edited
                    MessagingCenter.Send<BaseCounter>(cnt, "UpdateCounterFav");
                }
            }
            catch (Exception e)
            {
                LogService.Log(LogType.Error, e.Message);
            }
        }

        /// <summary>
        /// Called when Favorite button is clicked
        /// </summary>
        public override void OnFavoriteButtonCommandClicked()
        {
            // allow change only in edit mode
            if (EditMode)
            {
                // change Favorite and save it to current category
                SelectedCounter.Favorite = !SelectedCounter.Favorite;
                CatFav = !CatFav;
                
                LogService.Log(LogType.Info, $"Updating favorite for counter {SelectedCounter.Name} to {SelectedCounter.Favorite}");
            }
        }

        /// <summary>
        /// Called when Pinned button is clicked
        /// </summary>
        public override void OnPinnedButtonCommandClicked()
        {
            // allow change only in edit mode
            if (EditMode)
            {
                // change Favorite and save it to current category
                SelectedCounter.Pinned= !SelectedCounter.Pinned;
                CatPin = !CatPin;

                LogService.Log(LogType.Info, $"Updating pinned for counter {SelectedCounter.Name} to {SelectedCounter.Pinned}");
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Counter favorite Y/N
        /// </summary>
        public bool CounterFavorite { get; set; }
        /// <summary>
        /// List of categories for dropdown selection
        /// </summary>
        public List<Category> CategoriesList
        {
            get { return categories; }
        }

        public ICommand SaveButtonCommand { get; set; }
        public ICommand CancelButtonCommand { get; set; }
        public ICommand UploadPhotoCommand { get; set; }
        public ICommand CameraPhotoCommand { get; set; }
        public ICommand EditButtonCommand { get; set; }

        /// <summary>
        /// Return true if we are in edit mode (new or existing) and not view mode
        /// </summary>
        public bool EditMode { get; set; }
        /// <summary>
        /// Selected counter
        /// </summary>
        public BaseCounter SelectedCounter { get; set; }

        /// <summary>
        /// Category selected in dropdown
        /// </summary>
        public Category SelectedCategory { get; set; }

        /// <summary>
        /// Is usage of steps selected
        /// </summary>
        public bool UseSteps { get; set; }

        public bool CatFav { get; set; }
        public bool CatPin { get; set; }
        public int Count { get; set; }
        #endregion

        #region Private properties
        private BaseCounter origCounter;
        private List<Category> categories;
        #endregion
    }
}
