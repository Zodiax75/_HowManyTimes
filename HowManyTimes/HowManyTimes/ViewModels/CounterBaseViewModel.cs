using HowManyTimes.Models;
using HowManyTimes.Services;
using HowManyTimes.Shared;
using System.Collections.ObjectModel;

namespace HowManyTimes.ViewModels
{
    internal class CounterBaseViewModel : BaseViewModel
    {
        #region Constructor

        #endregion

        #region Methods
        /// <summary>
        /// Adds new item to the counter collection
        /// </summary>
        /// <param name="col">Colection</param>
        /// <param name="c">counter item</param>
        public static void AddCounterItem(ObservableCollection<BaseCounter> col, BaseCounter c)
        {
            col.Add(c);
        }
        #endregion

        #region Properties

        #endregion

        #region Private properties

        #endregion
    }
}
