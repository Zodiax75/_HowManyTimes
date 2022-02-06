
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
            // load details for category
            if (counterId != null)
            {
                LogService.Log(LogType.Info, $"Loading counter details for counter Id: {counterId}");

                try
                {
                    // load the specified counter
                    //SelectedCategory = DBService.GetCategory(categoryId.Value).Result;
                    //LogService.Log(LogType.Info, $"Loaded details for counter {SelectedCategory.Id}: {SelectedCategory.Name}");

                    //// load bindings with actual category data
                    //CategoryName = SelectedCategory.Name;
                    //CategoryDesc = SelectedCategory.Description;
                    //CategoryFavorite = SelectedCategory.Favorite;
                    //CategoryImage = SelectedCategory.ImageUrl;

                    EditMode = false;
                }
                catch (Exception e)
                {
                    LogService.Log(LogType.Error, e.Message);
                }
            }
            else
            {
                // new category
                SelectedCounter = new BaseCounter();
                LogService.Log(LogType.Info, "New counter details initialized");

                EditMode = true;
            }

            // store original Favorite for cancel
            //favOriginal = CategoryFavorite;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when Save button is clicked
        /// </summary>
        public async void OnSaveButtonCommandClicked()
        {
            // New object should be created because changing just property doesnt fire OnPropertyChanged event!!!!!
            BaseCounter tmpC = new BaseCounter
            {
                Name = SelectedCounter.Name,
                Description = SelectedCounter.Description,
                Step = SelectedCounter.Step,
                Pinned = SelectedCounter.Pinned,
                Favorite = SelectedCounter.Favorite
            };

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

            SelectedCounter.Favorite = true;

            try
            {
                // insert or update decision
                if (SelectedCounter.Id == 0)
                {
                    // insert
                    LogService.Log(LogType.Info, $"Inserting new counter {SelectedCounter.Name}");

                    await DBService.InsertData(SelectedCounter);

                    // update category counter if needed
                    if(SelectedCounter.CounterCategory != null)
                    {
                        SelectedCounter.CounterCategory.Counters++;

                        LogService.Log(LogType.Info, $"Updating total counters counter ({SelectedCounter.CounterCategory.Counters}) for category {SelectedCounter.CounterCategory.Name}");
                        await DBService.UpdateData(SelectedCounter.CounterCategory);

                        MessagingCenter.Send<Category>(SelectedCounter.CounterCategory, "Update");
                    }

                    UserDialogs.Instance.Toast($"Counter {SelectedCounter.Name} successfully created.");
                    // notify mainpage that counter has been added
                    MessagingCenter.Send<BaseCounter>(SelectedCounter, "AddNewCounter");
                }
                else
                {
                    // update
                    //LogService.Log(LogType.Info, $"Updating category {SelectedCategory.Id}: {SelectedCategory.Name}");

                    //await DBService.UpdateData(SelectedCategory);

                    //UserDialogs.Instance.Toast($"Category {SelectedCategory.Name} successfully updated.");

                    //// notify mainpage that category has been edited
                    //if (favOriginal == CategoryFavorite)
                    //    // favorite flag not changes, just update values
                    //    MessagingCenter.Send<Category>(SelectedCategory, "Update");
                    //else
                    //    MessagingCenter.Send<Category>(SelectedCategory, "UpdateFav");
                }
            }
            catch (Exception e)
            {
                LogService.Log(LogType.Error, e.Message);
            }

            //favOriginal = CategoryFavorite;
            EditMode = false;
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
        #endregion

        #region Private properties
        private BaseCounter origCounter;
        private List<Category> categories;
        #endregion
    }
}
