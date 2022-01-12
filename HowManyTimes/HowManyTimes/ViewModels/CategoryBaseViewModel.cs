using System;
using System.Collections.Generic;
using System.Text;
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
        /// Called when Favorite button is clicked
        /// </summary>
        public virtual void OnFavoriteButtonCommandClicked()
        {
        }

        /// <summary>
        /// Called when Delete button is clicked
        /// </summary>
        public virtual async void OnDeleteButtonCommandClicked()
        {
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
