using HowManyTimes.Shared;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace HowManyTimes.Models
{
    /// <summary>
    /// Base counter for all counters to inherit from
    /// </summary>
    public class BaseCounter
    {
        #region Constructors
        /// <summary>
        /// Create counter with minimal initialization
        /// </summary>
        public BaseCounter()
        {
            dateCreated = DateTime.Now;
            totalUpdated = 0;
        }

        /// <summary>
        /// Create fully initialized counter
        /// </summary>
        /// <param name="Name">Name</param>
        /// <param name="Description">Description</param>
        /// <param name="Step">Counter step</param>
        /// <param name="Type">Counter type</param>
        /// <param name="Favorite">Is favorite?</param>
        /// <param name="Pinned">Is pinned?</param>
        public BaseCounter(string Name, string Description, uint Step, CounterType Type, Category CounterCategory, bool Favorite, bool Pinned)
        {
            BaseCounter baseCounter = this;
            baseCounter.Name = Name;
            baseCounter.Description = Description;
            baseCounter.Step = Step;
            baseCounter.Type = Type;
            baseCounter.dateCreated = DateTime.Now;
            baseCounter.CounterCategory = CounterCategory;
            baseCounter.Favorite = Favorite;
            baseCounter.Pinned = Pinned;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Increases counter by one step
        /// </summary>
        public void IncreaseCounter()
        {
            internalCounter += this.Step;
            UpdateModifiedValues();
        }

        /// <summary>
        /// Decrease counter by one step
        /// </summary>
        public void DecreaseCounter()
        {
            internalCounter -= this.Step;
            UpdateModifiedValues();
        }

        /// <summary>
        /// Updates last modified date
        /// </summary>
        protected void UpdateModifiedValues()
        {
            DateModified = DateTime.Now;
            totalUpdated++;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Primary key for sql-lite purposes
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Is counter marked as favorite?
        /// </summary>
        public bool Favorite { get; set; }

        /// <summary>
        /// Is counter marked as pinned (only one allowed)?
        /// </summary>
        public bool Pinned { get; set; }

        /// <summary>
        /// Category of the counter
        /// </summary>
        [OneToOne("CategoryCounterId")]
        public Category CounterCategory { get; set; }

        /// <summary>
        /// Step by which counter will be increased
        /// </summary>
        public uint Step { get; set; }
        /// <summary>
        /// Date and time counter was created
        /// </summary>
        public DateTime DateCreated
        { get => dateCreated; }
        /// <summary>
        /// Date and time counter was last modified
        /// </summary>
        public DateTime DateModified { get; set; }

        /// <summary>
        /// Name of the counter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the counter
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Type of the counter using CounterType
        /// </summary>
        public CounterType Type { get; set; }

        /// <summary>
        /// Image of the counter
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Internal counter
        /// </summary>
        public uint Counter
        {
            get => internalCounter;
        }

        /// <summary>
        /// How many time was the counter updated
        /// </summary>
        public uint TotalUpdated
        {
            get => totalUpdated;
        }
        #endregion

        #region Private properties
        protected DateTime dateCreated;
        protected uint internalCounter;
        protected uint totalUpdated;
        #endregion
    }
}
