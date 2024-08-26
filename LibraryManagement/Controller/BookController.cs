using LibraryManagement.Display;
using LibraryManagement.Manager;
using LibraryManagement.Model;
using LibraryManagement.Repository;
using LibraryManagement.Repository.Impl;
using System;
using System.Collections.Generic;
using static Mysqlx.Datatypes.Scalar.Types;


namespace LibraryManagement.Controller
{
    public class BookController
    {
        private readonly BookRepository _bookRepository;
        private readonly BorrowRepository _borrowRepository;
        public BookController(BookRepository bookRepository, BorrowRepository borrowRepository) 
        {
            _bookRepository = bookRepository;
            _borrowRepository = borrowRepository;
        }
        public int BookList(string keyword)
        {
            Console.WriteLine("************************************BOOK MANAGEMENT***************************");
            Console.WriteLine("** 1. Create Book                                                            *");
            Console.WriteLine("** 2. Edit Book                                                              *");
            Console.WriteLine("** 3. Delete Book                                                            *");
            Console.WriteLine("** 4. Search Book                                                            *");
            Console.WriteLine("** 0. Main Menu                                                              *");
            Console.WriteLine("******************************************************************************");
            Console.WriteLine();
            var books = _bookRepository.FindAllBookLikeTitle(keyword);
            DisplayContext.TableContent(books, new List<string>());
            int choice = DisplayContext.SelectMenuItem();
            return choice;
        }
        public void NewBook()
        {
            Console.WriteLine("************************************CREATE BOOK*******************************");
            Console.WriteLine("Enter title: ");
            string title = Console.ReadLine();
            Console.WriteLine("Enter category: ");
            string category = Console.ReadLine();
            Console.WriteLine("Enter author: ");
            string author = Console.ReadLine();
            Console.WriteLine("Enter publisher: ");
            string publisher = Console.ReadLine();
            if (
                 string.IsNullOrEmpty(title)
                || string.IsNullOrEmpty(category)
                || string.IsNullOrEmpty(author)
                || string.IsNullOrEmpty(publisher)
                )
            {
                Console.WriteLine("Invalid data");
            }
            else
            {
                Book book = new Book
                { 
                    Title = title,
                    Author = author,
                    Publisher = publisher,
                    Category = category,
                    Status = "Activated"

                };
                var result = _bookRepository.InsertBook(book);
                if (result)
                {
                    Console.WriteLine("Create Successfully!");
                }
                else
                {
                    Console.WriteLine("Create Fail");
                }
            }
            Console.WriteLine("******************************************************************************");
        }
        public void BookChange()
        {
            Console.WriteLine("**************************************EDIT BOOK*******************************");
            Console.WriteLine("Enter id: ");
            string id = Console.ReadLine();
            Console.WriteLine("Enter title: ");
            string title = Console.ReadLine();
            Console.WriteLine("Enter category: ");
            string category = Console.ReadLine();
            Console.WriteLine("Enter author: ");
            string author = Console.ReadLine();
            Console.WriteLine("Enter publisher: ");
            string publisher = Console.ReadLine();
            Console.WriteLine("Enter status: ");
            List<string> statusSelect = new List<string> { "Activated", "Deactivated" };
            string status = DisplayContext.SelectStatus(statusSelect);
            if (string.IsNullOrEmpty(id)
                || string.IsNullOrEmpty(title)
                || string.IsNullOrEmpty(category)
                || string.IsNullOrEmpty(author)
                || string.IsNullOrEmpty(publisher)
                || string.IsNullOrEmpty(status))
            {
                Console.WriteLine("Invalid data");
            }
            else
            {
                if (int.TryParse(id.Trim(), out int bookId))
                {
                    Book book = new Book
                    {
                        Id = bookId,
                        Title = title,
                        Author = author,
                        Publisher = publisher,
                        Category = category,
                        Status = status,
                    };
                    var result = _bookRepository.EditBook(book);
                    if (result)
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
                    Console.WriteLine($"Invalid ID entered: {id}");
                }
               
            }
            Console.WriteLine("******************************************************************************");
        }
        public void BookDelete()
        {
            Console.WriteLine("************************************DELETE BOOK*******************************");
            Console.WriteLine("Enter id: ");
            string id = Console.ReadLine();
            if (string.IsNullOrEmpty(id))
            {
                Console.WriteLine("Invalid Id");
            }
            else
            {
                if (int.TryParse(id.Trim(), out int bookId))
                {
                    var result = _bookRepository.DeleteBookById(id);
                    if (result)
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
                    Console.WriteLine($"Invalid ID entered: {id}");
                }
            }
        }
        public int SearchForBooks(string keyword)
        {
            Console.WriteLine("************************************SEARCH FOR BOOKS**************************");
            Console.WriteLine("** 1. Search Book                                                            *");
            Console.WriteLine("** 0. Main Menu                                                              *");
            Console.WriteLine("******************************************************************************");
            Console.WriteLine();
            var books = _bookRepository.FindAllBookLikeTitle(keyword);
            DisplayContext.TableContent(books, new List<string>());
            int choice = DisplayContext.SelectMenuItem();
            return choice;
        }
        public void BookReturn()
        {
            Console.WriteLine("************************************RETURN BOOKS*****************************");
            Console.WriteLine("Please enter the IDs of the book loan application you want to borrow, separated by commas:");
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid Data");
            }
            else
            {
                string[] idStrings = input.Split(',');
                List<int> borrowIds = new List<int>();

                foreach (string idString in idStrings)
                {
                    if (int.TryParse(idString.Trim(), out int id))
                    {
                        borrowIds.Add(id);
                    }
                    else
                    {
                        Console.WriteLine($"Invalid ID entered: {idString}");
                    }
                }
                if (borrowIds.Count > 0)
                {
                    var resultBorrow = _borrowRepository.UpdateStatusToReturned(borrowIds);
                    if (resultBorrow)
                    {
                        Console.WriteLine("Books have been successfully returned.");
                    }
                    else
                    {
                        Console.WriteLine("No books were returned. Check if the books were borrowed.");
                    }
                }
            }
        }
        public void BookBorrow()
        {
            Console.WriteLine("************************************BORROW BOOKS*****************************");
            Console.WriteLine("Please enter the IDs of the books you want to borrow, separated by commas:");

            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid Data");
            }
            else
            {
                string[] idStrings = input.Split(',');
                List<int> bookIds = new List<int>();

                foreach (string idString in idStrings)
                {
                    if (int.TryParse(idString.Trim(), out int id))
                    {
                        bookIds.Add(id);
                    }
                    else
                    {
                        Console.WriteLine($"Invalid ID entered: {idString}");
                    }
                }
                if (bookIds.Count > 0)
                {
                    var resultBorrow = _borrowRepository.InsertBatchBorrow(bookIds);
                    if(resultBorrow)
                    {
                        Console.WriteLine("Books have been successfully borrowed.");
                    } else
                    {
                        Console.WriteLine("Books have been fail borrowed.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID");
                }
            }
        }
        public int ViewBorrowedBooksInformation(string keyword)
        {
            Console.WriteLine("**********************************BORROW MANAGEMENT***************************");
            Console.WriteLine("** 1. Search Borrow                                                          *");
            Console.WriteLine("** 0. Main Menu                                                              *");
            Console.WriteLine("******************************************************************************");
            Console.WriteLine();
            int userId = AuthenticationManagement.GetAuthenticationId();
            var borrows = _borrowRepository.FindAllBorrowByUserIdLikeTitle(userId, keyword);
            DisplayContext.TableContent(borrows, new List<string> { "UserId" });
            int choice = DisplayContext.SelectMenuItem();
            return choice;
        }
    }
}
