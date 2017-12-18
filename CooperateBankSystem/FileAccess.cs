using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CooperateBankSystem
{
    public class FileAccess
    {
        public bool SendTodaysStatements(string username, List<string> memory)
        {
            var account = new InternalAccount();
            var today = DateTime.Today;
            string filePath = "C:\\Users\\vivi\\Desktop\\";
            string fileName;

            if (account.IsUserAdmin(username))
            {
                fileName = "statement_admin" + today.ToString("_dd_MM_yyyy") + ".txt";
            }
            else
            {
                var userId = account.GetUserId(username);
                fileName = "statement_user_" + userId + today.ToString("_dd_MM_yyyy") + ".txt";
            }

            if (!File.Exists(filePath + fileName))
            {
                memory.Insert(0, $"User: {username}\t | Date: {today:dd/MM/yyyy}\n");
                memory.Insert(1, "");
                memory.Insert(2, "Transaction\t Participants\t Amount\t Transaction Date        \t Account Balance\n");
            }

            try
            {
                using (var sw = new StreamWriter(filePath+fileName, true, Encoding.Unicode))
                {
                    foreach (var item in memory)
                    {
                        sw.WriteLine(item);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nA file error occurred.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Press Enter to see Details about the error or any other button to continue..");
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine($"\nError Details: {ex.Message}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("\nPress any button to continue ..");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();
                }

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\nTo go back to Main Menu press Enter. To continue and exit press any key..");
                Console.ForegroundColor = ConsoleColor.White;
                var k = Console.ReadKey();
                if (k.Key != ConsoleKey.Enter)
                {
                    Console.Clear();
                    Console.Title = "Internal Bank System - Exit";
                    Console.WriteLine("\nApplication is closing..");
                    Console.WriteLine($"Goodbye.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("\nPress any button to exit the application ..");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();
                    Environment.Exit(0);
                    Console.ReadKey();
                }
                return false;
            }
        }
    }
}
