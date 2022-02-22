using HowManyTimes.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowManyTimes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailCounter : ContentPage
    {
        #region Constructor
        public DetailCounter(int? counterId = null)
        {
            InitializeComponent();
            
            // null = new counter, id = edit counter
            if (counterId == null)
                BindingContext = new DetailCounterViewModel(null);
            else
                BindingContext = new DetailCounterViewModel(counterId.Value);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Switch toogle method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stepSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            Switch s = (Switch)sender;
            if (s != null)
            {
                if (s.IsToggled)
                    editSteps.IsVisible = true;
                else
                    editSteps.IsVisible = false;
            }
            else
                editSteps.IsVisible = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // show switch on if the counter has initially steps set to >1
            if(editSteps.Text != "1")
                stepSwitch.IsToggled = true;

        }
        #endregion

        #region Properties

        #endregion

        #region Private properties

        #endregion
        private void Label_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            bool a =false;

            if(BindingContext != null)
                a = ((DetailCounterViewModel)BindingContext).EditMode;

            if(!a)
                increaseLabel.TextColor = Color.Black;
            else
                increaseLabel.TextColor = Color.Silver;

            if (!a)
                resetLabel.TextColor = Color.Silver;
            else
                resetLabel.TextColor = Color.Black;

            // if there is no category for the counter, do not show background plate (isvisible is already bound)
            if (labelcounterCategory.Text == null)
                counterCategoryNonEdit.BackgroundColor = Color.Transparent;
            else
                counterCategoryNonEdit.BackgroundColor = Color.FromHex("#ed7307");
        }
    }
}