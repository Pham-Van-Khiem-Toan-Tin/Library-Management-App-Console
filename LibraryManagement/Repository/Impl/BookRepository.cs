using LibraryManagement.Config;
using LibraryManagement.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public BookRepository(ApplicationDBContext applicationDBContext) 
        {
            _dbContext = applicationDBContext;
        }
        public List<Book> FindAllBookLikeTitle(string title)
        {
            List<Book> books = new List<Book>();
            string query = "SELECT * FROM books WHERE title LIKE @keyword";
            try
            {
                _dbContext.OpenConnection();
                using (var cmd = _dbContext.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@keyword", "%" + title + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Book book = new Book
                            {
                                Id = reader.GetInt32("id"),
                                Title = reader.GetString("title"),
                                Author = reader.GetString("author"),
                                Publisher = reader.GetString("publisher"),
                                Category = reader.GetString("category"),
                                Status = reader.GetString("status")
                            };
                            books.Add(book);
                        }

                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            } finally
            {
                _dbContext.CloseConnection();
            }
            return books;
        }
        public bool DeleteBookById(string id)
        {
            bool result = false;
            string query = "DELETE FROM books WHERE id = @id";
            try
            {
                _dbContext.OpenConnection();
                using (var cmd = _dbContext.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    int rowAffected = cmd.ExecuteNonQuery();
                    if (rowAffected > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _dbContext.CloseConnection();
            }
            return result;
        }
        public bool EditBook(Book book)
        {
            bool result = false;
            string query = "UPDATE books SET title = @title, author = @author, publisher = @publisher, category = @category, status = @status WHERE id = @id";
            try
            {
                _dbContext.OpenConnection();
                using (var cmd = _dbContext.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@title", book.Title);
                    cmd.Parameters.AddWithValue("@author", book.Author);
                    cmd.Parameters.AddWithValue("@publisher", book.Publisher);
                    cmd.Parameters.AddWithValue("@category", book.Category);
                    cmd.Parameters.AddWithValue("@status", book.Status);
                    cmd.Parameters.AddWithValue("@id", book.Id);
                    int rowAffected = cmd.ExecuteNonQuery();
                    if (rowAffected > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _dbContext.CloseConnection();
            }
            return result;
        }
        public bool InsertBook(Book book)
        {
            bool result = false;
            string query = "INSERT INTO books (title, author, publisher, category, status) VALUES (@title, @author, @publisher, @category, @status)";
            try
            {
                _dbContext.OpenConnection();
                using (var cmd = _dbContext.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@title", book.Title);
                    cmd.Parameters.AddWithValue("@author", book.Author);
                    cmd.Parameters.AddWithValue("@publisher", book.Publisher);
                    cmd.Parameters.AddWithValue("@category", book.Category);
                    cmd.Parameters.AddWithValue("@status", "Activated");
                    int rowAffected = cmd.ExecuteNonQuery();
                    if (rowAffected > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _dbContext.CloseConnection();
            }
            return result;
        }
    }
}
