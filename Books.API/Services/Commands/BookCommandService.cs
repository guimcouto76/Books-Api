using Books.API.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Books.API.Infrastructure;
using Books.API.Models;
using LiteDB;

namespace Books.API.Services.Commands
{
    public class BookCommandService : IBookCommandService
    {
        private const string BooksCollection = "Books";
        private readonly ILiteDatabase _booksDb;
        private readonly IMapper _mapper;

        public BookCommandService(IBooksDbContext booksDbContext, IMapper mapper)
        {
            _booksDb = booksDbContext.Database;
            _mapper = mapper;
        }

        public async Task<long> CreateBook(BookDto bookDto)
        {
            if (bookDto == null)
                throw new ArgumentNullException(nameof(bookDto));

            var book = _mapper.Map<Book>(bookDto);

            return await Task.Run(() =>
                _booksDb.GetCollection<Book>(BooksCollection)
                    .Insert(book));
        }

        public async Task<bool> DeleteBook(long id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Book Id.");

            return await Task.Run(() =>
                _booksDb.GetCollection<Book>(BooksCollection)
                    .Delete(id));
        }

        public async Task<bool> UnpublishBook(long id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Book Id.");

            var book = await Task.Run(() =>
                _booksDb.GetCollection<Book>(BooksCollection)
                    .FindById(id));

            if (book == default)
                return false;

            book.PublishDate = null;

            return _booksDb.GetCollection<Book>(BooksCollection)
                .Update(book);
        }

        public async Task<bool> UpdateBook(long id, BookDto bookDto)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Id");

            if (bookDto == null)
                throw new ArgumentNullException(nameof(bookDto));

            var book = _mapper.Map<Book>(bookDto);

            return await Task.Run(() =>
            {
                book.Id = id;
                return _booksDb.GetCollection<Book>(BooksCollection)
                    .Update(book);
            });
        }
    }
}
