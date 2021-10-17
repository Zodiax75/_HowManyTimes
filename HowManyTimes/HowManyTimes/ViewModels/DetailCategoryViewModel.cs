
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
        public DetailCategoryViewModel(int categoryId) : this()
        {

        }
        #endregion

        #region Methods

        #endregion

        #region Properties

        #endregion

        #region Private properties

        #endregion
    }
}

