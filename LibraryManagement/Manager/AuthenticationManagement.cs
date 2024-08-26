using LibraryManagement.Config;
using LibraryManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Manager
{
    public class AuthenticationManagement
    {
        private static User _loggedUser;
        private static ApplicationDBContext _dbContext = new ApplicationDBContext("Server=localhost;Database=qltv;userid=root;password=271201;Port=3306");
        

        public static void SetAuthenticationUser(User user)
        {
            _loggedUser = user;
        }
        public static User GetAuthenticationUser()
        {
            return _loggedUser;
        }
        public static string GetAuthenticationRole()
        {
            return _loggedUser.Role;
        }
        public static int GetAuthenticationId() 
        {
            return _loggedUser.Id;
        }
        public static bool UpdatePasswordOfUser(string newPassword)
        {
            bool result = false;
            string query = "UPDATE users SET password = @Password WHERE id = @Id";
            try
            {
                _dbContext.OpenConnection();
                using (var cmd = _dbContext.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@Id", _loggedUser.Id);
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
        public static void Logout()
        {
            _loggedUser = null;
        }
    }
}
