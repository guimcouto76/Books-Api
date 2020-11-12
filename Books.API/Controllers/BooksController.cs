using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Books.API.Models.Dtos;
using Books.API.Services.Commands;
using Books.API.Services.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Books.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // User [Authorize] to allow only Authors Role to access the Books Controller
    public class BooksController : ControllerBase
    {
        private readonly ILogger<SearchBooksController> _logger;
        private readonly IMapper _mapper;
        private readonly IBookCommandService _bookCommandService;

        public BooksController(ILogger<SearchBooksController> logger, IMapper mapper, IBookCommandService bookCommandService)
        {
            _logger = logger;
            _mapper = mapper;
            _bookCommandService = bookCommandService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookDto>> Insert([FromBody] BookDto bookDto)
        {
            var id = await _bookCommandService.CreateBook(bookDto);

            if (id != default)
            {
                bookDto.Id = id;
                return Created("", bookDto);
            }

            _logger.LogWarning($"[POST] Bad request. Payload: {JsonConvert.SerializeObject(bookDto)}");
            return BadRequest();

        }

        [Route("{id}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> Update(int id, [FromBody] BookDto bookDto)
        {
            var result = await _bookCommandService.UpdateBook(id, bookDto);
            if (result) return NoContent();

            _logger.LogWarning($"[PUT] No book found using Id {id}");
            return NotFound();
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> Delete(int id)
        {
            var result = await _bookCommandService.DeleteBook(id);

            if (result) return NoContent();

            _logger.LogWarning($"[DELETE] No book found using Id {id}");
            return NotFound();
        }

        [Route("{id}")]
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> Unpublish(int id)
        {
            var result = await _bookCommandService.UnpublishBook(id);
            if (result) return NoContent();

            _logger.LogWarning($"[PUT] No book found using Id {id}");
            return NotFound();
        }


    }
}
