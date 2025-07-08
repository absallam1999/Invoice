using invoice.Data;
using invoice.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IRepository<Product> repository;

        public ProductController(IRepository<Product> _repository)
        {
            repository = _repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await repository.GetAll());
    }
}
