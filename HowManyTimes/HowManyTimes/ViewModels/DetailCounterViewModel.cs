
using Acr.UserDialogs;
using HowManyTimes.Models;
using HowManyTimes.Services;
using HowManyTimes.Shared;
using HowManyTimes.Validators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
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
            UploadPhotoCommand = new Command(OnUploadPhotoCommandClicked);
            CameraPhotoCommand = new Command(OnCameraPhotoCommandClicked);
            SaveButtonCommand = new Command(OnSaveButtonCommandClicked);
            CancelButtonCommand = new Command(OnCancelButtonCommandClicked);
            ResetCounterCommand = new Command(OnResetCounterCommandClicked);
            DeleteButtonCommand = new Command(OnDeleteButtonCommandClicked);
            EditButtonCommand = new Command(OnEditButtonCommandClicked);
            DeleteCategoryButtonCommand = new Command(OnDeleteCategoryButtonCommandClicked);
            IncreaseCounterCommand = new Command(async (o) =>
            {
                Label l = (Label)o;

                var _scale = l.Scale;
                await l.ScaleTo(_scale * 0.2, 250);

                SelectedCounter.IncreaseCounter();

                // save counter
                SaveCounterToDatabase(SelectedCounter);

                origCounter = BaseCounter.CopyCounter(SelectedCounter);
                Count = SelectedCounter.Counter;
                await l.ScaleTo(_scale, 250);
            });

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

                    // set the category
                    SetCategoryAndSteps();

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
                SelectedIndex = -1;
                EditMode = true;
            }

            // store original value
            origCounter = BaseCounter.CopyCounter(SelectedCounter);

            CatFav = SelectedCounter.Favorite;
            CatPin = SelectedCounter.Pinned;
            Count = SelectedCounter.Counter;
            CounterImage = SelectedCounter.ImageUrl;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when Edit counter button is clicked
        /// </summary>
        public void OnEditButtonCommandClicked()
        {
            EditMode = true;

            // set the category
            SetCategoryAndSteps();
        }

        /// <summary>
        /// Called when Clear category button is clicked
        /// </summary>
        public void OnDeleteCategoryButtonCommandClicked()
        {
            SelectedIndex = -1;
            SelectedCategory = null;
            SelectedCounter.CounterCategory = null;
        }

        private void SetCategoryAndSteps()
        {
            // set the category
            if (SelectedCounter.CounterCategory != null)
                SelectedIndex = CategoriesList.FindIndex(c => c.Id == SelectedCounter.CounterCategory.Id);
            else
                SelectedIndex = -1;

            // set steps and switch
        }

        /// <summary>
        /// Called when Reset counter button is clicked
        /// </summary>
        public async void OnResetCounterCommandClicked()
        {
            _ = await ResetCounter(SelectedCounter);
            Count = SelectedCounter.Counter;
        }

        /// <summary>
        /// Called when Delete button is clicked
        /// </summary>
        public async void OnDeleteButtonCommandClicked()
        {
            if (await DeleteCounter(SelectedCounter))
            {
                // return to previous page
                NavigateBack();
            }
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

            // restore original values
            CatFav = SelectedCounter.Favorite;
            CatPin = SelectedCounter.Pinned;
            Count = SelectedCounter.Counter;
            CounterImage = SelectedCounter.ImageUrl;

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
            else
            {
                tmpC.CounterCategory=new Category();
            }

            // validate the input
            CounterDetailValidator counterValidator = new CounterDetailValidator(UseSteps);

            var validationResults = counterValidator.Validate(tmpC);
            if (!validationResults.IsValid)
            {
                ShowErrors(validationResults);
                return;
            }

            SelectedCounter = BaseCounter.CopyCounter(tmpC);
            await SaveCounterToDatabase(SelectedCounter);

            // store new original baseline for cancellation
            origCounter = BaseCounter.CopyCounter(SelectedCounter);

            EditMode = false;
        }

        /// <summary>
        /// Inserts or updates counter in database
        /// </summary>
        /// <param name="cnt"></param>
        public async Task<bool> SaveCounterToDatabase(BaseCounter cnt)
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
                        await UpdateCategoryCounters(cnt.CounterCategory, true);
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

                    // update category total counter
                    // 1) no category => new assigned
                    // 2) category => no category
                    // 3) category => changed category
                    if(origCounter.CounterCategory == null && cnt.CounterCategory != null)
                    {
                        // no category => new assigned
                        await UpdateCategoryCounters(cnt.CounterCategory, true);
                    }
                    else if(origCounter.CounterCategory != null && cnt.CounterCategory.Name == null)
                    {
                        // was category and is deleted
                        // new empty category is created, have to check for name == null as the object is not null
                        await UpdateCategoryCounters(origCounter.CounterCategory, false);
                    }
                    else if(origCounter.CounterCategory.Id != cnt.CounterCategory.Id)
                    {
                        // original category changed to another one
                        await UpdateCategoryCounters(origCounter.CounterCategory, false);
                        await UpdateCategoryCounters(cnt.CounterCategory, true);
                    }

                    var a = 0;
                    

                    UserDialogs.Instance.Toast($"Counter {cnt.Name} successfully updated.");

                    // notify mainpage that category has been edited
                    MessagingCenter.Send<BaseCounter>(cnt, "UpdateCounterFav");
                }
                return (true);
            }
            catch (Exception e)
            {
                LogService.Log(LogType.Error, e.Message);
                return (false);
            }
        }

        /// <summary>
        /// Updates counters total for the category
        /// </summary>
        /// <param name="cat">category</param>
        /// <param name="Increase">true to increase counter, false to decrease</param>
        /// <returns></returns>
        public async Task<bool> UpdateCategoryCounters(Category cat, bool Increase)
        {
            try
            {
                if(Increase)
                    cat.Counters++;
                else
                    cat.Counters--;

                LogService.Log(LogType.Info, $"Updating total counters counter ({cat.Counters}) for category {cat.Name}");
                await DBService.UpdateData(cat);

                MessagingCenter.Send<Category>(cat, "Update");
                return (true);
            }
            catch(Exception e)
            {
                LogService.Log(LogType.Error, e.Message);
                return (false);
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

        /// <summary>
        /// Handles click when upload image button is clicked
        /// </summary>
        public async void OnUploadPhotoCommandClicked()
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            { Title = "Please select a photo" }
            );

            if (result is null)
            {
                // no photo selected
                CounterImage = null;
                SelectedCounter.ImageUrl = null;

                LogService.Log(LogType.Info, "No photo selected");
                UserDialogs.Instance.Toast("No photo selected for this category");
            }
            else
            {
                try
                {
                    CounterImage = result.FullPath;
                    SelectedCounter.ImageUrl= result.FullPath;
                }
                catch (Exception e)
                {
                    LogService.Log(LogType.Error, e.Message);
                    CounterImage = null;
                    SelectedCounter.ImageUrl = null;
                }
            }
        }

        /// <summary>
        /// Handles click when open camera button is clicked
        /// </summary>
        public async void OnCameraPhotoCommandClicked()
        {
            if (!MediaPicker.IsCaptureSupported)
            {
                // photo capturing not supported
                CounterImage = null;
                SelectedCounter.ImageUrl = null;
                LogService.Log(LogType.Error, "No camera available");
                UserDialogs.Instance.Toast("No camera available!");
            }

            var result = await MediaPicker.CapturePhotoAsync();

            if (result is null)
            {
                // no photo selected
                CounterImage= null;
                SelectedCounter.ImageUrl=null;
                LogService.Log(LogType.Info, "No photo selected");
                UserDialogs.Instance.Toast("No photo selected for this category");
            }
            else
            {
                try
                {
                    CounterImage = result.FullPath;
                    SelectedCounter.ImageUrl = result.FullPath;
                }
                catch (Exception e)
                {
                    LogService.Log(LogType.Error, e.Message);
                    CounterImage = null;
                    SelectedCounter.ImageUrl = null;
                }
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
        public ICommand DeleteCategoryButtonCommand { get; set; }

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
        public int SelectedIndex { get; set; }
        public string CounterImage { get; set; }
        #endregion

        #region Private properties
        private BaseCounter origCounter;
        private List<Category> categories;
        #endregion
    }
}
