using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Model
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
    }
}
