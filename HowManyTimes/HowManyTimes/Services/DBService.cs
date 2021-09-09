using HowManyTimes.Models;
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
        /// <summary>
        /// Inserts item into a database table
        /// </summary>
        /// <typeparam name="T">Category or Counter</typeparam>
        /// <param name="Item">Item to insert</param>
        /// <returns></returns>
        public static async Task InsertData<T>(T Item)
        {
            // create tables if needed
            await Database.CreateTableAsync<BaseCounter>().ConfigureAwait(false);
            await Database.CreateTableAsync<Category>().ConfigureAwait(false);

            // insert into db
            await Database.InsertAsync(Item).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns all categories
        /// </summary>
        /// <returns>List of all categories</returns>
        public static async Task<List<Category>> GetCategory()
        {
            List<Category> categories = await Database.Table<Category>().ToListAsync().ConfigureAwait(false);
            return categories;
        }

        /// <summary>
        /// Returns category specified by ID
        /// </summary>
        /// <param name="id">ID of the category</param>
        /// <returns>category object</returns>
        public static async Task<Category> GetCategory(int id)
        {
            Category cat = await Database.Table<Category>().FirstOrDefaultAsync(c => c.Id == id).ConfigureAwait(false);
            return cat;
        }
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
                }
                return db;
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
