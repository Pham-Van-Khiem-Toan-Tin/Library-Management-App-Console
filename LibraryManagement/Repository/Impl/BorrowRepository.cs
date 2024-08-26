using LibraryManagement.Config;
using LibraryManagement.Manager;
using LibraryManagement.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Repository.Impl
{
    public class BorrowRepository : IBorrowRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public BorrowRepository(ApplicationDBContext applicationDBContext) 
        {
            _dbContext = applicationDBContext;
        }
        public List<Borrow> FindNumberOfBorrowByDay()
        {
            List<Borrow> borrows = new List<Borrow>();
            DateTime today = DateTime.Now;
            DateTime sevenDaysAgo = today.AddDays(-7);
            string query = "SELECT * FROM borrowrecords WHERE borrowDate >= @sevenDaysAgo AND borrowDate <= @today";
            try
            {
                _dbContext.OpenConnection();
                using (var cmd = _dbContext.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@sevenDaysAgo", sevenDaysAgo);
                    cmd.Parameters.AddWithValue("@today", today);
                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            Borrow borrow = new Borrow
                            {
                                Id = reader.GetInt32(0),
                                BorrowDate = reader.GetDateTime("borrowDate").ToString("dd-MM-yyyy"),
                            };
                            borrows.Add(borrow);
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
            return borrows;
        }
        public List<Borrow> FindAllBorrowLikeStatusAndTitle(string bookTitle, string status)
        {
            List<Borrow> borrows = new List<Borrow>();
            string query = "SELECT br.id, br.userId, br.bookId, b.title AS title, br.borrowDate, br.returnDate, br.status FROM borrowrecords br JOIN books b ON br.bookId = b.id WHERE br.status LIKE @Status AND b.title LIKE @Title";
            try
            {
                _dbContext.OpenConnection();
                using (var cmd = _dbContext.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@Status", "%" + status + "%");
                    cmd.Parameters.AddWithValue("@Title", "%" + bookTitle + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            Borrow borrow = new Borrow
                            {
                                Id = reader.GetInt32("id"),
                                BookId = reader.GetInt32("bookId"),
                                UserId = reader.GetInt32("userId"),
                                BookTitle = reader.GetString("title"),
                                BorrowDate = reader.GetDateTime("borrowDate").ToString("dd-MM-yyyy"),
                                ReturnDate = reader.IsDBNull(reader.GetOrdinal("returnDate"))
                                              ? "N/A"
                                              : reader.GetDateTime("returnDate").ToString("dd-MM-yyyy"),
                                Status = reader.GetString("status")
                            };
                            borrows.Add(borrow);
                        }

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
            return borrows;
        }
        public List<Borrow> FindAllBorrowByUserIdLikeTitle(int userId, string title)
        {
            List<Borrow> borrows = new List<Borrow>();
            string query = "SELECT br.id, br.bookId, b.title AS title, br.borrowDate, br.returnDate, br.status FROM borrowrecords br JOIN books b ON br.bookId = b.id WHERE br.userId = @UserId AND b.title LIKE @Title";
            try
            {
                _dbContext.OpenConnection();
                using (var cmd = _dbContext.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Title", "%" + title + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Borrow borrow = new Borrow
                            {
                                Id = reader.GetInt32("id"),
                                BookId = reader.GetInt32("bookId"),
                                BookTitle = reader.GetString("title"),
                                BorrowDate = reader.GetDateTime("borrowDate").ToString("dd-MM-yyyy"),
                                ReturnDate = reader.IsDBNull(reader.GetOrdinal("returnDate"))
                                             ? "N/A"
                                             : reader.GetDateTime("returnDate").ToString("dd-MM-yyyy"),
                                Status = reader.GetString("status")
                            };
                            borrows.Add(borrow);
                        }

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
            return borrows;
        }
        public bool UpdateStatusToReturned(List<int> bookIds)
        {
            bool result = false;
            MySqlTransaction transaction = null;
            try
            {
                _dbContext.OpenConnection();
                MySqlConnection conn = _dbContext.Connection;
                transaction = conn.BeginTransaction();

                // Tạo câu lệnh UPDATE với nhiều giá trị
                var sb = new StringBuilder("UPDATE borrowrecords SET status = @Status, returnDate = @ReturnDate WHERE id IN (");
                var parameters = new List<MySqlParameter>();

                // Xây dựng câu lệnh SQL với nhiều tham số
                for (int i = 0; i < bookIds.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append($"@Id{i}");
                    parameters.Add(new MySqlParameter($"@Id{i}", bookIds[i]));
                }
                sb.Append(") AND status = 'Borrowed';"); // Chỉ cập nhật những đơn sách có trạng thái 'Borrowed'

                // Thêm tham số trạng thái
                parameters.Add(new MySqlParameter("@Status", "Returned"));
                parameters.Add(new MySqlParameter("@ReturnDate", DateTime.Now));

                using (var cmd = new MySqlCommand(sb.ToString(), conn, transaction))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result = true;
                    }
                }

                // Commit giao dịch nếu không có lỗi
                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                try
                {
                    transaction?.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine($"Rollback error: {rollbackEx.Message}");
                }
            }
            finally
            {
                _dbContext.CloseConnection();
            }
            return result;
        }
        public bool InsertBatchBorrow(List<int> bookIds)
        {
            int userId = AuthenticationManagement.GetAuthenticationId();
            bool result = false;
            MySqlTransaction transaction = null;
            try
            {
                _dbContext.OpenConnection();
                MySqlConnection conn = _dbContext.Connection;
                transaction = conn.BeginTransaction();

                // Tạo câu lệnh UPDATE với nhiều giá trị
                var sb = new StringBuilder("INSERT INTO borrowrecords (userId, bookId, borrowDate, status) VALUES ");
                var parameters = new List<MySqlParameter>();

                // Xây dựng câu lệnh SQL với nhiều tham số
                for (int i = 0; i < bookIds.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append($"(@UserId{i}, @BookId{i}, @BorrowDate{i}, @Status{i})");
                    parameters.Add(new MySqlParameter($"@UserId{i}", userId));
                    parameters.Add(new MySqlParameter($"@BookId{i}", bookIds[i]));
                    parameters.Add(new MySqlParameter($"@BorrowDate{i}", DateTime.Now));
                    parameters.Add(new MySqlParameter($"@Status{i}", "Borrowed"));
                }

                using (var cmd = new MySqlCommand(sb.ToString(), conn, transaction))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result = true;
                    }
                }

                // Commit giao dịch nếu không có lỗi
                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                try
                {
                    transaction?.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine($"Rollback error: {rollbackEx.Message}");
                }
            }
            finally
            {
                _dbContext.CloseConnection();
            }
            return result;
        }

    }
}
