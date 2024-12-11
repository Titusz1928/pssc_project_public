using Lab2.Domain.Models;
using Lab2.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductsRepository _productsRepository;

        public ProductsController(ILogger<ProductsController> logger, IProductsRepository productsRepository)
        {
            _logger = logger;
            _productsRepository = productsRepository;
        }

        // Endpoint to retrieve all products
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productsRepository.GetAllProductsAsync();
            var result = products.Select(product => new
            {
                ProductId = product.ProductId,
                Code = product.Code.Value,
                Stock = product.Stoc
            });

            return Ok(result);
        }
    }
}