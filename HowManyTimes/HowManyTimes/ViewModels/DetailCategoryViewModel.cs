
using HowManyTimes.Models;
using HowManyTimes.Services;
using HowManyTimes.Shared;
using System;

namespace HowManyTimes.ViewModels
{
    internal class DetailCategoryViewModel : BaseViewModel
    {
        // NOTE: No OnPropertyChanges calls for setters as FODY is implemented in background!

        #region Constructor
        public DetailCategoryViewModel()
        {

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
                    categorySelected = DBService.GetCategory(categoryId.Value).Result;
                    LogService.Log(LogType.Info, $"Loaded details for category {categorySelected.Id}: {categorySelected.Name}");
                }
                catch(Exception e)
                {
                    LogService.Log(LogType.Error, e.Message);
                }
            }
            else
            {
                // new category
                LogService.Log(LogType.Info, "New category details initialized");
            }

        }
        #endregion

        #region Methods

        #endregion

        #region Properties
        /// <summary>
        /// Selected category
        /// </summary>
        public Category SelectedCategory
        {
            get { return categorySelected; }
            set
            {
                categorySelected = value;
            }
        }
        #endregion

        #region Private properties
        private Category categorySelected = null;
        #endregion
    }
}

