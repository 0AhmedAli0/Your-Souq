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

        public async Task<ActionResult> CreatePagedResult<T,TDTO>(IGenericRepository<T> _repository,
            ISpecification<T> spec, int pageIndex, int pageSize, Func<T,TDTO> func ) where T : BaseEntity ,IDtoConvertible
        {
            var data = await _repository.ListAsync(spec);

            var count = await _repository.CountAsync(spec);

            var data2 = data.Select(func).ToList();

            return Ok(new Pagination<TDTO>(pageIndex, pageSize, count, data2));
        }
    }
}
