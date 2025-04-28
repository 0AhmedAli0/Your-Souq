using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class BuggyController : BaseApiController
    {
        [HttpGet("unauthorized")]
        public IActionResult GetUnauthorized()
        {
            return Unauthorized();
        }
        [HttpGet("badRequest")]
        public IActionResult GetBadRequest()
        {
            return BadRequest("not a good request");
        }
        [HttpGet("notFound")]
        public IActionResult GetNotFound()
        {
            return NotFound();
        }
        [HttpGet("internalerror")]
        public IActionResult GetInternalError()
        {
            throw new Exception("this is a test Exception");
        }
        [HttpPost("validationerror")]
        public IActionResult GetValidationError(Product product)
        {
            return Ok();
        }
       
    }
}
