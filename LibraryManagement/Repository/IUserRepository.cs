using LibraryManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Repository
{
    public interface IUserRepository
    {
        User FindUserByUsernameAndPassword(string username, string password);
        User FindUserById(int id);
        List<User> FindAllUserLikeUserName(string username);
        bool DeleteUserById(int id);
        bool UpdateUser(User user);
        bool InsertUser(string name, string password);
    }
}
