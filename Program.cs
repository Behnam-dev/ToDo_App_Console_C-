using System;
using Microsoft.Data.Sqlite;

namespace TD
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.OutputEncoding = System.Text.Encoding.UTF8; // To properly show emojis
            Console.Clear();

            Connect_To_db();
            Menu();

            Console.Read();
        }
        public static void Menu()
        {
            Console.WriteLine("\n\t\t\t\t\t\t*** Todo List ***");
            Console.WriteLine("\n\t\t\t\t\t\t[1] Create Task\n\t\t\t\t\t\t[2] Delete Task\n\n\n\t\t\t");
            Console.WriteLine( "⚫️"+ DateTime.Today.ToShortDateString());
            Print_Tasks();
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
        public static void Print_Tasks()
            {
            using(var connection = new SqliteConnection("Data Source=tasks.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM tasks";
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        var title = reader["title"].ToString();
                        var date = reader["date"].ToString();
                        var done = reader["done"];
                        Console.WriteLine(title + "\t" + date + "\n\n\t\t\t");
                    }
                }
                command.ExecuteNonQuery(); 
            }
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
                        // Console.WriteLine("\nPlease enter a date in this format \"3 26\"");
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
            }
            Console.Clear();
            Menu();  
        }
    }
}