using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace HowManyTimes.Services
{
    public static class DBService
    {
        #region Methods

        #endregion

        #region Properties
        /// <summary>
        /// Returns local database handler
        /// </summary>
        public static SQLiteAsyncConnection Database 
        { 
            get 
            {
                if(db is null)
                {
                    var databasePath = Path.Combine(FileSystem.AppDataDirectory, "HMTData.db");

                    db = new SQLiteAsyncConnection(databasePath);
                    db.CloseAsync();

                    return null;
                }

                return(db);
            }
        }
        #endregion

        #region Private properties
        /// <summary>
        /// Main shared connection to sql-lite database
        /// </summary>
        private static SQLiteAsyncConnection db;
        #endregion
    }


}
