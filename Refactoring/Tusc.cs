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
            Console.WriteLine("Welcome to TUSC");
            Console.WriteLine("---------------");

            // Login
            Login:

            // Prompt for user input
            Console.WriteLine();
            Console.WriteLine("Enter Username:");
            string userName = GetUserEntry();

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
                    // Prompt for user input
                    Console.WriteLine("Enter Password:");
                    string userPassword = GetUserEntry();

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
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine();
                        Console.WriteLine("Login successful! Welcome " + userName + "!");
                        Console.ResetColor();
                        
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
                                Console.WriteLine();
                                Console.WriteLine("Your balance is " + user.Balance.ToString("C"));
                            }
                        }

                        // Show product list
                        while (true)
                        {
                            // Prompt for user input
                            Console.WriteLine();
                            Console.WriteLine("What would you like to buy?");
                            for (int i = 0; i < 7; i++)
                            {
                                Product product = productList[i];
                                Console.WriteLine(i + 1 + ": " + product.Name + " (" + product.Price.ToString("C") + ")");
                            }
                            Console.WriteLine(productList.Count + 1 + ": Exit");

                            // Prompt for user input
                            Console.WriteLine("Enter a number:");
                            string answer = GetUserEntry();
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
                                Console.WriteLine();
                                Console.WriteLine("Press Enter key to exit");
                                GetUserEntry();
                                return;
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.WriteLine("You want to buy: " + productList[productChoice].Name);
                                Console.WriteLine("Your balance is " + userBalance.ToString("C"));

                                // Prompt for user input
                                Console.WriteLine("Enter amount to purchase:");
                                answer = GetUserEntry();
                                int purchaseQuantity = Convert.ToInt32(answer);

                                // Check if balance - quantity * price is less than 0
                                if (userBalance - productList[productChoice].Price * purchaseQuantity < 0)
                                {
                                    Console.Clear();
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine();
                                    Console.WriteLine("You do not have enough money to buy that.");
                                    Console.ResetColor();
                                    continue;
                                }

                                // Check if quantity is less than quantity
                                if (productList[productChoice].Qty <= purchaseQuantity)
                                {
                                    Console.Clear();
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine();
                                    Console.WriteLine("Sorry, " + productList[productChoice].Name + " is out of stock");
                                    Console.ResetColor();
                                    continue;
                                }

                                // Check if quantity is greater than zero
                                if (purchaseQuantity > 0)
                                {
                                    // Balance = Balance - Price * Quantity
                                    userBalance = userBalance - productList[productChoice].Price * purchaseQuantity;

                                    // Quanity = Quantity - Quantity
                                    productList[productChoice].Qty = productList[productChoice].Qty - purchaseQuantity;

                                    Console.Clear();
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("You bought " + purchaseQuantity + " " + productList[productChoice].Name);
                                    Console.WriteLine("Your new balance is " + userBalance.ToString("C"));
                                    Console.ResetColor();
                                }
                                else
                                {
                                    // Quantity is less than zero
                                    Console.Clear();
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine();
                                    Console.WriteLine("Purchase cancelled");
                                    Console.ResetColor();
                                }
                            }
                        }
                    }
                    else
                    {
                        // Invalid Password
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine();
                        Console.WriteLine("You entered an invalid password.");
                        Console.ResetColor();

                        goto Login;
                    }
                }
                else
                {
                    // Invalid User
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine("You entered an invalid user.");
                    Console.ResetColor();

                    goto Login;
                }
            }

            // Prevent console from closing
            Console.WriteLine();
            Console.WriteLine("Press Enter key to exit");
            GetUserEntry();
        }

        private static string GetUserEntry()
        {
            return Console.ReadLine();
        }
    }
}
