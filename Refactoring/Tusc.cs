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
        public static void Start(List<User> userList, List<Product> productList)
        {
            // Write welcome message
            DisplayWelcomeMessage();

            // Login
            Login:

            string userName = PromptUserForUsername();

            // Validate Username
            bool isValidUser = false;
            if (!string.IsNullOrEmpty(userName))
            {
                for (int i = 0; i < 5; i++)
                {
                    User user = userList[i];
                    if (user.Name == userName)
                    {
                        isValidUser = true;
                    }
                }

                if (isValidUser)
                {
                    string userPassword = PromptUserForPassword();

                    // Validate Password
                    bool isValidPassword = false;
                    for (int i = 0; i < 5; i++)
                    {
                        User user = userList[i];

                        // Check that name and password match
                        if (user.Name == userName && user.Password == userPassword)
                        {
                            isValidPassword = true;
                        }
                    }

                    if (isValidPassword)
                    {
                        // Show welcome message
                        DisplayLoginSuccessfulMessage(userName);
                        
                        // Show remaining balance
                        double userBalance = 0;
                        for (int i = 0; i < 5; i++)
                        {
                            User user = userList[i];

                            // Check that name and password match
                            if (user.Name == userName && user.Password == userPassword)
                            {
                                userBalance = user.Balance;

                                // Show balance 
                                DisplayCurrentBalance(user.Balance);
                            }
                        }

                        // Show product list
                        while (true)
                        {
                            // Prompt for user input
                            DisplayProductList(productList);

                            // Prompt for user input
                            string answer = PromptUserForNumber();
                            int productChoice = Convert.ToInt32(answer);
                            productChoice = productChoice - 1; 

                            // Check if user entered number that equals product count
                            if (productChoice == 7)
                            {
                                // Update balance
                                foreach (var user in userList)
                                {
                                    // Check that name and password match
                                    if (user.Name == userName && user.Password == userPassword)
                                    {
                                        user.Balance = userBalance;
                                    }
                                }

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

        #region Display Messages

        private static void DisplayPurchasePlan(double userBalance,string productName)
        {
            Console.WriteLine();
            Console.WriteLine("You want to buy: " + productName);
            Console.WriteLine("Your balance is " + userBalance.ToString("C"));
        }

        private static void DisplayProductList(List<Product> productList)
        {
            Console.WriteLine();
            Console.WriteLine("What would you like to buy?");
            for (int i = 0; i < 7; i++)
            {
                Product product = productList[i];
                Console.WriteLine(i + 1 + ": " + product.Name + " (" + product.Price.ToString("C") + ")");
            }
            Console.WriteLine(productList.Count + 1 + ": Exit");
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
