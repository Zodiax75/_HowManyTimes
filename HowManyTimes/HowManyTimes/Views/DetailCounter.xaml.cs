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

            if (!increaseLabel.IsEnabled)
                increaseLabel.TextColor = Color.Silver;

            if(!resetLabel.IsEnabled)
                resetLabel.TextColor = Color.Silver;

            
        }
        #endregion

        #region Properties

        #endregion

        #region Private properties

        #endregion
    }
}