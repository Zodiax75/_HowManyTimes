using HowManyTimes.Shared;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace HowManyTimes.Models
{
    /// <summary>
    /// Base counter for all counters to inherit from
    /// </summary>
    [Table("Counters)")]
    public class BaseCounter
    {
        #region Constructors
        /// <summary>
        /// Create counter with minimal initialization
        /// </summary>
        public BaseCounter()
        {
            dateCreated = DateTime.Now;
            DateModified = DateTime.Now;
            totalUpdated = 0;
            TotalUpdated = 0;
            this.Step = 1;
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
        public BaseCounter(int Id, string Name, string Description, int Counter, int Step, CounterType Type, Category CounterCategory, DateTime DateCreated, DateTime DateModified, string ImageUrl, uint TotalUpdates, bool Favorite, bool Pinned)
        {
            BaseCounter baseCounter = this;
            baseCounter.Id = Id;
            baseCounter.Name = Name;
            baseCounter.Description = Description;
            baseCounter.internalCounter = Counter;
            baseCounter.Counter = Counter;
            baseCounter.Step = Step;
            baseCounter.Type = Type;
            baseCounter.dateCreated = DateCreated;
            baseCounter.DateModified = DateModified;
            baseCounter.ImageUrl = ImageUrl;
            baseCounter.totalUpdated = TotalUpdates;
            baseCounter.TotalUpdated = TotalUpdates;
            baseCounter.CounterCategory = CounterCategory;
            baseCounter.Favorite = Favorite;
            baseCounter.Pinned = Pinned;
        }
        #endregion

        #region Methods
        public static BaseCounter CopyCounter(BaseCounter c)
        {
            BaseCounter a = new BaseCounter(c.Id, c.Name, c.Description, c.Counter, c.Step, c.Type, c.CounterCategory, c.DateCreated, c.DateModified, c.ImageUrl, c.TotalUpdated, c.Favorite, c.Pinned);
            return a;
        }

        /// <summary>
        /// Increases counter by one step
        /// </summary>
        public void IncreaseCounter()
        {
            internalCounter += this.Step;
            Counter += this.Step;
            UpdateModifiedValues();
        }

        /// <summary>
        /// Decrease counter by one step
        /// </summary>
        public void DecreaseCounter()
        {
            internalCounter -= this.Step;
            Counter -= this.Step;
            UpdateModifiedValues();
        }

        public void ResetCounter()
        {
            this.internalCounter = 0;
            this.Counter = 0;
            // No  UpdateModifiedValues(); as we do not want to count it as update
        }

        /// <summary>
        /// Updates last modified date
        /// </summary>
        protected void UpdateModifiedValues()
        {
            DateModified = DateTime.Now;
            TotalUpdated++;
            totalUpdated++;
        }

        public override bool Equals(object obj)
        {
            var c = obj as BaseCounter;

            if (
                this.Counter == c.Counter &&
                this.CounterCategory == c.CounterCategory &&
                this.DateCreated == c.DateCreated &&
                this.DateModified == c.DateModified &&
                this.Description == c.Description &&
                this.Favorite == c.Favorite &&
                this.Id == c.Id &&
                this.ImageUrl == c.ImageUrl &&
                this.Name == c.Name &&
                this.Step == c.Step &&
                this.TotalUpdated == c.TotalUpdated &&
                this.Type == c.Type
                )
                return true;
            else
                return false;
                    
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

        [ForeignKey(typeof(Category))]
        public int CategoryId { get; set; }

        /// <summary>
        /// Category of the counter
        /// </summary>
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public Category CounterCategory { get; set; }

        /// <summary>
        /// Step by which counter will be increased
        /// </summary>
        public int Step { get; set; }
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
        public int Counter { get; set; }
        //public int Counter
        //{
        //    get => internalCounter;
        //}

        /// <summary>
        /// How many time was the counter updated
        /// </summary>
        public uint TotalUpdated { get; set; }
        #endregion

        #region Private properties
        protected DateTime dateCreated;
        protected int internalCounter;
        protected uint totalUpdated;
        #endregion
    }
}
