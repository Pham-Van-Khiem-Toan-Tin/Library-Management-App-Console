using LibraryManagement.Display;
using LibraryManagement.Manager;
using LibraryManagement.Model;
using LibraryManagement.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Controller
{
    public class UserController
    {
        private readonly UserRepository _userRepository;
        public UserController(UserRepository userRepository) 
        { 
            _userRepository = userRepository;
        }
        public bool Login()
        {
            Console.WriteLine("Welcome to the Library Management System");

            Console.WriteLine("************************************LOGIN*************************************");
            for (int attempts = 0; attempts < 4; attempts++)
            {
                string inputUsername = string.Empty;
                string inputPassword = string.Empty;
                while (string.IsNullOrEmpty(inputUsername) || string.IsNullOrEmpty(inputPassword))
                {
                    Console.Write("Enter username: ");
                    inputUsername = Console.ReadLine();

                    Console.Write("Enter password: ");
                    inputPassword = ReadPassword();
                }
                var user = _userRepository.FindUserByUsernameAndPassword(inputUsername, inputPassword);
                if (user != null)
                {
                    Console.WriteLine("\nLogin successful!");
                    Console.WriteLine("******************************************************************************");
                    AuthenticationManagement.SetAuthenticationUser(user);
                    return true;
                }
                else
                {
                    Console.WriteLine("\nInvalid username or password. Try again.");
                }
                if (attempts == 3)
                {
                    Console.WriteLine("Maximum login attempts exceeded. Exiting program.");
                    Environment.Exit(0);
                }
            }
            return false;
        }
        public int Profile()
        {
            Console.WriteLine("************************************PROFILE***********************************");
            Console.WriteLine("** 1. Change Password                                                        *");
            Console.WriteLine("** 0. Main Menu                                                              *");
            Console.WriteLine("******************************************************************************");
            int userId = AuthenticationManagement.GetAuthenticationId();
            var user = _userRepository.FindUserById(userId);
            List<User> users = new List<User>();
            users.Add(user);
            
            DisplayContext.TableContent(users, new List<string>());
            int choice = DisplayContext.SelectMenuItem();
            return choice;
        }
        public void ChangePassword()
        {
            Console.WriteLine("********************************CHANGE PASSWORD*******************************");
            Console.Write("Enter new password: ");
            string newPass = ReadPassword();
            Console.Write("\nEnter confirm password");
            string confirmPass = ReadPassword();
            if (string.IsNullOrEmpty(newPass)
                || string.IsNullOrEmpty(confirmPass))
            {
                Console.WriteLine("\nInvalid data");
            }
            else
            {
                if (newPass != confirmPass)
                {
                    Console.WriteLine("\nPassword does not match");
                } else
                {
                    var resultPass = AuthenticationManagement.UpdatePasswordOfUser(newPass);
                    if (resultPass)
                    {
                        Console.WriteLine("\nChange password successfully!");
                    }
                    else
                    {
                        Console.WriteLine("\nChange password fail");
                    }
                }
            }
            Console.WriteLine("******************************************************************************");

        }
        public int UserList(string keyword)
        {
            Console.WriteLine("************************************USER MANAGEMENT***************************");
            Console.WriteLine("** 1. Create User                                                            *");
            Console.WriteLine("** 2. Edit User                                                              *");
            Console.WriteLine("** 3. Delete User                                                            *");
            Console.WriteLine("** 4. Search User                                                            *");
            Console.WriteLine("** 0. Main Menu                                                              *");
            Console.WriteLine("******************************************************************************");
            Console.WriteLine();
            var users = _userRepository.FindAllUserLikeUserName(keyword);
            DisplayContext.TableContent(users, new List<string>());
            int choice = DisplayContext.SelectMenuItem();
            return choice;
        }
        public void NewUser()
        {
            Console.WriteLine("************************************CREATE USER*******************************");
            Console.Write("Enter name: ");
            string name = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = ReadPassword();
            Console.Write("\nEnter confirm password: ");
            string confirmPass = ReadPassword();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPass))
            {
                Console.WriteLine("Invalid data");
            } else
            {
                if (password != confirmPass)
                {
                    Console.WriteLine("Password does not match");
                }
                else
                {
                    var createResult = _userRepository.InsertUser(name, password);
                    if (createResult)
                    {
                        Console.WriteLine("\nCreate Successfully!");
                    }
                    else
                    {
                        Console.WriteLine("\nCreate Fail");
                    }
                }
            }
            Console.WriteLine("******************************************************************************");
        }
        public void UserChange()
        {
            Console.WriteLine("************************************EDIT USER*********************************");
            Console.Write("Enter id: ");
            string id = Console.ReadLine();
            Console.Write("\nEnter name: ");
            string name = Console.ReadLine();
            Console.Write("\nEnter role: ");
            List<string> roles = new List<string> { "admin", "user", "librarian" };
            string role = DisplayContext.SelectStatus(roles);
            Console.WriteLine("******************************************************************************");
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(role))
            {
                Console.WriteLine("Invalid data");
            }
            else
            {
                User user = null;
                if (int.TryParse(id, out int userId))
                {
                    user = new User
                    {
                        Id = userId,
                        UserName = name,
                        Role = role
                    };
                    var editResult = _userRepository.UpdateUser(user);
                    if (editResult)
                    {
                        Console.WriteLine("Edit Successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Edit Fail");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid user ID format.");
                }
                
            }
        }
        public void UserDelete()
        {
            Console.WriteLine("************************************DELETE USER*******************************");
            Console.Write("Enter id: ");
            string id = Console.ReadLine();
            if (string.IsNullOrEmpty(id))
            {
                Console.WriteLine("Invalid data");
            }
            else
            {
                if (int.TryParse(id, out int userId))
                {
                    var resultDelete = _userRepository.DeleteUserById(userId);
                    if (resultDelete)
                    {
                        Console.WriteLine("Delete Successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Delete Fail");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid user ID format.");
                }
            }
            Console.WriteLine("******************************************************************************");
        }
        public string Search()
        {
            Console.WriteLine("************************************SEARCH USER*******************************");
            Console.WriteLine("Enter keyword: ");
            string keyword = Console.ReadLine();
            Console.WriteLine("******************************************************************************");
            return keyword.Trim();
        }
        private string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1); // Using Substring method
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            return password;
        }

    }
}
