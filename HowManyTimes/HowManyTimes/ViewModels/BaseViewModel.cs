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
            BackButtonCommand = new Command(OnBackButtonCommandClicked);
        }
        #endregion

        #region Methods
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
        public ICommand BackButtonCommand { get; set; }
        #endregion

        #region Private properties
        #endregion
    }
}
