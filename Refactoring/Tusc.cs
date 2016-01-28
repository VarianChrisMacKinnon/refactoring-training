using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactoring
{
    public class Tusc
    {        
        private static List<User> _userList;
        private static List<Product> _productList;

        public static int EXIT_NUMBER
        {
            get { return _productList.Count + 1; }
        }
        public static int ZERO_INDEX_ADJUST
        {
            get { return 1; }
        }

        public static void Start(List<User> userList, List<Product> productList)
        {
            _userList = userList;
            _productList = productList;

            // Write welcome message
            DisplayWelcomeMessage();

            // Login
            Login:
            
            string userName = PromptUserForUsername();

            // Validate Username
            if (!string.IsNullOrEmpty(userName))
            {
                bool isValidUser = ValidateUser(userName);

                if (isValidUser)
                {
                    string userPassword = PromptUserForPassword();

                    // Validate Password
                    bool isValidPassword = ValidateUserPassword(userName, userPassword);

                    if (isValidPassword)
                    {
                        // Show welcome message
                        DisplayLoginSuccessfulMessage(userName);
                        
                        // Show remaining balance
                        double userBalance = GetCurrentBalance(userName, userPassword);
                        DisplayCurrentBalance(userBalance);                        

                        // Show product list
                        while (true)
                        {
                            // Prompt for user input
                            DisplayProductList();

                            // Prompt for user input
                            string answer = PromptUserForNumber();
                            int productChoice = Convert.ToInt32(answer);
                            productChoice = productChoice - ZERO_INDEX_ADJUST; 

                            // Check if user entered number that equals product count
                            if (productChoice == EXIT_NUMBER)
                            {
                                UpdateUserBalance(userName, userPassword, userBalance);

                                // Write out new balance
                                string json = JsonConvert.SerializeObject(userList, Formatting.Indented);
                                File.WriteAllText(@"Data\Users.json", json);

                                // Write out new quantities
                                string json2 = JsonConvert.SerializeObject(productList, Formatting.Indented);
                                File.WriteAllText(@"Data\Products.json", json2);


                                // Prevent console from closing
                                PromptUserToExit();
                                return;
                            }
                            else
                            {
                                DisplayPurchasePlan(userBalance, productList[productChoice].Name);

                                // Prompt for user input
                                answer = PromptUserForPurchaseAmount();
                                int purchaseQuantity = Convert.ToInt32(answer);

                                // Check if balance - quantity * price is less than 0
                                if (userBalance - productList[productChoice].Price * purchaseQuantity < 0)
                                {
                                    DisplayNotEnoughMoney();
                                    continue;
                                }

                                // Check if quantity is less than quantity
                                if (productList[productChoice].Quantity <= purchaseQuantity)
                                {
                                    DisplayOutOfStock(productList[productChoice].Name);
                                    continue;
                                }

                                // Check if quantity is greater than zero
                                if (purchaseQuantity > 0)
                                {
                                    // Balance = Balance - Price * Quantity
                                    userBalance = userBalance - productList[productChoice].Price * purchaseQuantity;

                                    // Quanity = Quantity - Quantity
                                    productList[productChoice].Quantity = productList[productChoice].Quantity - purchaseQuantity;

                                    DisplayPurchaseSummary(productList[productChoice].Name, userBalance, purchaseQuantity);
                                }
                                else
                                {
                                    // Quantity is less than zero
                                    DisplayPurchasedCancelled();
                                }
                            }
                        }
                    }
                    else
                    {
                        // Invalid Password
                        DisplayInvalidPassword();

                        goto Login;
                    }
                }
                else
                {
                    // Invalid User
                    DisplayInvalidUser();

                    goto Login;
                }
            }

            // Prevent console from closing
            PromptUserToExit();
        }

        private static void UpdateUserBalance(string userName, string userPassword, double userBalance)
        {
            User user = FindUser(userName, userPassword);

            if( user != null )
            {
                user.Balance = userBalance;
            }
        }

        private static double GetCurrentBalance(string userName, string userPassword)
        {
            double userBalance = 0;

            User user = FindUser(userName, userPassword);

            if( user != null )
            {
                userBalance = user.Balance;
            }
            
            return userBalance;
        }

        private static bool ValidateUserPassword(string userName, string userPassword)
        {
            bool isValidPassword = false;
            
            if( FindUser(userName, userPassword) != null )
            {
                isValidPassword = true;
            }

            return isValidPassword;
        }

        private static bool ValidateUser(string userName)
        {
            bool isValidUser = false;

            if( FindUser(userName) != null )
            {
                isValidUser = true;
            }

            return isValidUser;
        }

        private static User FindUser(string userName, string userPassword)
        {
            User user = null;

            for (int i = 0; i < _userList.Count; i++)
            {
                if (_userList[i].Name == userName && _userList[i].Password == userPassword)
                {
                    user = _userList[i];
                }
            }

            return user;
        }

        private static User FindUser(string userName)
        {
            User user = null;

            for (int i = 0; i < _userList.Count; i++)
            {
                if (_userList[i].Name == userName)
                {
                    user = _userList[i];
                }
            }

            return user;
        }

        #region Display Messages

        private static void DisplayPurchasePlan(double userBalance,string productName)
        {
            Console.WriteLine();
            Console.WriteLine("You want to buy: " + productName);
            Console.WriteLine("Your balance is " + userBalance.ToString("C"));
        }

        private static void DisplayProductList()
        {
            Console.WriteLine();
            Console.WriteLine("What would you like to buy?");
            for (int i = 0; i < _productList.Count; i++)
            {
                Product product = _productList[i];
                Console.WriteLine(i + ZERO_INDEX_ADJUST + ": " + product.Name + " (" + product.Price.ToString("C") + ")");
            }
            Console.WriteLine(EXIT_NUMBER + ": Exit");
        }

        private static void DisplayPurchaseSummary(string productName, double userBalance, int purchaseQuantity)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("You bought " + purchaseQuantity + " " + productName);
            Console.WriteLine("Your new balance is " + userBalance.ToString("C"));
            Console.ResetColor();
        }

        private static void DisplayInvalidUser()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("You entered an invalid user.");
            Console.ResetColor();
        }

        private static void DisplayInvalidPassword()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("You entered an invalid password.");
            Console.ResetColor();
        }

        private static void DisplayPurchasedCancelled()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("Purchase cancelled");
            Console.ResetColor();
        }

        private static void DisplayOutOfStock(string productName)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("Sorry, " + productName + " is out of stock");
            Console.ResetColor();
        }

        private static void DisplayNotEnoughMoney()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("You do not have enough money to buy that.");
            Console.ResetColor();
        }

        private static void DisplayCurrentBalance(double userBalance)
        {
            Console.WriteLine();
            Console.WriteLine("Your balance is " + userBalance.ToString("C"));
        }

        private static void DisplayLoginSuccessfulMessage(string userName)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Login successful! Welcome " + userName + "!");
            Console.ResetColor();
        }

        private static void DisplayWelcomeMessage()
        {
            Console.WriteLine("Welcome to TUSC");
            Console.WriteLine("---------------");
        }

        #endregion

        #region Prompts for User Entry

        private static string PromptUserForUsername()
        {
            Console.WriteLine();
            return PromptUserForEntry("Enter Username:");
        }

        private static string PromptUserForPassword()
        {
            return PromptUserForEntry("Enter Password:");
        }

        private static string PromptUserForNumber()
        {
            return PromptUserForEntry("Enter a number:");
        }

        private static string PromptUserForPurchaseAmount()
        {
            return PromptUserForEntry("Enter amount to purchase:");
        }

        private static void PromptUserToExit()
        {
            Console.WriteLine();
            PromptUserForEntry("Press Enter key to exit");
        }

        private static string PromptUserForEntry(string displayText)
        {
            Console.WriteLine(displayText);
            return GetUserEntry();
        }

        private static string GetUserEntry()
        {
            return Console.ReadLine();
        }
       
        #endregion

    }
}
