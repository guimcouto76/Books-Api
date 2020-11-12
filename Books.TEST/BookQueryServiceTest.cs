using AutoMapper;
using Books.API.Infrastructure;
using Books.API.Models;
using Books.API.Models.Dtos;
using Books.API.Services.Queries;
using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Books.TEST
{
    [TestClass]
    public class BookQueryServiceTest
    {
        #region Initialization

        private const string BooksCollection = "Books";
        private const long ValidId = 123;
        private const long InvalidId = 999;
        private readonly IBooksDbContext _mockedBooksDbContext;
        private readonly IMapper _mockedMapper;

        public BookQueryServiceTest()
        {
            _mockedBooksDbContext = GetMockedBooksDbContext();
            _mockedMapper = GetMockedAutomapper();
        }

        #endregion

        #region Mocks
        private IBooksDbContext GetMockedBooksDbContext()
        {
            var mockedBooksDbContext = new Mock<IBooksDbContext>();

            var liteDatabase = GetMockedLiteDatabase();

            mockedBooksDbContext
                .SetupGet(x => x.Database)
                .Returns(liteDatabase);

            return mockedBooksDbContext.Object;
        }

        private ILiteDatabase GetMockedLiteDatabase()
        {
            var mockedLiteDatabase = new Mock<ILiteDatabase>();

            mockedLiteDatabase
                .Setup(x => x.GetCollection<Book>(It.Is<string>(q => q == BooksCollection), It.IsAny<BsonAutoId>()))
                .Returns(GetMockedLiteCollection());

            return mockedLiteDatabase.Object;
        }

        private ILiteCollection<Book> GetMockedLiteCollection()
        {
            var mockedLiteCollection = new Mock<ILiteCollection<Book>>();

            mockedLiteCollection
               .Setup(x => x.FindAll())
               .Returns(new List<Book>());

            mockedLiteCollection
                .Setup(x => x.FindById(It.Is<BsonValue>(q => q == ValidId)))
                .Returns(new Book());

            mockedLiteCollection
                .Setup(x => x.FindById(It.Is<BsonValue>(q => q != ValidId)))
                .Returns<Book>(null);

            return mockedLiteCollection.Object;

        }
        private IMapper GetMockedAutomapper() { 
            var mockedAutomapper = new Mock<IMapper>();

            var mockedBook = new Book();

            var mockedBookDto = new BookDto();

            mockedAutomapper.Setup(_ => _.Map<BookDto>(It.IsNotNull<Book>()))
                .Returns(mockedBookDto);

            mockedAutomapper.Setup(_ => _.Map<Book>(It.IsNotNull<BookDto>()))
                .Returns(mockedBook);

            return mockedAutomapper.Object;
        }

        #endregion

        #region GetBooks

        [TestMethod]
        public async Task GetBooks_MockedDatabase_ReturnNotNull()
        {
            // Arrange
            var bookQueryService = new BookQueryService(_mockedBooksDbContext, _mockedMapper);

            // Act
            var books = await bookQueryService.GetBooks(string.Empty, string.Empty, string.Empty);

            // Assert
            Assert.IsNotNull(books);
        }

        #endregion

        #region GetBookById

        [TestMethod]
        public async Task GetBookById_ValidId_ReturnNotNull()
        {
            // Arrange
            var bookQueryService = new BookQueryService(_mockedBooksDbContext, _mockedMapper);

            // Act
            var books = await bookQueryService.GetBookById(ValidId);

            // Assert
            Assert.IsNotNull(books);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task GetBookById_InvalidId_RaiseArgumentException(int id)
        {
            // Arrange
            var bookQueryService = new BookQueryService(_mockedBooksDbContext, _mockedMapper);

            // Act and Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => bookQueryService.GetBookById(id));
        }

        [DataTestMethod]
        [DataRow(111)]
        [DataRow(222)]
        [DataRow(333)]
        public async Task GetBookById_InvalidId_ReturnNull(int id)
        {
            // Arrange
            var bookQueryService = new BookQueryService(_mockedBooksDbContext, _mockedMapper);

            // Act
            var book = await bookQueryService.GetBookById(id);

            // Assert
            Assert.IsNull(book);
        }

        #endregion
    }
}
