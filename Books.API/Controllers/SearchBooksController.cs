using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Books.API.Models;
using Books.API.Models.Dtos;
using Books.API.Services.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Books.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchBooksController : ControllerBase
    {
        private readonly ILogger<SearchBooksController> _logger;
        private readonly IBookQueryService _bookQueryService;


        public SearchBooksController(ILogger<SearchBooksController> logger, IBookQueryService bookQueryService)
        {
            _logger = logger;
            _bookQueryService = bookQueryService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> GetBook(long id)
        {
            var bookDto = await _bookQueryService.GetBookById(id);

            if (bookDto != default)
            {
                return Ok(bookDto);
            }

            _logger.LogWarning($"[GET] No book found using Id {id}");
            
            return NotFound();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookDto>>> Get([FromQuery] string title, [FromQuery] string isbn, [FromQuery] string author)
        {
            var books = await _bookQueryService.GetBooks(title, isbn, author);
            return Ok(books);
        }
    }
}
