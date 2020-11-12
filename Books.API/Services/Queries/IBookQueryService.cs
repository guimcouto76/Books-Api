using Books.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Books.API.Models.Dtos;

namespace Books.API.Services.Queries
{
    public interface IBookQueryService
    {
        Task<BookDto> GetBookById(long id);
        Task<IEnumerable<BookDto>> GetBooks(string title, string isbn, string author);
    }
}
