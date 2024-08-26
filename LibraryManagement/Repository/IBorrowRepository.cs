using LibraryManagement.Model;
using System.Collections.Generic;

namespace LibraryManagement.Repository
{
    public interface IBorrowRepository
    {
        List<Borrow> FindNumberOfBorrowByDay();
        List<Borrow> FindAllBorrowLikeStatusAndTitle(string bookTitle, string status);
        List<Borrow> FindAllBorrowByUserIdLikeTitle(int userId, string title);
        bool UpdateStatusToReturned(List<int> bookIdList);
        bool InsertBatchBorrow(List<int> bookIds);
    }
}
