using HowManyTimes.Models;
using HowManyTimes.Services;
using HowManyTimes.Shared;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Acr.UserDialogs;
using Xamarin.Essentials;
using FluentValidation;
using HowManyTimes.Validators;

namespace HowManyTimes.ViewModels
{
    public class DetailCategoryViewModel : BaseViewModel
    {
        // NOTE: No OnPropertyChanges calls for setters as FODY is implemented in background!

        #region Constructor
        public DetailCategoryViewModel()
        {
            // bind commands to buttons
            SaveButtonCommand = new Command(OnSaveButtonCommandClicked);
            CancelButtonCommand = new Command(OnCancelButtonCommandClicked);
            EditButtonCommand = new Command(OnEditButtonCommandClicked);
            DeleteButtonCommand = new Command(OnDeleteButtonCommandClicked);
            FavoriteButtonCommand = new Command(OnFavoriteButtonCommandClicked);
            UploadPhotoCommand = new Command(OnUploadPhotoCommandClicked);
            CameraPhotoCommand = new Command(OnCameraPhotoCommandClicked);
            BackButtonCommand = new Command(OnBackButtonCommandClicked);

            // initialize validators
            categoryValidator = new CategoryDetailValidator();
        }

        /// <summary>
        /// Init detail page with particular category details
        /// </summary>
        /// <param name="categoryId">category Id to be preloaded</param>
        public DetailCategoryViewModel(int? categoryId=null) : this()
        {
            // load details for category
            if(categoryId != null)
            {
                LogService.Log(LogType.Info, $"Loading category details for category Id: {categoryId}");

                try
                {
                    // load the specified category
                    SelectedCategory = DBService.GetCategory(categoryId.Value).Result;
                    LogService.Log(LogType.Info, $"Loaded details for category {SelectedCategory.Id}: {SelectedCategory.Name}");

                    // load bindings with actual category data
                    CategoryName = SelectedCategory.Name;
                    CategoryDesc = SelectedCategory.Description;
                    CategoryFavorite = SelectedCategory.Favorite;
                    CategoryImage = SelectedCategory.ImageUrl;

                    EditMode = false;
                }
                catch(Exception e)
                {
                    LogService.Log(LogType.Error, e.Message);
                }
            }
            else
            {
                // new category
                SelectedCategory = new Category();
                LogService.Log(LogType.Info, "New category details initialized");

                CategoryName = "";
                CategoryDesc = "";
                CategoryFavorite = false;
                CategoryImage = null;

                EditMode = true;
            }

            // store original Favorite for cancel
            favOriginal = CategoryFavorite;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Handles click when upload image button is clicked
        /// </summary>
        public async void OnBackButtonCommandClicked()
        {
            NavigateBack();
        }
        /// <summary>
        /// Handles click when upload image button is clicked
        /// </summary>
        public async void OnUploadPhotoCommandClicked()
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                { Title = "Please select a photo"}
            );
         
            if (result is null)
            {
                // no photo selected
                CategoryImage = null;
                LogService.Log(LogType.Info, "No photo selected");
                UserDialogs.Instance.Toast("No photo selected for this category");
            }
            else
            {
                try
                {
                    CategoryImage = result.FullPath;
                }
                catch (Exception e)
                {
                    LogService.Log(LogType.Error, e.Message);
                    CategoryImage = null;
                }
            }
        }

        /// <summary>
        /// Handles click when open camera button is clicked
        /// </summary>
        public async void OnCameraPhotoCommandClicked()
        {
            if(!MediaPicker.IsCaptureSupported)
            {
                // photo capturing not supported
                CategoryImage = null;
                LogService.Log(LogType.Error, "No camera available");
                UserDialogs.Instance.Toast("No camera available!");
            }

            var result = await MediaPicker.CapturePhotoAsync();

            if (result is null)
            {
                // no photo selected
                CategoryImage = null;
                LogService.Log(LogType.Info, "No photo selected");
                UserDialogs.Instance.Toast("No photo selected for this category");
            }
            else
            {
                try
                {
                    CategoryImage = result.FullPath;
                }
                catch (Exception e)
                {
                    LogService.Log(LogType.Error, e.Message);
                }
            }
        }

        /// <summary>
        /// Called when Save button is clicked
        /// </summary>
        public async void OnSaveButtonCommandClicked()
        {
            int tempId = SelectedCategory.Id;

            // New object should be created because changing just property doesnt fire OnPropertyChanged event!!!!!
            Category c = new Category
            {
                Id = tempId,
                Name = CategoryName,
                Description = CategoryDesc,
                Favorite = CategoryFavorite,
                ImageUrl = CategoryImage
            };

            var validationResults = categoryValidator.Validate(c);
            if (validationResults.IsValid)
            {
                App.Current.MainPage.DisplayAlert("FluentValidation", "Validation Success..!!", "Ok");
            }
            else
            {
                App.Current.MainPage.DisplayAlert("FluentValidation", validationResults.Errors[0].ErrorMessage, "Ok");
            }
            return;

            SelectedCategory = c;

            try
            {
                // insert or update decision
                if (SelectedCategory.Id == 0)
                {
                    // insert
                    LogService.Log(LogType.Info, $"Inserting new category {SelectedCategory.Name}");

                    await DBService.InsertData(SelectedCategory);

                    UserDialogs.Instance.Toast($"Category {SelectedCategory.Name} successfully created.");
                    // notify mainpage that catogory has been added
                    MessagingCenter.Send<Category>(SelectedCategory, "AddNew");
                }
                else
                {
                    // update
                    LogService.Log(LogType.Info, $"Updating category {SelectedCategory.Id}: {SelectedCategory.Name}");

                    await DBService.UpdateData(SelectedCategory);

                    UserDialogs.Instance.Toast($"Category {SelectedCategory.Name} successfully updated.");

                    // notify mainpage that catogory has been added
                    MessagingCenter.Send<Category>(SelectedCategory, "Update");
                }
            }
            catch (Exception e)
            {
                LogService.Log(LogType.Error, e.Message);
            }

            favOriginal = CategoryFavorite;
            EditMode = false;
        }

        /// <summary>
        /// Called when Delete button is clicked
        /// </summary>
        public async void OnDeleteButtonCommandClicked()
        {
            // check if there are some counters connected
            if(SelectedCategory.Counters > 0)
            {
                await UserDialogs.Instance.AlertAsync("To delete a category, you must not have any counters active.","Active counters!", "Ok");
                return;
            }

            // delete from DB if existing category (with verification)
            var result = await UserDialogs.Instance.ConfirmAsync($"Are you sure to delete category {SelectedCategory.Name}?", "Confirm delete", "Yes", "No");

            if (!result)
                return; // detetion was not confirmed

            LogService.Log(LogType.Info, $"Deleting category {SelectedCategory.Id}: {SelectedCategory.Name}");

            try
            {
                await DBService.DeleteData(SelectedCategory);

                // notify mainpage that catogory has been deleted
                MessagingCenter.Send<Category>(SelectedCategory, "Delete");

                UserDialogs.Instance.Toast($"Category {SelectedCategory.Name} successfully deleted.");

                // return to previous page
                NavigateBack();
            }
            catch (Exception e)
            {
                LogService.Log(LogType.Error, e.Message);
            }
        }

        /// <summary>
        /// Called when Favorite button is clicked
        /// </summary>
        public void OnFavoriteButtonCommandClicked()
        {
            // allow change only in edit mode
            if(EditMode)
            {
                // change Favorite and save it to current category
                CategoryFavorite = !CategoryFavorite;
                SelectedCategory.Favorite = CategoryFavorite;

                LogService.Log(LogType.Info, $"Updating favorite for category {SelectedCategory.Name} to {SelectedCategory.Favorite}");
            }
        }

        /// <summary>
        /// Called when Cancel button is clicked
        /// </summary>
        public void OnEditButtonCommandClicked()
        {
            EditMode = true;
        }

        /// <summary>
        /// Called when Cancel button is clicked
        /// </summary>
        public void OnCancelButtonCommandClicked()
        {
            if(String.IsNullOrEmpty(SelectedCategory.Name))
                NavigateBack(); // nothing is filled and task is cancelled, go to previous page

            // edit was cancelled, restore original data to bindings
            CategoryName = SelectedCategory.Name;
            CategoryDesc = SelectedCategory.Description;
            CategoryFavorite = favOriginal;
            CategoryImage = SelectedCategory.ImageUrl;

            EditMode = false;
        }

        /// <summary>
        /// Navigates back in the flow
        /// </summary>
        private async void NavigateBack()
        {
            await Application.Current.MainPage.Navigation.PopAsync(true);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Stream of the category image
        /// </summary>
        public string CategoryImage { get; set; }

        /// <summary>
        /// Selected category
        /// </summary>
        public Category SelectedCategory { get; set; }
        /// <summary>
        /// Category name
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Category description
        /// </summary>
        public string CategoryDesc { get; set; }

        /// <summary>
        /// Category favorite Y/N
        /// </summary>
        public bool CategoryFavorite { get; set; }

        public ICommand SaveButtonCommand { get; set; }
        public ICommand CancelButtonCommand { get; set;}
        public ICommand EditButtonCommand { get; set; }
        public ICommand FavoriteButtonCommand { get; set; }
        public ICommand DeleteButtonCommand { get; set; }
        public ICommand UploadPhotoCommand { get; set; }
        public ICommand CameraPhotoCommand { get; set; }
        public ICommand BackButtonCommand { get; set; }

        /// <summary>
        /// Return true if we are in edit mode (new or existing) and not view mode
        /// </summary>
        public bool EditMode { get; set; }
        #endregion

        #region Private properties
        private bool favOriginal;
        private readonly CategoryDetailValidator categoryValidator;
        #endregion
    }
}