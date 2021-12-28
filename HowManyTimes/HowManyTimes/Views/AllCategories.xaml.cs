using HowManyTimes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HowManyTimes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllCategories : ContentPage
    {
        #region Constructor
        public AllCategories()
        {
            InitializeComponent();

            BindingContext = new AllCategoriesViewModel();
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