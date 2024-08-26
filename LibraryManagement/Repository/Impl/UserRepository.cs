using LibraryManagement.Config;
using LibraryManagement.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LibraryManagement.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public UserRepository(ApplicationDBContext applicationDBContext) 
        {
            _dbContext = applicationDBContext;
        }
        public User FindUserByUsernameAndPassword(string username, string password)
        {
            User user = null;
            string query = "SELECT * FROM users WHERE username = @Username AND password = @Password";
            try
            {
                _dbContext.OpenConnection();
                using (var cmd = _dbContext.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Id = reader.GetInt32("id"),
                                Role = reader.GetString("role")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            } finally
            {
                _dbContext.CloseConnection();
            }
            return user;
        }
        public User FindUserById(int id)
        {
            User user = null;
            string query = "SELECT * FROM users WHERE id = @Id";
            try
            {
                _dbContext.OpenConnection();
                using (var cmd = _dbContext.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Id = reader.GetInt32("id"),
                                UserName = reader.GetString("username"),
                                Role = reader.GetString("role")
                            };

                        }
                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            } finally { _dbContext.CloseConnection(); }
            return user;
        }
        public List<User> FindAllUserLikeUserName(string username)
        {
            List<User> users = new List<User>();
            string query = "SELECT * FROM users WHERE username LIKE @username";
            try
            {
                _dbContext.OpenConnection();
                using (var cmd = _dbContext.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@username", "%" + username + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User
                            {
                                Id = reader.GetInt32("id"),
                                UserName = reader.GetString("username"),
                                Role = reader.GetString("role")
                            };
                            users.Add(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { _dbContext.CloseConnection(); }
            return users;
        }
        public bool DeleteUserById(int id)
        {
            bool result = false;
            string query = "DELETE FROM users WHERE id = @id";
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
            finally { _dbContext.CloseConnection(); }
            return result;
        }
        public bool UpdateUser(User user)
        {
            bool result = false;
            string query = "UPDATE users SET username = @username, role = @role WHERE id = @id";
            try
            {
                _dbContext.OpenConnection();
                using (var cmd = _dbContext.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@id", user.Id);
                    cmd.Parameters.AddWithValue("@username", user.UserName);
                    cmd.Parameters.AddWithValue("@role", user.Role);
                   
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
            finally { _dbContext.CloseConnection(); }
            return result;
        }
        public bool InsertUser(string name, string password)
        {
            bool result = false;
            string query = "INSERT INTO users (username, password, role) VALUES (@username, @password, @role)";
            try
            {
                _dbContext.OpenConnection();
                using (var cmd = _dbContext.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@username", name);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@role", "user");
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
            finally { _dbContext.CloseConnection(); }
            return result;
        }
    }
}
