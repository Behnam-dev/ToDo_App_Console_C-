using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.Sqlite;

namespace TD
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.OutputEncoding = System.Text.Encoding.UTF8; // To properly show emojis
        try
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
        }
        catch (IOException){}

            Console.Clear();

            Connect_To_db();
            while(true)
            {
                Menu();
            }
        }
        public static void Menu()
        {
            Console.WriteLine("\n\n\t\t\t\t*** Todo List ***");
            Console.WriteLine("\n\n\t\t\t\t🌀 Create Task\t\t\t "+ "🗓   "+ DateTime.Today.ToShortDateString());
            int count = Print_Tasks();
            // while(true)
            // {
            //     var key = Console.ReadKey(true);
            //     switch(key.Key)
            //     {
            //         case ConsoleKey.D1:
            //             Create_Task();
            //             return;
            //         case ConsoleKey.D2:
            //             Delete_Task();
            //             return;
            //         default:
            //             break; 
            //     }
            // }
            int row = 5;
            Console.SetCursorPosition(26, row);
            Console.Write("➡️");
            while(true)
            {
                var key = Console.ReadKey(true);
                switch(key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (row != 5)
                        {
                            Console.SetCursorPosition(26, row); // Delete the current arrow
                            Console.Write(" ");                 //
                            row = row-3;
                            Console.SetCursorPosition(26, row); // Set new arrow
                            Console.Write("➡️");
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (row != 5+(count*3))
                        {
                            Console.SetCursorPosition(26, row); 
                            Console.Write(" ");                 
                            row = row+3;
                            Console.SetCursorPosition(26, row); // Set new arrow
                            Console.Write("➡️");
                        }
                        break;
                    case ConsoleKey.Delete:
                        Query_Delete((row-5)/3);
                        // Query_Delete(1);
                        Console.Clear();
                        return;
                    case ConsoleKey.Enter:
                        if (row==5)
                        {
                            Create_Task();
                            return;
                        }
                        else
                        {
                            bool check = Done_Check_Task((row-5)/3);
                            Console.SetCursorPosition(32,row);
                            if (check)
                            {
                                Console.Write("🔲");
                            }
                            else
                            {
                                Console.Write("☑️");
                            }
                            Console.SetCursorPosition(26, row);
                        }
                        Change_Done((row-5)/3);
                        break;
                    default:
                        break;
                }
            }
        }

        public static void Connect_To_db()
        {
            using(var connection = new SqliteConnection("Data Source=tasks.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS tasks(
                                        id INTEGER PRIMARY KEY,
                                        title VARCHAR,
                                        date VARCHAR,
                                        done BOOLEAN)";
                command.ExecuteNonQuery();   
            }
        }
        public static int Print_Tasks()
        {
            int count = 0;
            using(var connection = new SqliteConnection("Data Source=tasks.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM tasks";
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        count++;
                        var title = reader["title"].ToString();
                        var date = reader["date"].ToString();
                        var done = reader["done"];
                        if (Convert.ToInt32(reader["done"]) != 0)
                        {
                        Console.WriteLine("\n\n\t\t\t\t" + "☑️  " + title + "\t" + "Date: "+ date);
                        }
                        else
                        {
                        Console.WriteLine("\n\n\t\t\t\t" + "🔲  " + title + "\t" + "Date: "+ date);
                        }
                    }
                }
                command.ExecuteNonQuery(); 
            }
            return count;
        }

        public static void Create_Task()
        {
            Console.Clear();
            Console.WriteLine($"\n\t\tToday is {DateTime.Today.ToShortDateString()}");
            Console.Write("\n\nTask: ");
            var title = Console.ReadLine();
            string? date = DateTime.Today.ToShortDateString(); // Default
            bool ask = true;
            while(ask)
            {
                try
                {
                    Console.Write("\n\nDate (month \"Space\" day): ");
                    date = Console.ReadLine();
                    if(date == null | date == "")
                    {
                        throw new Exception();
                    }
                    else if(date.Length > 5)
                    {
                        throw new Exception();
                    }
                    
                    foreach(char ch in date)
                    {
                        int ascii = (int)ch;
                        if(!((ascii >47 & ascii <58) | ascii == 32))
                        {
                            throw new Exception();
                        }
                    }
                }
                catch(Exception)
                {
                    Console.WriteLine("\nPlease enter a date in this format \"3 26\"");
                    continue;
                }
                ask = false;
            }
            date = date.Replace(' ', '/');
            using(var connection = new SqliteConnection("Data Source=tasks.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO tasks (title, date, done) VALUES(@title, @date, @done)";
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@done", false);
                command.ExecuteNonQuery();
            }
            Console.Clear();
            return;
        }

        public static void Query_Delete(int index)
        {
            using(var connection = new SqliteConnection("Data Source=tasks.db"))
            {   
                int count = 0;
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM tasks";
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        count ++ ;
                        if (count==index)
                        {
                            var del_command = connection.CreateCommand();
                            del_command.CommandText = "DELETE FROM tasks WHERE id=@id";
                            del_command.Parameters.AddWithValue("@id", reader["id"]);
                            del_command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public static bool Done_Check_Task(int index)
        {
            using(var connection = new SqliteConnection("Data Source=tasks.db"))
            {   
                int count = 0;
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM tasks";
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        count ++ ;
                        if (count==index)
                        {
                            return Convert.ToInt32(reader["done"]) != 0;
                        }
                    }
                }
            }
            return false;
        }

        public static void Change_Done(int index)
        {
            using(var connection = new SqliteConnection("Data Source=tasks.db"))
            {   
                int count = 0;
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM tasks";
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        count ++ ;
                        if (count==index)
                        {
                            var update_command = connection.CreateCommand();
                            update_command.CommandText = "UPDATE tasks SET done = @status WHERE id=@id";
                            if (Convert.ToInt32(reader["done"]) != 0)
                            {
                                update_command.Parameters.AddWithValue("@status",false);
                            }
                            else
                            {
                                update_command.Parameters.AddWithValue("@status",true);
                            }
                            update_command.Parameters.AddWithValue("@id",reader["id"]);
                            update_command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}