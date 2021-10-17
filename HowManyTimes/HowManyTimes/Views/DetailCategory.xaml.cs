
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HowManyTimes.ViewModels;

namespace HowManyTimes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailCategory : ContentPage
    {
        #region Constructor
        public DetailCategory(int? categoryId = null)
        {
            InitializeComponent();

            // null = new category, id = edit category
            if (categoryId == null)
                BindingContext = new DetailCategoryViewModel();
            else
                BindingContext = new DetailCategoryViewModel(int.Parse(categoryId.ToString()));
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