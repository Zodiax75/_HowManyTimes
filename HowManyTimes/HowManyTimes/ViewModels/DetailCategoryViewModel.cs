﻿using HowManyTimes.Models;
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
    public class DetailCategoryViewModel : CategoryBaseViewModel
    {
        // NOTE: No OnPropertyChanges calls for setters as FODY is implemented in background!

        #region Constructor
        public DetailCategoryViewModel()
        {
            // bind commands to buttons
            SaveButtonCommand = new Command(OnSaveButtonCommandClicked);
            CancelButtonCommand = new Command(OnCancelButtonCommandClicked);
            UploadPhotoCommand = new Command(OnUploadPhotoCommandClicked);
            CameraPhotoCommand = new Command(OnCameraPhotoCommandClicked);
            EditButtonCommand = new Command(OnEditButtonCommandClicked);

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
        /// Called when Delete button is clicked
        /// </summary>
        public override async void OnDeleteButtonCommandClicked()
        {
            if(await DeleteCategory(SelectedCategory))
            {
                // return to previous page
                NavigateBack();
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

            // validate the input
            var validationResults = categoryValidator.Validate(c);
            if (!validationResults.IsValid)
            {
                ShowErrors(validationResults);
                return;
            }

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

                    // notify mainpage that category has been edited
                    if (favOriginal == CategoryFavorite)
                        // favorite flag not changes, just update values
                        MessagingCenter.Send<Category>(SelectedCategory, "Update");
                    else
                        MessagingCenter.Send<Category>(SelectedCategory, "UpdateFav");
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
        public ICommand UploadPhotoCommand { get; set; }
        public ICommand CameraPhotoCommand { get; set; }
        public ICommand EditButtonCommand { get; set; }

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