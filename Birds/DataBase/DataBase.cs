using Birds.Models;
using Birds.Views;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Birds.DataBase
{
    public class Database
    {
        static List<Bird> _birds;
        public static List<ObservableGroupCollection<string, Item>> groupedData = new List<ObservableGroupCollection<string, Item>>();

        public static List<Bird> BirdsTable
        {
            get
            {
                try
                {
                    Connection.CreateTable<Bird>();
                }
                catch
                {

                }
                try
                {
                    Connection.CreateTable<Images>();
                }
                catch
                {

                }
              
                _birds = Connection.Query<Bird>("SELECT * FROM Birds");
                if (_birds == null) _birds = new List<Bird>();
                return _birds;
            }
        }
        public static ObservableCollection<ObservableGroupCollection<string, Item>> BirdsGroups
        {
            get
            {
                groupedData = ItemsPage.viewModel.Items
                         .GroupBy(p => p.Text.ToString())
                         .Select(p => new ObservableGroupCollection<string, Item>(p))
                         .ToList();
                var ret = new ObservableCollection<ObservableGroupCollection<string, Item>>(groupedData);
                return ret;
            }
        }
        private static SQLiteConnection connection;
        public static SQLiteConnection Connection
        {
            get
            {
                string fileName = "Birds.db3";
                string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                if (connection == null) connection = new SQLiteConnection(Path.Combine(folderPath, fileName));
                SQLiteCommand c = connection.CreateCommand("PRAGMA encoding =\"UTF-8\"");
                c.ExecuteNonQuery();

                return connection;
            }

        }

        [SQLite.Table("Birds")]
        public class Bird
        {
            [SQLite.PrimaryKey, SQLite.AutoIncrement]
            public int No { get; set; }
            public int Number { get; set; }

            public string Id { get; set; } /*= Guid.NewGuid().ToString();*/
            public string Text { get; set; }
            public string Description { get; set; }
            public DateTime BirthDate { get; set; }
            [SQLite.Ignore]
            public string Search { get { return Id + "" + Text + "" + Description + "" + BirthDate.ToString(" yyyy MM dd "); } }

            [SQLite.Ignore]
            public string Age {
                get 
                {
                    var startDate = BirthDate;
                    var endDate = DateTime.Now;

                    int diff = (7 + (startDate.DayOfWeek - DayOfWeek.Saturday)) % 7;
                    var weekStartDate = startDate.AddDays(-1 * diff).Date;
                    var i = 1;
                    var weekEndDate = DateTime.MinValue;
                    while (weekEndDate < endDate)
                    {
                        i++;
                        weekEndDate = weekStartDate.AddDays(6);
                        var shownStartDate = weekStartDate < startDate ? startDate : weekStartDate;
                        var shownEndDate = weekEndDate > endDate ? endDate : weekEndDate;
                        //Console.WriteLine($"Week {i++}: {shownStartDate:dd MMMM yyyy} - {shownEndDate:dd MMMM yyyy}");
                        weekStartDate = weekStartDate.AddDays(7);
                    }
                    return i.ToString();
                }
            }

        }

       

    }
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Calculates the age in years of the current System.DateTime object today.
        /// </summary>
        /// <param name="birthDate">The date of birth</param>
        /// <returns>Age in years today. 0 is returned for a future date of birth.</returns>
        public static int Age(this DateTime birthDate)
        {
            return Age(birthDate, DateTime.Today);
        }

        /// <summary>
        /// Calculates the age in years of the current System.DateTime object on a later date.
        /// </summary>
        /// <param name="birthDate">The date of birth</param>
        /// <param name="laterDate">The date on which to calculate the age.</param>
        /// <returns>Age in years on a later day. 0 is returned as minimum.</returns>
        public static int Age(this DateTime birthDate, DateTime laterDate)
        {
            int age;
            age = laterDate.Year - birthDate.Year;

            if (age > 0)
            {
                age -= Convert.ToInt32(laterDate.Date < birthDate.Date.AddYears(age));
            }
            else
            {
                age = 0;
            }

            return age;
        }
    }
}
