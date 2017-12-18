using Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Threading;

namespace CooperateBankSystem
{
    class Program
    {
        private const string connectionString = @"Data Source=.\SQL2017XPS;Initial Catalog=afdemp_csharp_1;Integrated Security = true; Trusted_Connection = True;";
        private static List<string> memory= new List<string>();

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("el-GR");
            var fmt = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            fmt.ShortDatePattern = @"yyyy-MM-dd";
            fmt.LongTimePattern = @"HH:mm:ss.FFF";


            Console.Title = "Internal Bank System";
            //opening message
            Console.WriteLine("Opening Application...");

            //check for database connectivity
            Console.Write("Opening connection to Database.. ");
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) // conn antikeimeno p antiproswpevei to connection me thn vash mas
                {
                    conn.Open();
                    Console.WriteLine($"Connection state: {conn.State}");                     
                }
            }
            catch(Exception)
            {                
                Console.WriteLine("Connection error.");// You have {tries} tries.\n");             
            }
            Thread.Sleep(1000); //wait

            //Login Screen
            var username = LoginScreen.DisplayLogin(connectionString);

            if (string.IsNullOrEmpty(username))
            {
                //Login fail --> Application closes
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid input. Application is closing..");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine("Goodbye..");
            }
            else
            {
                while (true)
                {
                    //Display Menu
                    var choice = Menu.DisplayMenu(username);
                    bool admin = new InternalAccount().IsUserAdmin(username);

                    var i = 0;
                    Console.WriteLine(i++);

                    //Display SubMenu
                    switch (choice.Key)
                    {
                        case ConsoleKey.D1:
                            {
                                ViewMyAccount(username);
                            }
                            break;
                        case ConsoleKey.D2:
                            {
                                if (admin)
                                {
                                    ViewAccounts(username);
                                }
                                else
                                {
                                    DepositToAdmin(username);
                                }
                            }
                            break;
                        case ConsoleKey.D3:
                            {
                                Deposit(username);
                            }
                            break;
                        case ConsoleKey.D4:
                            {
                                if (admin)
                                {
                                    WithdrawMenu(username);
                                }
                                else
                                {
                                    //Send to file user
                                    SendToFile(username);
                                }
                            }
                            break;
                        case ConsoleKey.D5:
                            {
                                if (admin)
                                {
                                    //sent to file admin
                                    SendToFile(username);
                                }
                                else
                                {
                                    InvalidInput();
                                }
                            }
                            break;
                        case ConsoleKey.Escape:
                            {
                                Exit(username);
                            }
                            break;
                        default:
                            {
                                InvalidInput();
                            }
                            break;
                    }
                }
            }

            Console.ReadKey();
        }



        private static void ViewMyAccount(string username)
        {
            var account = new InternalAccount();

            Console.Title = "Internal Bank System - My Account";
            Console.Clear();
            if (account.IsUserAdmin(username))
                Console.WriteLine($"User:{username}\tYou pressed 1. View Cooperative's internal bank account");
            else
                Console.WriteLine($"User:{username}\tYou pressed 1. View my bank account");

            Console.WriteLine();          
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine(account.ViewAccount(username));
            Console.WriteLine("------------------------------------------------------------");
  
            GoToMainMenu();
            return;
        }

        private static void ViewAccounts(string username)
        {
            var message = "";
            var i = 0;
            while(true)
            {
                Console.Clear();
                Console.Title = "Internal Bank System -  View user's Account";
                Console.Clear();
                Console.WriteLine($"User:{username}\tYou pressed 2. View user bank accounts\n");

                //error messages
                if (i > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{message} \nPlease try again.");// You have {tries} tries.\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    //------------------------------------------------------------------------------------------------
                    var key = Esc();
                    if (key.Key == ConsoleKey.Escape)
                        return;
                    //------------------------------------------------------------------------------------------------
                }
                i++;

                //available users
                var account = new InternalAccount();
                var users = account.GetUsernames();

                //username input
                Console.WriteLine("Please insert the username of the account you want to view.");
                Console.WriteLine("These are the available users:");
                foreach (var item in users)
                {
                    Console.WriteLine($"- {item}");
                }
                Console.WriteLine("To view all accounts type [all] in place of username.");
                Console.Write("Username: ");
                var otherUsername = Console.ReadLine().Trim();

                if (otherUsername.ToLower() == "all")
                {
                    //success
                    Console.WriteLine("\nProcessing your request..");

                    Thread.Sleep(500);
                    Console.Clear();
                    Console.WriteLine($"User:{username}\tYou pressed 2. View user bank accounts\n");
                    Console.WriteLine("------------------------------------------------------------");

                    foreach (var item in users)
                    {                                              
                        Console.WriteLine(account.ViewAccount(item));
                        Console.WriteLine("------------------------------------------------------------");
                    }  
                    
                    GoToMainMenu();
                    return;
                }
                //input validation
                if (!users.Contains(otherUsername))
                {
                    message = "Invalid input. User not found.";
                }
                else
                {
                    //success
                    Console.WriteLine("\nProcessing your request..");

                    Thread.Sleep(500);
                    Console.Clear();
                    Console.WriteLine($"User:{username}\tYou pressed 2. View user bank accounts\n");
                    Console.WriteLine("------------------------------------------------------------");
                    Console.WriteLine(account.ViewAccount(otherUsername));
                    Console.WriteLine("------------------------------------------------------------");
                    
                    GoToMainMenu();
                    return;
                }
            }
        }

        private static void Deposit(string username)
        {
            var message = "";
            var i = 0;
            while(true)
            {
                Console.Clear();
                Console.Title = "Internal Bank System - Deposit Menu";
                Console.WriteLine($"User:{username}\tYou pressed 3. Deposit to user's bank account\n");

                //error messages
                if (i > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{message} \nPlease try again.");
                    Console.ForegroundColor = ConsoleColor.White;
                    //------------------------------------------------------------------------------------------------
                    var key = Esc();
                    if (key.Key == ConsoleKey.Escape)
                        return;
                    //------------------------------------------------------------------------------------------------
                }
                i++;

                //available users
                var account = new InternalAccount();
                var users = account.GetUsernames();
                                
                //deposit data input
                Console.WriteLine("Please insert the username of the account you want to deposit to and the amount you want to deposit.");
                Console.WriteLine("These are the available users:");
                foreach (var item in users)
                {
                    if (item != username)
                        Console.WriteLine($"- {item}");
                }
                if (account.IsUserAdmin(username))
                {
                    Console.WriteLine("To deposit the same amount to all accounts type [all] in place of username.");
                }
                Console.Write("Username: ");
                var otherUsername = Console.ReadLine().Trim();
                Console.Write("Amount: ");
                var amountValidation = Decimal.TryParse(Console.ReadLine(), out decimal amount);

                //If admin and input all
                if(account.IsUserAdmin(username) && otherUsername.ToLower() == "all")
                {
                    //Input validation for all
                    var adminAmount = (users.Count - 1) * amount;
                    Console.WriteLine();
                    if (amount <= 0 || !amountValidation)
                    {
                        message = "Invalid input. Insert a positive number as amount..";
                        continue;
                    }
                    else if (!account.HasSufficientBalance(username, adminAmount))
                    {
                        message = "Insufficient account balance.";
                        continue;
                    }
                    

                    //Successful input
                    Console.WriteLine("\nProcessing your request..");

                    account.DepositToAll(username, amount, out string msg);
                    memory.Add(msg);
                    Thread.Sleep(500);

                    DisplayResults();
                   
                    return;
                }


                //Input validation for user
                Console.WriteLine();
                if (!account.IsUserValid(otherUsername))
                {
                    message = "User not found.";
                }
                else if (amount <= 0 || !amountValidation)
                {
                    message = "Invalid input. Insert a positive number as amount..";
                }
                else if (username == otherUsername)
                {
                    message = "Cannot deposit to your account.";
                }
                else if (account.IsUserAdmin(otherUsername))
                {
                    message = "Cannot deposit to Cooperative’s internal bank account. \nTo deposit to Cooperative’s internal bank account go back to Main Menu.";
                }
                else if (!account.HasSufficientBalance(username, amount))
                {
                    message = "Insufficient account balance.";
                }
                else
                {
                    //Successful input
                    Console.WriteLine("\nProcessing your request..");

                    account.DepositToAccount(username, otherUsername, amount, out string msg);
                    memory.Add(msg);
                    Thread.Sleep(500);

                    DisplayResults();
                    return;
                }                
            }
        }

        private static void DepositToAdmin(string username)
        {
            var message = "";
            var i = 0;
            while (true)
            {                
                Console.Clear();
                Console.Title = "Internal Bank System - Deposit Menu";
                Console.WriteLine($"User:{username}\tYou pressed 2. Deposit to Cooperative’s internal bank account\n");

                //error messages                
                if (i > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{message} \nPlease try again."); // You have {tries} tries.\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    //------------------------------------------------------------------------------------------------
                    var key = Esc();
                    if (key.Key == ConsoleKey.Escape)
                        return;
                    //------------------------------------------------------------------------------------------------
                }
                i++;

                //deposit data input
                Console.WriteLine("Please insert the amount you want to deposit.");
                Console.Write("Amount: ");
                var amountValidation = Decimal.TryParse(Console.ReadLine(), out decimal amount);

                //Input validation
                var account = new InternalAccount();
                Console.WriteLine();

                if (amount <= 0 || !amountValidation)
                {
                    message = "Invalid input. Insert a positive number as amount..";
                }
                else if (!account.HasSufficientBalance(username, amount))
                {
                    message = "Insufficient account balance.";
                }
                else
                {
                    //Successful input
                    Console.WriteLine("\nProcessing your request..");

                    account.DepositToAccount(username, "admin", amount, out string msg); 
                    memory.Add(msg);
                    Thread.Sleep(500);
                    DisplayResults();
                    return;
                }               
            }            
        }

        private static void WithdrawMenu(string username)
        {
            var message = "";
            var i = 0;
            while (true)
            {
                Console.Clear();
                Console.Title = "Internal Bank System - Withdraw Menu";
                Console.WriteLine($"User:{username}\tYou pressed 4. Withdraw from account\n");


                //error messages
                if (i > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{message} \nPlease try again."); // You have {tries} tries.\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    //------------------------------------------------------------------------------------------------
                    var key = Esc();
                    if (key.Key == ConsoleKey.Escape)
                        return;
                    //------------------------------------------------------------------------------------------------
                }
                i++;

                //available users
                var account = new InternalAccount();
                var users = account.GetUsernames();

                //withdraw data input
                Console.WriteLine("Please insert the username of the account you want to withdraw from and the amount you want to withdraw.");
                Console.WriteLine("These are the available users:");
                foreach (var item in users)
                {
                    Console.WriteLine($"- {item}");
                }
                Console.WriteLine("To withdraw the same amount to from all accounts type [all] in place of username.");
                Console.Write("Username: ");
                var otherUsername = Console.ReadLine().Trim();
                Console.Write("Amount: ");
                var amountValidation = Decimal.TryParse(Console.ReadLine(), out decimal amount);

                // if admin from all
                if (account.IsUserAdmin(username) && otherUsername.ToLower() == "all")
                {
                    //Input validation for all
                    Console.WriteLine();
                    if (amount <= 0 || !amountValidation)
                    {
                        message = "Invalid input. Insert a positive number as amount..";
                        continue;
                    }

                    var suffBalance = true;
                    foreach (var unames in users)
                    {
                        if (!account.HasSufficientBalance(unames, amount))
                        {
                            message = "Insufficient account balance.";
                            suffBalance = false;
                            continue;
                        }
                    }
                    if (!suffBalance)
                    {
                        message = "Insufficient account balance.";
                        continue;
                    }


                    //Successful input
                    Console.WriteLine("\nProcessing your request..");

                    account.WithdrawFromAll(username, amount, out string msg);
                    memory.Add(msg);
                    Thread.Sleep(500);

                    DisplayResults();
                    return;
                }

                //Input validation
                Console.WriteLine();
                if (!account.IsUserValid(otherUsername))
                {
                    message = "User not found.";
                }
                else if (amount <= 0 || !amountValidation)
                {
                    message = "Invalid input. Insert a positive number as amount..";
                }
                else if (username == otherUsername)
                {
                    message = "Cannot withdraw from Cooperative's internal bank account.";
                }
                else if (!account.HasSufficientBalance(otherUsername, amount))
                {
                    message = "Insufficient account balance.";
                }
                else
                {
                    //Successful input
                    Console.WriteLine("\nProcessing your request..");

                    account.Withdraw(otherUsername, amount, out string msg);
                    memory.Add(msg);

                    Thread.Sleep(500);
                    DisplayResults();
                    return;
                }                
            }
        }

        private static void SendToFile(string username)
        {
            Console.Title = "Internal Bank System - Save File";
            Console.Clear();
            var account = new InternalAccount();
            if (account.IsUserAdmin(username))
                Console.WriteLine($"User:{username}\tYou pressed 5. Send today’s transactions and Exit");
            else
                Console.WriteLine($"User:{username}\tYou pressed 4. Send today’s transactions and Exit");
            Console.WriteLine();

            if (memory.Count==0)
            {
                Console.WriteLine("There are no transactions to be saved.. Memory is empty.");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\nPress any button to continue and exit the application ..");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();

                Console.Clear();
                Console.Title = "Internal Bank System - Exit";
                Console.WriteLine($"\nGoodbye {username}.");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\nPress any button to exit the application ..");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
                Environment.Exit(0);
                Console.ReadKey();
            }

            //success
            Console.WriteLine("Saving memory to file..");
            var file = new FileAccess();
            var valid = file.SendTodaysStatements(username, memory);
            Thread.Sleep(500);
            if (valid)
            {
                Console.WriteLine( );
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Successful Operation!");
                Console.ForegroundColor = ConsoleColor.White;

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\nPress any button to continue ..");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();

                Console.Clear();
                Console.Title = "Internal Bank System - Exit";
                Console.WriteLine($"\nGoodbye {username}.");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\nPress any button to exit the application ..");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
                Environment.Exit(0);
                Console.ReadKey();
            }

        }

        private static void DisplayResults()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Successful Transaction!");
            Console.ForegroundColor = ConsoleColor.White;

            GoToMainMenu();
        } // SubMenu

        private static ConsoleKeyInfo Esc()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\nTo go back to Main Menu press ESC. To continue press any key..");
            Console.ForegroundColor = ConsoleColor.White;
            var esc = Console.ReadKey();
            Console.SetCursorPosition(0, Console.CursorTop - 2);
            Console.WriteLine("                                                                  ");
            Console.WriteLine("                                                                  ");
            Console.SetCursorPosition(0, Console.CursorTop - 2);
            return esc;
        } //Menu & SubMenu

        private static void GoToMainMenu()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\nPress any button to continue to Main Menu ..");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
        } // Submenu

        private static void Exit(string username)
        {
            Console.Clear();
            Console.Title = "Internal Bank System - Exit";
            Console.WriteLine("\nAre you sure you want to exit?");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Press the spacebar to go back to Main Menu or any other button to continue..");
            Console.ForegroundColor = ConsoleColor.White;
            var key = Console.ReadKey();
            if (key.Key != ConsoleKey.Spacebar)
            {
                Console.Clear();
                Console.WriteLine($"\nGoodbye {username}.");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\nPress any button to exit the application ..");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
                Environment.Exit(0);
                Console.ReadKey();
            }
        } //Menu

        private static void InvalidInput()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Invalid input. Please try again.");// You have {tries} tries.\n");
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(1500);
        }
    }
}
