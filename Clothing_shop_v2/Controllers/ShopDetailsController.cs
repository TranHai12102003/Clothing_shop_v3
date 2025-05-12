using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Services.ISerivce;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Controllers
{
    public class ShopDetailsController : Controller
    {
        private readonly ILogger<ShopDetailsController> _logger;
        private readonly IProductService _productService;
        public ShopDetailsController(ILogger<ShopDetailsController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        public async Task<IActionResult> Index(int id)
        {
            var product = await _productService.GetProductDetail(id);
            if (product == null)
            {
                return NotFound();
            }
             return View(product.Value);
        }
    }
}
