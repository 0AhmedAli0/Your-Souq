using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        public async Task<ActionResult> CreatePagedResult<T> (IGenericRepository<T> _repository,
            ISpecification<T> spec , int pageIndex, int pageSize) where T : BaseEntity
        {
            var data = await _repository.ListAsync(spec);

            var count = await _repository.CountAsync(spec);

            return Ok(new Pagination<T>(pageIndex, pageSize, count, data));
        }
    }
}
