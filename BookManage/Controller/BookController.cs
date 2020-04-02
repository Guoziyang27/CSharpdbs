using System;
using System.Reflection;
using System.Threading.Tasks;
using BookManage.Models;
using BookManage.Services;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;

namespace BookManage.Controller
{
    [Route("api")]
    public class BookController : ControllerBase
    {
        private readonly BookManagementService _service;

        public BookController(BookManagementService service) => _service = service;

        [HttpPost("init/database")]
        public IActionResult InitDatabase()
        {
            return Ok(ApiResponse.Success(new
            {
                info = _service.IntiDatabase()
            }));
        }

        [HttpPost("add/books")]
        public IActionResult AddBooks([FromBody] BookReqDto data)
        {
            if (data == null)
                return Ok(ApiResponse.Error("ERROR_PARAMETER"));
            var books = new Book()
            {
                Class = data.Class,
                Title = data.Title,
                Author = data.Author,
                Publish = data.Publish,
                Pubyear = data.Pubyear,
                Price = data.Price,
                Total = data.Number,
                Stock = data.Number
            };
            var result = _service.AddBook(books);
            return Ok(ApiResponse.Success(new {id = result}));
        }

        [HttpGet("get/books")]
        public IActionResult QueryBooks([FromQuery] BookQueryReqDto data)
        {
            if (data == null)
                return Ok(ApiResponse.Error("ERROR_PARAMETER"));
            var query = new Query()
            {
                Class = data.Class,
                Author = data.Author,
                Publish = data.Publish,
                Pubyear = new Tuple<int, int>(data.PubyearLower, data.PubyearUpper),
                Price = new Tuple<double, double>(data.PriceLower, data.PriceUpper)
            };

            var result = _service.GetBook(query);

            return Ok(ApiResponse.Success(new {list = result}));
        }

        [HttpGet("get/person/{id}")]
        public IActionResult GetPerson([FromRoute] int id)
        {
            var result = _service.GetPersonInfo(id);
            return result.id != -1 ? Ok(ApiResponse.Success(result)) :
                Ok(ApiResponse.Error("PERSON_NOT_FOUND"));
        }
        
        [HttpGet("get/borrowed/{id}")]
        public IActionResult GetBorrowed([FromRoute] int id)
        {
            var result = _service.GetBorrowed(id);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPost("borrow/{cid}/{bid}")]
        public IActionResult Borrow([FromRoute] int cid, [FromRoute] int bid)
        {
            var isEmpty = _service.IsEmpty(bid);
            if (isEmpty)
            {
                var date = _service.GetReturnDate(bid);
                return Ok(ApiResponse.Success(new {info = "No avaiable book", return_date = date}));
            }

            var result = _service.Borrow(bid, cid);
            
            return Ok(ApiResponse.Success(new {info = result}));
        }
        [HttpPost("return/{cid}/{bid}")]
        public IActionResult Return([FromRoute] int cid, [FromRoute] int bid)
        {
            var BorrowedID = _service.IsBorrowed(bid, cid);
            if (BorrowedID == -1)
            {
                return Ok(ApiResponse.Success(new {info = "This card didn't borrow this book"}));
            }

            var result = _service.Return(BorrowedID);
            
            return Ok(ApiResponse.Success(new {info = result}));
        }

        [HttpPost("add/card")]
        public IActionResult AddCard([FromBody] CardReqDto c)
        {
            Person p = new Person()
            {
                Name = c.Name,
                Company = c.Company,
                Class = c.Class
            };
            var cardID = _service.AddCard(p);
            return Ok(ApiResponse.Success(new {ID = cardID}));
        }
        
        [HttpPost("del/card/{id}")]
        public IActionResult DelCard([FromRoute] int id)
        {
            var result = _service.DelCard(id);
            return Ok(ApiResponse.Success(new {info = result}));
        }
    }
}