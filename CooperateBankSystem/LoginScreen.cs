
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CooperateBankSystem
{
    public class LoginScreen
    {        
        public static string DisplayLogin(string connectionString)
        {
            var message = "";
            for (var tries = 3; tries > 0; tries--)
            {
                Console.Clear();
                
                Console.Title = "Internal Bank System - Login";

                Console.WriteLine();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkMagenta;

                Console.WriteLine("Welcome to the Internal Bank System of Cooperative Company !");

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;

                //error messages
                if (tries < 3 && tries > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine($"{message} Please try again. You have {tries} tries.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                
                //username and password input
                Console.WriteLine();
                Console.WriteLine("Enter your username and password to login to your account.");
                Console.Write("Username: ");
                var username = Console.ReadLine().Trim();
                Console.Write("Password: ");
                var password = ReadLineMasked();

                //Input validation
                var account = new InternalAccount();                               
                Console.WriteLine();            
                if (!account.IsUserValid(username))
                {
                    message = "User not found.";
                }
                else if (!account.IsPasswordValid(username, password))
                {
                   message = "Invalid entry.";
                }
                else
                {
                    //Successful login
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Login Successful!");
                    Console.ForegroundColor = ConsoleColor.White;
                    System.Threading.Thread.Sleep(500);
                    return username;
                }                
            }

            //after 3 fails application displays message and closes.
            return null;            
        }
      
        private static string ReadLineMasked(char mask = '*')
        {
            var sb = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (!char.IsControl(keyInfo.KeyChar))
                {
                    sb.Append(keyInfo.KeyChar);
                    Console.Write(mask);
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);

                    if (Console.CursorLeft == 0)
                    {
                        Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                        Console.Write(' ');
                        Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                    }
                    else Console.Write("\b \b");
                }
            }
            Console.WriteLine();
            return sb.ToString();
        }
    }
}
