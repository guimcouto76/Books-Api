using Books.API.Models;
using Books.API.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.API.Services.Commands
{
    public interface IBookCommandService
    {
        Task<long> CreateBook(BookDto bookDto);
        Task<bool> UpdateBook(long id, BookDto bookDto);
        Task<bool> DeleteBook(long id);
        Task<bool> UnpublishBook(long id);
    }
}
