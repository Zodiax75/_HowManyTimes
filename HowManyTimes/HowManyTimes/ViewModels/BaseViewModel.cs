using HowManyTimes.Views;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace HowManyTimes.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Constructor
        public BaseViewModel()
        {
            // Bind commands
            NewCategoryCommand = new Command(OnNewCategoryCommandClicked);
            NewCounterCommand = new Command(OnNewCounterCommandClicked);
            BackButtonCommand = new Command(OnBackButtonCommandClicked);
        }
        #endregion

        #region Methods
        /// <summary>
        /// On New category click function
        /// </summary>
        private void OnNewCategoryCommandClicked()
        {
            Application.Current.MainPage.Navigation.PushAsync(new DetailCategory(), true);
        }

        /// <summary>
        /// On New counter click function
        /// </summary>
        private void OnNewCounterCommandClicked()
        {
            Application.Current.MainPage.Navigation.PushAsync(new DetailCounter(), true);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Handles click when upload image button is clicked
        /// </summary>
        public void OnBackButtonCommandClicked()
        {
            NavigateBack();
        }

        /// <summary>
        /// Navigates back in the flow
        /// </summary>
        protected async void NavigateBack()
        {
            await Application.Current.MainPage.Navigation.PopAsync(true);
        }
        #endregion

        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Command to handle back button click
        /// </summary>
        public ICommand BackButtonCommand { get; set; }
        /// <summary>
        /// Command to handle New category click
        /// </summary>
        public ICommand NewCategoryCommand { get; set; }
        
        /// <summary>
        /// Command to handle New counter click
        /// </summary>
        public ICommand NewCounterCommand { get; set; }
        #endregion

        #region Private properties
        #endregion
    }
}
