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
            // TODO: vyřešit založení tabulek při prvním spuštění!
            // await Database.CreateTableAsync<BaseCounter>().ConfigureAwait(false);
            // await Database.CreateTableAsync<Category>().ConfigureAwait(false);

            // insert into db
            _ = await Database.InsertAsync(Item).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates specific object in the database
        /// </summary>
        /// <typeparam name="T">Category or counter</typeparam>
        /// <param name="Item">Item to update</param>
        /// <returns></returns>
        public static async Task UpdateData<T>(T Item)
        {
            _ = await Database.UpdateAsync(Item).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns all categories
        /// </summary>
        /// <param name="FavoritesOnly">True if only favorite categories will be returned</param>
        /// <param name="Batchsize">Number of items to return</param>
        /// <returns>List of categories (all or limited sub)</returns>
        public static async Task<List<Category>> GetCategory(bool FavoritesOnly, int? Batchsize)
        {
            List<Category> categories;

            if (FavoritesOnly)
            {
                // get only favorite ones
                if (Batchsize != null)
                    // take only first "batchsize" number of records
                    categories = await Database.Table<Category>().Where(x => x.Favorite).Take(int.Parse(Batchsize.ToString())).ToListAsync().ConfigureAwait(false);
                else
                    categories = await Database.Table<Category>().Where(x => x.Favorite).ToListAsync().ConfigureAwait(false);
            }
            else
            {
                // get all categories
                if (Batchsize != null)
                    // take only first "batchsize" number of records
                    categories = await Database.Table<Category>().Take(int.Parse(Batchsize.ToString())).ToListAsync().ConfigureAwait(false);
                else
                    categories = await Database.Table<Category>().ToListAsync().ConfigureAwait(false);
            }

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

        /// <summary>
        /// Deletes specific item from the database
        /// </summary>
        /// <param name="id">ID of the category to be deleted</param>
        /// <returns></returns>
        public static async Task DeleteCategory(int id)
        {
            await Database.DeleteAsync<Category>(id);
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
