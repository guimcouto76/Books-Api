using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.API.Models.Dtos
{
    public class BookDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public DateTime? PublishDate { get; set; }
    }
}
