using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace todo.net
{
    internal class DB
    {
        SQLiteConnection DBConn;

        public DB()
        {
            string databasePath = Path.Combine(Environment.CurrentDirectory, "todo.net.db");
            DBConn = new SQLiteConnection(databasePath);
            DBConn.CreateTable<ToDoItem>();
        }

        public ToDoItem Get(int id)
        {
            var query = DBConn.Table<ToDoItem>().Where(t => t.Id.Equals(id));
            return query.FirstOrDefault();
        }

        public List<ToDoItem> GetActive()
        {
            var query = DBConn
                .Table<ToDoItem>()
                .Where(t => t.Completed == null)
                .OrderBy(t => t.Id);
            if (query.Count() == 0)
            {
                return new List<ToDoItem>();
            }
            return query.ToList();
        }

        public List<ToDoItem> GetCompleted()
        {
            var query = DBConn
                .Table<ToDoItem>()
                .Where(t => t.Completed != null)
                .OrderBy(t => t.Id);
            return query.ToList();
        }

        public List<ToDoItem> GetRecentlyCompleted()
        {
            DateTime oneWeekAgo = DateTime.Today.AddDays(-7);
            var query = DBConn
                .Table<ToDoItem>()
                .Where(t =>
                    (
                        t.Completed != null
                        && t.Completed > oneWeekAgo
                    )
                )
                .OrderBy(t => t.Id);
            return query.ToList();
        }

        public List<ToDoItem> GetAll()
        {
            var query = DBConn.Table<ToDoItem>().Where(t => t.Deleted == false).OrderBy(t => t.Id);
            return query.ToList();
        }

        public List<ToDoItem> GetDeleted()
        {
            var query = DBConn.Table<ToDoItem>().Where(t => t.Deleted == true).OrderBy(t => t.Id);
            return query.ToList();
        }

        public void Add(ToDoItem item)
        {
            DBConn.Insert(item);
        }

        public void Update(ToDoItem item)
        {
            DBConn.Update(item);
        }
    }

    internal class ToDoItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Summary { get; set; }
        public DateTime Added { get; } = DateTime.Today;
        public DateTime? Completed { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
