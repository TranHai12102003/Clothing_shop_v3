using Clothing_shop_v2.Mappings;
using System.Security.Claims;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Clothing_shop_v2.Services.ISerivce;
using Shopapp.Mappings;

namespace Clothing_shop_v2.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ClothingShopV3Context _context;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        public CheckoutController(ClothingShopV3Context context, ICartService cartService, IOrderService orderService)
        {
            _context = context;
            _cartService = cartService;
            _orderService = orderService;
        }
        public async Task<IActionResult> Index()
        {
            List<CartGetVModel> cartItems;

            if (!User.Identity.IsAuthenticated)
            {
                // Người dùng chưa đăng nhập: Lấy giỏ hàng từ cookie
                var cartCookie = Request.Cookies["Cart"];
                var cartSession = string.IsNullOrEmpty(cartCookie)
                    ? new List<CartCreateVModel>()
                    : JsonSerializer.Deserialize<List<CartCreateVModel>>(cartCookie);

                var variantIds = cartSession.Select(c => c.VariantId).ToList();
                var variants = await _context.Variants
                    .Where(v => variantIds.Contains(v.Id))
                    .Include(v => v.Product)
                        .ThenInclude(p => p.ProductImages)
                    .Include(v => v.Size)
                    .Include(v => v.Color)
                    .ToListAsync();

                cartItems = variants.Select(v => new CartGetVModel
                {
                    Id = 0,
                    VariantId = v.Id,
                    UserId = 0,
                    Quantity = cartSession.FirstOrDefault(c => c.VariantId == v.Id)?.Quantity ?? 0,
                    TotalPrice = v.Price * (cartSession.FirstOrDefault(c => c.VariantId == v.Id)?.Quantity ?? 0),
                    Variant = VariantMapping.EntityGetVModel(v)
                }).ToList();

                // Lấy danh mục
                ViewBag.Categories = await _context.Categories
                    .Select(c => CategoryMapping.EntityToVModel(c))
                    .ToListAsync();

                ViewBag.CartCount = cartSession.Count;
            }
            else
            {
                // Người dùng đã đăng nhập: Lấy giỏ hàng từ dịch vụ
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                cartItems = await _cartService.GetAll(userId) ?? new List<CartGetVModel>();
                ViewBag.CartCount = cartItems.Count;
                // Lấy thông tin hồ sơ người dùng (nếu có)
                var userProfile = User.Identity.IsAuthenticated
                    ? await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
                    : null;
                ViewBag.UserProfile = userProfile;
            }
            return View(cartItems ?? new List<CartGetVModel>());
        }
    }
}
