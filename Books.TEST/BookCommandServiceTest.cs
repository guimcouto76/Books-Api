using AutoMapper;
using Books.API.Infrastructure;
using Books.API.Models;
using Books.API.Models.Dtos;
using Books.API.Services.Commands;
using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Books.TEST
{
    [TestClass]
    public class BookCommandServiceTest
    {
        #region Initialization

        private const string BooksCollection = "Books";
        private const long ValidId = 123;
        private const long InvalidId = 999;
        private readonly IBooksDbContext _mockedBooksDbContext;
        private readonly IMapper _mockedMapper;

        public BookCommandServiceTest()
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
                .Setup(x => x.FindById(It.Is<BsonValue>(q => q == ValidId)))
                .Returns(new Book());

            mockedLiteCollection
                .Setup(x => x.FindById(It.Is<BsonValue>(q => q != ValidId)))
                .Returns<Book>(null);

            mockedLiteCollection
                .Setup(x => x.Insert(It.Is<Book>(q => q != null)))
                .Returns(1);

            mockedLiteCollection
                .Setup(x => x.Update(It.Is<Book>(q => q != null)))
                .Returns(true);

            mockedLiteCollection
                .Setup(x => x.Delete(It.Is<BsonValue>(q => q == ValidId)))
                .Returns(true);

            mockedLiteCollection
                .Setup(x => x.Delete(It.Is<BsonValue>(q => q != ValidId)))
                .Returns(false);

            return mockedLiteCollection.Object;

        }
        private IMapper GetMockedAutomapper()
        {
            var mockedAutomapper = new Mock<IMapper>();

            mockedAutomapper.Setup(_ => _.Map<BookDto>(It.IsNotNull<Book>()))
                .Returns((Book book) => new BookDto { 
                    Title = book.Title,
                    Author = book.Author,
                    Isbn = book.Isbn,
                    Price = book.Price,
                    PublishDate = book.PublishDate,
                });

            mockedAutomapper.Setup(_ => _.Map<Book>(It.IsNotNull<BookDto>()))
                 .Returns((BookDto bookDto) => new Book
                 {
                     Id = ValidId,
                     Title = bookDto.Title,
                     Author = bookDto.Author,
                     Isbn = bookDto.Isbn,
                     Price = bookDto.Price,
                     PublishDate = bookDto.PublishDate,
                 });

            return mockedAutomapper.Object;
        }

        #endregion

        #region CreateBook

        [TestMethod]
        public async Task CreateBook_ValidBook_ReturnTrue()
        {
            // Arrange
            var bookCommandService = new BookCommandService(_mockedBooksDbContext, _mockedMapper);
            var bookDto = new BookDto
            {
                Title = "Advanced C#",
                Author = "Guilherme Couto",
                Isbn = "12345",
                Price = 50,
                PublishDate = DateTime.Now
            };

            // Act
            try
            {
                var id = await bookCommandService.CreateBook(bookDto);

                // Assert
                Assert.IsTrue(id > 0);
            }
            catch (Exception error) {
                Console.WriteLine(error.Message);
            }
        }

        [TestMethod]
        public async Task CreateBook_NullBook_RaiseArgumentNullException()
        {
            // Arrange
            var bookCommandService = new BookCommandService(_mockedBooksDbContext, _mockedMapper);

            // Act and Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => bookCommandService.CreateBook(null));
        }

        #endregion

        #region DeleteBook

        [TestMethod]
        public async Task DeleteBook_ValidId_ReturnTrue()
        {
            // Arrange
            var bookCommandService = new BookCommandService(_mockedBooksDbContext, _mockedMapper);

            // Act
            var sucess = await bookCommandService.DeleteBook(ValidId);

            // Assert
            Assert.IsTrue(sucess);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task DeleteBook_InvalidId_RaiseArgumentException(int id)
        {
            // Arrange
            var bookCommandService = new BookCommandService(_mockedBooksDbContext, _mockedMapper);

            // Act and Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => bookCommandService.DeleteBook(id));
        }

        [DataTestMethod]
        [DataRow(111)]
        [DataRow(222)]
        [DataRow(333)]
        public async Task DeleteBook_InvalidId_ReturnFalse(int id)
        {
            // Arrange
            var bookCommandService = new BookCommandService(_mockedBooksDbContext, _mockedMapper);

            // Act
            var sucess = await bookCommandService.DeleteBook(id);

            // Assert
            Assert.IsFalse(sucess);
        }

        #endregion

        #region UpdateBook

        [TestMethod]
        public async Task UpdateBook_ValidBook_ReturnTrue()
        {
            // Arrange
            var bookCommandService = new BookCommandService(_mockedBooksDbContext, _mockedMapper);
            var bookDto = new BookDto
            {
                Title = "Advanced C#",
                Author = "Guilherme Couto",
                Isbn = "12345",
                Price = 50,
                PublishDate = DateTime.Now
            };

            // Act
            var result = await bookCommandService.UpdateBook(ValidId, bookDto);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task UpdateBook_InvalidId_RaiseArgumentException(int id)
        {
            // Arrange
            var bookCommandService = new BookCommandService(_mockedBooksDbContext, _mockedMapper);

            // Act and Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => bookCommandService.UpdateBook(id, null));
        }


        [TestMethod]
        public async Task UpdateBook_NullBook_RaiseArgumentNullException()
        {
            // Arrange
            var bookCommandService = new BookCommandService(_mockedBooksDbContext, _mockedMapper);

            // Act and Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => bookCommandService.UpdateBook(ValidId, null));
        }

        #endregion

        #region UnpublishBook

        [TestMethod]
        public async Task UnpublishBook_ValidId_ReturnTrue()
        {
            // Arrange
            var bookCommandService = new BookCommandService(_mockedBooksDbContext, _mockedMapper);

            // Act
            var result = await bookCommandService.UnpublishBook(ValidId);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task UnpublishBook_InvalidId_RaiseArgumentException(int id)
        {
            // Arrange
            var bookCommandService = new BookCommandService(_mockedBooksDbContext, _mockedMapper);

            // Act
            var result = await bookCommandService.UnpublishBook(InvalidId);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion
    }
}
