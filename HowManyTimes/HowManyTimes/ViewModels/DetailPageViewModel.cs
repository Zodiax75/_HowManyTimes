using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace HowManyTimes.ViewModels
{
    public class DetailPageViewModel : BaseViewModel
    {
    // NOTE: No OnPropertyChanges calls for setters as FODY is implemented in background!

        #region Constructor
        public DetailPageViewModel()
        {
            tapCommand = new Command(OnTapped);
            tapCommandSingle = new Command(OnTappedSingle);
        }
        #endregion

        public int taps = 0;
        public ICommand tapCommand;
        public ICommand tapCommandSingle;

        public ICommand TapCommand
        {
            get { return tapCommand; }
        }
        public ICommand TapCommandSingle
        {
            get { return tapCommandSingle; }
        }


        void OnTapped(object s)
        {
            taps++;
            Title = taps.ToString();
        }
        void OnTappedSingle(object s)
        {
            taps--;
            Title = taps.ToString();

        }

        #region Properties
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
            }
        }
        #endregion

        #region Private properties
        private string title = "Went for MTB";
        #endregion
    }
}
