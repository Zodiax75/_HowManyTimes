
using HowManyTimes.Models;
using HowManyTimes.Services;
using HowManyTimes.Shared;
using System;
using System.Windows.Input;

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

                //CategoryName = "";
                //CategoryDesc = "";
                //CategoryFavorite = false;
                //CategoryImage = null;

                EditMode = true;
            }

            // store original Favorite for cancel
            //favOriginal = CategoryFavorite;
        }
        #endregion

        #region Methods

        #endregion

        #region Properties
        /// <summary>
        /// Counter favorite Y/N
        /// </summary>
        public bool CounterFavorite { get; set; }

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
        #endregion

        #region Private properties
        private bool favOriginal;
        // private readonly CategoryDetailValidator categoryValidator;
        #endregion
    }
}
