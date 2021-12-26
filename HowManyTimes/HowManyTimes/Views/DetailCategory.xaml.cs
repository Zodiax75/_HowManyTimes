
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HowManyTimes.ViewModels;
using System;

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
                BindingContext = new DetailCategoryViewModel(null);
            else
                BindingContext = new DetailCategoryViewModel(categoryId.Value);
        }
        #endregion

        #region Methods
        public void OnEditClicked(object sender, EventArgs e)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Private properties

        #endregion
    }
}