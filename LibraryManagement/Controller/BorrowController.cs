using LibraryManagement.Config;
using LibraryManagement.Display;
using LibraryManagement.Repository.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Controller
{
    
    public class BorrowController
    {
        private readonly BorrowRepository _borrowRepository;
        public BorrowController(BorrowRepository borrowRepository)
        {
            _borrowRepository = borrowRepository;
        }
        public int BorrowList(string title, string status)
        {
            Console.WriteLine("**********************************BORROW MANAGEMENT***************************");
            Console.WriteLine("** 1. Search Borrow                                                          *");
            Console.WriteLine("** 0. Main Menu                                                              *");
            Console.WriteLine("******************************************************************************");
            Console.WriteLine();
            var borrows = _borrowRepository.FindAllBorrowLikeStatusAndTitle(title, status);
            DisplayContext.TableContent(borrows, new List<string>());
            int choice = DisplayContext.SelectMenuItem();
            return choice;
        }
        public void DashBoard()
        {
            Console.WriteLine("************************************DASHBOARD*********************************");
            Console.WriteLine();
            var borrows = _borrowRepository.FindNumberOfBorrowByDay();
            DateTime today = DateTime.Now;
            DateTime sevenDaysAgo = today.AddDays(-7);
            var dates = Enumerable.Range(0, 8) // 8 để bao gồm cả ngày hôm nay và ngày trước đó
                        .Select(i => sevenDaysAgo.AddDays(i))
                        .ToList();
            var borrowCountsByDate = dates
                .Select(date => new
                {
                    Date = date.Date.ToString("dd-MM-yyyy"),
                    Count = borrows.Count(b =>
                    {
                        if (DateTime.TryParseExact(b.BorrowDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime borrowDate))
                        {
                            return borrowDate.Date == date.Date;
                        }
                        return false;
                    })
                })
                .OrderBy(result => result.Date)
                .ToList();
            DisplayContext.TableContent (borrowCountsByDate, new List<string>());
        }
    }
}
