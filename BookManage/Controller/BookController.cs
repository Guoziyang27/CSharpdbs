using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using BookManage.Models;
using BookManage.Services;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace BookManage.Controller
{
    [Route("api")]
    public class BookController : ControllerBase
    {
        private readonly BookManagementService _service;

        public BookController(BookManagementService service) => _service = service;

        [HttpPost("authenticate")]
        public async Task<IActionResult> Login([FromBody] AuthenticateReqDto body)
        {
            var result = _service.GetPersonInfo(Int32.Parse(body.CardId));
            if (result.Password == body.Password)
            {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, body.CardId));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = false
                    });
                return Ok(ApiResponse.Success(new
                {
                    info = "Login successfully"
                }));
            }

            return Unauthorized();
        }

        [HttpPut("authenticate")]
        public async Task<IActionResult> Logout()
        {
            var id = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(ApiResponse.Success(new
            {
                info = "Logout successfully"
            }));
        }

        [HttpPost("init/database")]
        public IActionResult InitDatabase()
        {
            return Ok(ApiResponse.Success(new
            {
                info = _service.IntiDatabase()
            }));
        }

        [HttpPost("books")]
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
            return Ok(ApiResponse.Success(new {data = result}));
        }

        [HttpGet("books")]
        public IActionResult QueryBooks([FromQuery] BookQueryReqDto body)
        {
            if (body == null)
                return Ok(ApiResponse.Error("ERROR_PARAMETER"));
            BookQueryReqDto data = body;
            var query = new Query()
            {
                Class = data.Class,
                Author = data.Author,
                Publish = data.Publish,
                Title = data.Title,
                Pubyear = new Tuple<int, int>(data.PubyearLower, data.PubyearUpper),
                Price = new Tuple<double, double>(data.PriceLower, data.PriceUpper)
            };

            List<Book> result = _service.GetBook(query);
            
            return Ok(ApiResponse.Success(new {data = result}));
        }

        [HttpGet("books/{id}")]
        public IActionResult QueryOneBook([FromRoute] int id)
        {
            var result = _service.GetBook(id);

            return Ok(ApiResponse.Success(new {data = result}));
        }

        [HttpGet("person_info")]
        public IActionResult GetPerson()
        {
            var id = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = _service.GetPersonInfo(id);
            return result.id != -1 ? Ok(ApiResponse.Success(result)) :
                Ok(ApiResponse.Error("PERSON_NOT_FOUND"));
        }
        
        [HttpGet("paerson_borrowed")]
        public IActionResult GetBorrowed()
        {
            var id = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = _service.GetBorrowed(id);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPost("borrow/{bid}")]
        public IActionResult Borrow([FromRoute] int bid)
        {
            var cid = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var isEmpty = _service.IsEmpty(bid);
            if (isEmpty)
            {
                var date = _service.GetReturnDate(bid);
                return Ok(ApiResponse.Success(new {info = "No avaiable book", return_date = date}));
            }

            var result = _service.Borrow(bid, cid);
            
            return Ok(ApiResponse.Success(new {info = result}));
        }
        [HttpPost("return/{bid}")]
        public IActionResult Return([FromRoute] int bid)
        {
            var cid = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var borrowedId = _service.IsBorrowed(bid, cid);
            if (borrowedId == -1)
            {
                return Ok(ApiResponse.Success(new {info = "This card didn't borrow this book"}));
            }

            var result = _service.Return(borrowedId);
            
            return Ok(ApiResponse.Success(new {info = result}));
        }

        [HttpPost("register")]
        public IActionResult AddCard([FromBody] CardReqDto c)
        {
            Person p = new Person()
            {
                Name = c.Name,
                Company = c.Company,
                Password = c.Password,
                Class = c.Class
            };
            var cardId = _service.AddCard(p);
            return Ok(ApiResponse.Success(new {ID = cardId}));
        }
        
        [HttpPut("register")]
        public async Task<IActionResult> DelCard()
        {
            var id = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = _service.DelCard(id);
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(ApiResponse.Success(new {info = result}));
        }
    }
}