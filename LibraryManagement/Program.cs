using LibraryManagement.Config;
using LibraryManagement.Controller;
using LibraryManagement.Display;
using LibraryManagement.Repository;
using LibraryManagement.Repository.Impl;
using System;



namespace LibraryManagement
{
    internal class Program
    {
        static string connectionString = "Server=localhost;Database=qltv;userid=root;password=271201;Port=3306";
        static void Main(string[] args)
        {
            ApplicationDBContext applicationDBContext = new ApplicationDBContext(connectionString);
            UserRepository userRepository = new UserRepository(applicationDBContext);
            UserController userController = new UserController(userRepository);
            BookRepository bookRepository = new BookRepository(applicationDBContext);
            BorrowRepository borrowRepository = new BorrowRepository(applicationDBContext);
            BookController bookController = new BookController(bookRepository, borrowRepository);
            BorrowController borrowController = new BorrowController(borrowRepository);
            DisplayContext displayContext = new DisplayContext(userController, bookController, borrowController);
            displayContext.MainRouter();
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        
    }
}
