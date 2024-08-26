using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Model
{
    public class Borrow
    {
        public int Id { get; set; }
        public string BookTitle { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string BorrowDate { get; set; }
        public string ReturnDate { get; set; }
        public string Status { get; set; }
    }
}
