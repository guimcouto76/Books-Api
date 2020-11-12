using Books.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Books.API.Infrastructure;
using LiteDB;
using Books.API.Models.Dtos;

namespace Books.API.Services.Queries
{
    public class BookQueryService : IBookQueryService
    {
        private const string BooksCollection = "Books";
        private readonly ILiteDatabase _booksDb;
        private readonly IMapper _mapper;

        public BookQueryService(IBooksDbContext booksDbContext, IMapper mapper)
        {
            _booksDb = booksDbContext.Database;
            _mapper = mapper;
        }

        public async Task<BookDto> GetBookById(long id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Book Id");

            var book =  await Task.Run(() =>
                _booksDb.GetCollection<Book>(BooksCollection)
                    .FindById(id));

            return _mapper.Map<BookDto>(book);
        }

        public async Task<IEnumerable<BookDto>> GetBooks(string title, string isbn, string author)
        {
            var books =  await Task.Run(() =>
                _booksDb.GetCollection<Book>(BooksCollection)
                    .FindAll());

            // Since LiteDb is very limited I'm filtering the list in memory
            // In a real project we should use a Database with server filtering capabilities

            var booksQuery = books.AsQueryable();

            if (!string.IsNullOrEmpty(title))
                booksQuery = booksQuery.Where(b => b.Title == title);

            if (!string.IsNullOrEmpty(isbn))
                booksQuery = booksQuery.Where(b => b.Isbn == isbn);

            if (!string.IsNullOrEmpty(author))
                booksQuery = booksQuery.Where(b => b.Author == author);

            return _mapper.Map<IEnumerable<BookDto>>(booksQuery.ToList());
        }
    }
}
