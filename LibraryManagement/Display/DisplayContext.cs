using LibraryManagement.Controller;
using LibraryManagement.Manager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagement.Display
{
    public class DisplayContext
    {
        private readonly UserController _userController;
        private readonly BookController _bookController;
        private readonly BorrowController _borrowController;
        public DisplayContext(UserController userController, BookController bookController, BorrowController borrowController) 
        {
            _userController = userController;
            _bookController = bookController;
            _borrowController = borrowController;
        }
        public void MainRouter()
        {
            var resultLogin = _userController.Login();
            if (resultLogin)
            {
                MainMenu();
            }
        }
        public void MainMenu()
        {
            string role = AuthenticationManagement.GetAuthenticationRole();
            Console.WriteLine("\nC# Library Management Program");
            Console.WriteLine("****************************************MENU**********************************");
            if (role == "admin")
            {
                Console.WriteLine("** 1. Dashboard                                                              *");
                Console.WriteLine("** 2. User Management                                                        *");
            }
            if (role == "admin" || role == "librarian")
            {
                Console.WriteLine("** 3. Book Management                                                        *");

            }
            if (role == "librarian")
            {
                Console.WriteLine("** 4. Readership Statistics                                                  *");
            }
            if (role == "user")
            {
                Console.WriteLine("** 5. Search for books                                                       *");
                Console.WriteLine("** 6. Borrow books                                                           *");
                Console.WriteLine("** 7. Return books                                                           *");
                Console.WriteLine("** 8. View Borrowed Books Information                                        *");

            }
            Console.WriteLine("** 9. Profile                                                                *");
            Console.WriteLine("** 10. Logout                                                                *");
            Console.WriteLine("******************************************************************************");
            Console.WriteLine("Enter your select: ");
            int choice = int.Parse(Console.ReadLine());
            HandleUserChoice(choice);
        }
        private void HandleUserChoice(int choice)
        {
            string role = AuthenticationManagement.GetAuthenticationRole();
            if (role == "admin" || role == "librarian")
            {
                if (role == "admin")
                {
                    if (choice == 1)
                    {
                        _borrowController.DashBoard();
                        MainMenu();
                    }
                    if (choice == 2)
                    {
                        var subchoice = _userController.UserList("");
                        switch (subchoice)
                        {
                            case 0:
                                MainMenu();
                                break;
                            case 1:
                                _userController.NewUser();
                                _userController.UserList("");
                                break;
                            case 2:
                                _userController.UserChange();
                                _userController.UserList("");
                                break;
                            case 3:
                                _userController.UserDelete();
                                _userController.UserList("");
                                break;
                            case 4:
                                string keyword = _userController.Search();
                                _userController.UserList(keyword);
                                break;
                        }
                    }
                }
                if (role == "librarian")
                {
                    if (choice == 4)
                    {
                        var subchoice = _borrowController.BorrowList("","");
                        switch (subchoice)
                        {
                            case 0:
                                MainMenu();
                                break;
                            case 1:
                                Console.Write("Enter your key word: ");
                                string keywordSearch = Console.ReadLine();
                                Console.WriteLine("Enter status: 1. Borrowed | 2. Returned | 3. All");
                                List<string> statuListSelect = new List<string> { "Borrowed", "Returned", "" };
                                string statuSelect = SelectStatus(statuListSelect);
                                _borrowController.BorrowList(keywordSearch, statuSelect);
                                break;
                        }
                    }
                }
                if (choice == 3)
                {
                    var subchoice = _bookController.BookList("");
                    switch (subchoice)
                    {
                        case 0:
                            MainMenu();
                            break;
                        case 1:
                            _bookController.NewBook();
                            _bookController.BookList("");
                            break;
                        case 2:
                            _bookController.BookChange();
                            _bookController.BookList("");
                            break;
                        case 3:
                            _bookController.BookDelete();
                            _bookController.BookList("");
                            break;
                        case 4:
                            Console.WriteLine("************************************SEARCH BOOK*******************************");
                            Console.WriteLine("Enter keyword: ");
                            string keyword = Console.ReadLine();
                            Console.WriteLine("******************************************************************************");
                            _bookController.BookList(keyword);
                            break;
                    }
                }
            }
            else if (role == "user")
            {
                if (choice == 5)
                {
                    var subchoice = _bookController.SearchForBooks("");
                    switch (subchoice)
                    {
                        case 0:
                            MainMenu();
                            break;
                        case 1:
                            Console.Write("Enter your key word: ");
                            string keywordUserName = Console.ReadLine();
                            _bookController.SearchForBooks(keywordUserName.Trim());
                            break;
                    }
                }
                else if (choice == 6)
                {
                    _bookController.BookBorrow();
                    MainMenu();
                }
                else if (choice == 7)
                {
                    _bookController.BookReturn();
                    MainMenu();
                }
                else if (choice == 8)
                {
                    var subchoice = _bookController.ViewBorrowedBooksInformation("");
                    switch (subchoice)
                    {
                        case 0:
                            MainMenu();
                            break;
                        case 1:
                            Console.Write("Enter your key word: ");
                            string keywordSearch = Console.ReadLine();
                            _bookController.ViewBorrowedBooksInformation(keywordSearch.Trim());
                            break;
                    }
                }
            }
            if (choice == 9)
            {
                var subchoice = _userController.Profile();
                switch (subchoice)
                {
                    case 0:
                        MainMenu();
                        break;
                    case 1:
                        _userController.ChangePassword();
                        break;
                    default:
                        MainMenu();
                        break;
                }
            }
            else if (choice == 10)
            {
                AuthenticationManagement.Logout();
                MainRouter();
            }
            else
            {
                Console.WriteLine("Invalid choice. Please select a valid option.");
                MainMenu();
            }
        }
        public static int SelectMenuItem()
        {
            string select;
            int result;
            Console.WriteLine("Enter your select: ");
            select = Console.ReadLine();
            if (int.TryParse(select, out result))
            {
                return result;
            }
            else
            {
                Console.WriteLine("Incorrect selection. Please select again.");
                return SelectMenuItem();
            }
        }
        public static string SelectStatus(List<string> statuses)
        {
            if (statuses == null || statuses.Count == 0)
            {
                throw new ArgumentException("Status list cannot be null or empty.");
            }

            Console.WriteLine("Please select a option:");
            for (int i = 0; i < statuses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {(string.IsNullOrEmpty(statuses[i]) ? "All" : statuses[i])}");
            }

            int selection = -1;
            while (selection < 1 || selection > statuses.Count)
            {
                Console.Write("Enter the number of your choice: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out selection) && selection >= 1 && selection <= statuses.Count)
                {
                    return statuses[selection - 1];
                }

                Console.WriteLine("Invalid choice. Please try again.");
            }

            // This line is unreachable but needed to satisfy compiler
            return null;
        }
        public static void TableContent<T>(List<T> contents, List<string> excludedFields)
        {
            if (contents == null || contents.Count == 0)
            {
                Console.WriteLine("Không có dữ liệu để hiển thị.");
                return;
            }

            var properties = typeof(T).GetProperties()
                                       .Where(p => !excludedFields.Contains(p.Name))
                                       .ToArray();

            if (properties.Length == 0)
            {
                Console.WriteLine("Tất cả các trường đã bị loại trừ. Không có gì để hiển thị.");
                return;
            }

            // Tính độ rộng lớn nhất của mỗi cột
            List<int> columnWidths = new List<int>();

            foreach (var property in properties)
            {
                int maxWidth = Math.Max(property.Name.Length, contents.Max(item => property.GetValue(item)?.ToString().Length ?? 0));
                columnWidths.Add(maxWidth);
            }

            // In ra tiêu đề cột
            for (int i = 0; i < properties.Length; i++)
            {
                Console.Write(properties[i].Name.PadRight(columnWidths[i] + 2));
            }
            Console.WriteLine();

            // In ra các dòng trong bảng
            foreach (var item in contents)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    var value = properties[i].GetValue(item)?.ToString() ?? string.Empty;
                    Console.Write(value.PadRight(columnWidths[i] + 2));
                }
                Console.WriteLine();
            }
        }
    }
}
