
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CooperateBankSystem
{
    public static class Menu
    {
        public static ConsoleKeyInfo DisplayMenu(string username)
        {
            Console.Clear();
            Console.Title = "Internal Bank System - Main Menu";

            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;

            Console.WriteLine($"Welcome {username}\n");

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            var account = new InternalAccount();
            if (account.IsUserAdmin(username))
            {
                return DisplayAdminMenu(username);
            }
            return DisplaySimpleUserMenu(username);

        }

        private static ConsoleKeyInfo DisplayAdminMenu(string username)
        {
            Console.WriteLine("Main Menu");
            Console.WriteLine("--------------------");
            Console.WriteLine("[ 1 ] View Cooperative's internal bank account");
            Console.WriteLine("[ 2 ] View user bank accounts");
            Console.WriteLine("[ 3 ] Deposit to user's bank account");
            Console.WriteLine("[ 4 ] Withdraw from account");
            Console.WriteLine("[ 5 ] Send today’s transactions and Exit");
            Console.WriteLine("--------------------\n");
            Console.Write("Please select an option from 1-5 or press ESC to exit the program: ");

            var choice = Console.ReadKey();
            return choice;
        }

        private static ConsoleKeyInfo DisplaySimpleUserMenu(string username)
        {
            Console.WriteLine("Main Menu");
            Console.WriteLine("--------------------");
            Console.WriteLine("[ 1 ] View my bank account");
            Console.WriteLine("[ 2 ] Deposit to Cooperative’s internal bank account");
            Console.WriteLine("[ 3 ] Deposit to user's bank account");
            Console.WriteLine("[ 4 ] Send today’s transactions and Exit");
            Console.WriteLine("--------------------\n");
            Console.Write("Please select an option from 1-4 or press ESC to exit the program: ");

            var choice = Console.ReadKey();
            return choice;
        }
    }
}
