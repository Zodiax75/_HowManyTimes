using SQLite;
using SQLiteNetExtensions.Attributes;

namespace HowManyTimes.Models
{
    public class Category
    {
        #region Constructors
        #endregion

        #region Methods
        #endregion

        #region Properties
        /// <summary>
        /// ID of the category for sql-lite purposes
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Is category marked as favorite?
        /// </summary>
        public bool Favorite { get; set; }
        /// <summary>
        /// Name of the category
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the category
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// ID of the related counter
        /// </summary>
        [ForeignKey(typeof(BaseCounter))]
        public int CategoryCounterId { get; set; }

        /// <summary>
        /// Number of counters within this category
        /// </summary>
        public int Counters { get; set; }
        #endregion

        #region Private properties
        #endregion
    }
}
