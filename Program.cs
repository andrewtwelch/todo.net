using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace todo.net
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DB ToDoDatabase = new DB();
            Console.WriteLine("todo.net\r\n");

            if (args.Length == 0)
            {
                Console.WriteLine(
                    """
                    Please enter a valid command.

                    Commands:
                    add <summary>           Add a new task
                    show <id>               Show details for a task
                    complete <id>           Mark task as completed
                    complete <id> <date>    Mark task as completed on a certain date
                    uncomplete <id>         Mark task as uncomplete
                    delete <id>             Delete task
                    undelete <id>           Restore deleted task
                    list                    List all open and recently closed tasks
                    list all                List all tasks
                    list complete           List completed tasks
                    list deleted            List deleted tasks
                    """
                );
            }
            else
            {

                switch (args[0])
                {
                    case "add":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Please enter a summary to add a new task.");
                        }
                        else
                        {
                            string summary = String.Join(' ', args.Skip(1).ToArray());
                            ToDoItem item = new ToDoItem
                            {
                                Summary = summary
                            };
                            ToDoDatabase.Add(item);
                            Console.WriteLine("You have added a new task.");
                            Console.WriteLine($"\r\nYou have {ToDoDatabase.GetActive().Count().ToString()} outstanding tasks.");
                        }
                        break;
                    case "show":
                        if (args.Length != 2)
                        {
                            Console.WriteLine("Please enter an ID to view a task.");
                        }
                        else
                        {
                            try
                            {
                                ToDoItem? item = ToDoDatabase.Get(Int32.Parse(args[1]));
                                if (item == null)
                                {
                                    Console.WriteLine("Task with that ID was not found.");
                                }
                                else
                                {
                                    Console.WriteLine(
                                        $"""
                                    Task ID:        {item.Id.ToString()}
                                    Summary:        {item.Summary}
                                    Date Added:     {item.Added.ToString()}
                                    Status:         {(
                                            item.Deleted
                                                ? "Deleted"
                                                : (
                                                    item.Completed.HasValue
                                                        ? "Completed on " + DateOnly.FromDateTime(item.Completed.Value).ToString()
                                                        : "Not completed"
                                                )
                                        )}
                                    """
                                    );
                                    Console.WriteLine($"\r\nYou have {ToDoDatabase.GetActive().Count().ToString()} outstanding tasks.");
                                }
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine(
                                    "Unable to parse task ID, please ensure you have entered a number."
                                );
                            }
                        }
                        break;
                    case "complete":
                        if (args.Length != 2 && args.Length != 3)
                        {
                            Console.WriteLine("Please enter an ID to complete a task.");
                        }
                        else
                        {
                            try
                            {
                                ToDoItem? item = ToDoDatabase.Get(Int32.Parse(args[1]));
                                if (item == null)
                                {
                                    Console.WriteLine("Task with that ID was not found.");
                                }
                                else if (item.Completed.HasValue)
                                {
                                    Console.WriteLine(
                                        $"Task {item.Id.ToString()} ({item.Summary}) has already been marked as completed on {DateOnly.FromDateTime(item.Completed.Value).ToString()}."
                                    );
                                    Console.WriteLine($"\r\nYou have {ToDoDatabase.GetActive().Count().ToString()} outstanding tasks.");
                                }
                                else
                                {
                                    DateTime completedDate;
                                    if (args.Length == 2)
                                    {
                                        completedDate = DateTime.Today;
                                    }
                                    else
                                    {
                                        if (!DateTime.TryParse(args[2].ToString(), out completedDate))
                                        {
                                            Console.WriteLine(
                                                "Unable to parse completion date, please ensure you have entered a valid date."
                                            );
                                            break;
                                        }
                                    }
                                    item.Completed = completedDate;
                                    ToDoDatabase.Update(item);
                                    Console.WriteLine(
                                        $"Task {item.Id.ToString()} ({item.Summary}) has been marked as completed on {DateOnly.FromDateTime(completedDate).ToString()}."
                                    );
                                    Console.WriteLine($"\r\nYou have {ToDoDatabase.GetActive().Count().ToString()} outstanding tasks.");
                                }
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine(
                                    "Unable to parse task ID, please ensure you have entered a number."
                                );
                            }
                        }
                        break;
                    case "uncomplete":
                        if (args.Length != 2)
                        {
                            Console.WriteLine(
                                "Please enter an ID to uncomplete a task, with no extra arguments."
                            );
                        }
                        else
                        {
                            try
                            {
                                ToDoItem? item = ToDoDatabase.Get(Int32.Parse(args[1]));
                                if (item == null)
                                {
                                    Console.WriteLine("Task with that ID was not found.");
                                }
                                else if (!item.Completed.HasValue)
                                {
                                    Console.WriteLine(
                                        $"Task {item.Id.ToString()} ({item.Summary}) has not yet been marked as complete."
                                    );
                                    Console.WriteLine($"\r\nYou have {ToDoDatabase.GetActive().Count().ToString()} outstanding tasks.");
                                }
                                else
                                {
                                    item.Completed = null;
                                    ToDoDatabase.Update(item);
                                    Console.WriteLine(
                                        $"Task {item.Id.ToString()} ({item.Summary}) has been marked as uncompleted."
                                    );
                                    Console.WriteLine($"\r\nYou have {ToDoDatabase.GetActive().Count().ToString()} outstanding tasks.");
                                }
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine(
                                    "Unable to parse task ID, please ensure you have entered a number."
                                );
                            }
                        }
                        break;
                    case "delete":
                        if (args.Length != 2)
                        {
                            Console.WriteLine(
                                "Please enter an ID to delete a task, with no extra arguments."
                            );
                        }
                        else
                        {
                            try
                            {
                                ToDoItem? item = ToDoDatabase.Get(Int32.Parse(args[1]));
                                if (item == null)
                                {
                                    Console.WriteLine("Task with that ID was not found.");
                                }
                                else if (item.Deleted)
                                {
                                    Console.WriteLine(
                                        $"Task {item.Id.ToString()} ({item.Summary}) has already been deleted."
                                    );
                                    Console.WriteLine($"\r\nYou have {ToDoDatabase.GetActive().Count().ToString()} outstanding tasks.");
                                }
                                else
                                {
                                    item.Deleted = true;
                                    ToDoDatabase.Update(item);
                                    Console.WriteLine(
                                        $"Task {item.Id.ToString()} ({item.Summary}) has been deleted."
                                    );
                                    Console.WriteLine($"\r\nYou have {ToDoDatabase.GetActive().Count().ToString()} outstanding tasks.");
                                }
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine(
                                    "Unable to parse task ID, please ensure you have entered a number."
                                );
                            }
                        }
                        break;
                    case "undelete":
                        if (args.Length != 2)
                        {
                            Console.WriteLine(
                                "Please enter an ID to undelete a task, with no extra arguments."
                            );
                        }
                        else
                        {
                            try
                            {
                                ToDoItem? item = ToDoDatabase.Get(Int32.Parse(args[1]));
                                if (item == null)
                                {
                                    Console.WriteLine("Task with that ID was not found.");
                                }
                                else if (!item.Deleted)
                                {
                                    Console.WriteLine(
                                        $"Task {item.Id.ToString()} ({item.Summary}) still exists."
                                    );
                                    Console.WriteLine($"\r\nYou have {ToDoDatabase.GetActive().Count().ToString()} outstanding tasks.");
                                }
                                else
                                {
                                    item.Deleted = false;
                                    ToDoDatabase.Update(item);
                                    Console.WriteLine(
                                        $"Task {item.Id.ToString()} ({item.Completed} has been undeleted."
                                    );
                                    Console.WriteLine($"\r\nYou have {ToDoDatabase.GetActive().Count().ToString()} outstanding tasks.");
                                }
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine(
                                    "Unable to parse task ID, please ensure you have entered a number."
                                );
                            }
                        }
                        break;
                    case "list":
                        if (args.Length == 1)
                        {
                            List<ToDoItem> list = ToDoDatabase.GetActive();
                            list.AddRange(ToDoDatabase.GetRecentlyCompleted());
                            list = list.OrderBy(t => t.Id).ToList();
                            if (list.Count() == 0)
                            {
                                Console.WriteLine("You have no tasks.");
                            }
                            else
                            {
                                PrintTaskList(list);
                            }
                            Console.WriteLine($"\r\nYou have {ToDoDatabase.GetActive().Count().ToString()} outstanding tasks.");
                        }
                        else if (args.Length == 2)
                        {
                            switch (args[1])
                            {
                                case "all":
                                    List<ToDoItem> allList = ToDoDatabase.GetAll();
                                    allList.OrderBy(t => t.Id);
                                    if (allList.Count() == 0)
                                    {
                                        Console.WriteLine("You have no tasks.");
                                    }
                                    else
                                    {
                                        PrintTaskList(allList);
                                    }
                                    Console.WriteLine($"\r\nYou have {ToDoDatabase.GetActive().Count().ToString()} outstanding tasks.");
                                    break;
                                case "completed":
                                    List<ToDoItem> completedList = ToDoDatabase.GetCompleted();
                                    completedList.OrderBy(t => t.Id);
                                    if (completedList.Count() == 0)
                                    {
                                        Console.WriteLine("You have no completed tasks.");
                                    }
                                    else
                                    {
                                        PrintTaskList(completedList);
                                    }
                                    Console.WriteLine($"\r\nYou have {ToDoDatabase.GetActive().Count().ToString()} outstanding tasks.");
                                    break;
                                case "deleted":
                                    List<ToDoItem> deletedList = ToDoDatabase.GetDeleted();
                                    deletedList.OrderBy(t => t.Id);
                                    if (deletedList.Count() == 0)
                                    {
                                        Console.WriteLine("You have no deleted tasks.");
                                    }
                                    else
                                    {
                                        PrintTaskList(deletedList);
                                    }
                                    Console.WriteLine($"\r\nYou have {ToDoDatabase.GetActive().Count().ToString()} outstanding tasks.");
                                    break;
                                default:
                                    Console.WriteLine(
                                        "Invalid argument, please specify either all, completed, deleted, or leave blank for active and recently completed tasks."
                                    );
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine(
                                "Too many arguments, please specify either all, completed, deleted, or leave blank for active and recently completed tasks."
                            );
                        }
                        break;
                    default:
                        Console.WriteLine(
                            """
                        Please enter a valid command.

                        Commands:
                        add <summary>       Add a new task
                        show <id>           Show details for a task
                        complete <id>       Mark task as completed
                        uncomplete <id>     Mark task as uncomplete
                        delete <id>         Delete task
                        list                List all open and recently closed tasks
                        list all            List all tasks
                        list complete       List completed tasks
                        list deleted        List deleted tasks
                        """
                        );
                        break;
                }
            }

            static void PrintTaskList(List<ToDoItem> list)
            {
                int highestIdLength = list.Max(t => t.Id).ToString().Length;
                foreach (ToDoItem item in list)
                {
                    Console.WriteLine($"{(item.Completed.HasValue ? "[x]" : "[ ]")}  {item.Id.ToString().PadRight(highestIdLength)}  {item.Summary}{(item.Completed.HasValue ? $" (Completed on {DateOnly.FromDateTime(item.Completed.Value).ToString()})" : String.Empty)}");
                }
            }
        }
    }
}
