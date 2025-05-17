using System.Security.Claims;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ClothingShopV3Context _context;
        public CartController(ICartService cartService, ClothingShopV3Context context)
        {
            _cartService = cartService;
            _context = context;
        }
        [Authorize] // Yêu cầu đăng nhập
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _cartService.GetAll(userId);
            return View(result);
        }
        //[HttpPost]
        //public async Task<IActionResult> AddToCart( CartCreateVModel cartVModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(cartVModel);
        //    }
        //    var response = await _cartService.Create(cartVModel);
        //    if (response.IsSuccess)
        //    {
        //        TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
        //        return RedirectToAction("Index");
        //    }
        //    TempData["ErrorMessage"] = response.Message; // Lưu thông báo thành công
        //    return View(cartVModel);
        //}
        [HttpPost]
        public async Task<IActionResult> AddToCart(CartFormVModel formModel)
        {
            if (!ModelState.IsValid || formModel.SizeId == 0 || formModel.ColorId == 0 || formModel.Quantity < 1)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn kích cỡ, màu sắc và số lượng hợp lệ.";
                return RedirectToAction("Index", "ShopDetails", new { id = formModel.ProductId });
            }

            // Tìm VariantId dựa trên ProductId, SizeId và ColorId
            var variant = await _context.Variants
                .FirstOrDefaultAsync(v => v.ProductId == formModel.ProductId &&
                                         v.SizeId == formModel.SizeId &&
                                         v.ColorId == formModel.ColorId);

            if (variant == null)
            {
                TempData["ErrorMessage"] = "Biến thể sản phẩm không tồn tại.";
                return RedirectToAction("Index", "ShopDetails", new { id = formModel.ProductId });
            }

            // Tạo CartCreateVModel để gửi đến service
            var cartModel = new CartCreateVModel
            {
                UserId = formModel.UserId,
                VariantId = variant.Id,
                Quantity = formModel.Quantity
            };

            // Gọi service để thêm vào giỏ hàng
            var response = await _cartService.Create(cartModel);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Index", "Cart");
            }

            TempData["ErrorMessage"] = response.Message;
            return RedirectToAction("Index", "ShopDetails", new { id = formModel.ProductId });
        }
        [HttpPost]
        public async Task<IActionResult> Update(CartUpdateVModel cartVModel)
        {
            if (!ModelState.IsValid)
            {
                return View(cartVModel);
            }
            var response = await _cartService.Update(cartVModel);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = response.Message; // Lưu thông báo thành công
            return View(cartVModel);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _cartService.Delete(id);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = response.Message; // Lưu thông báo thành công
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteByUserId()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var response = await _cartService.DeleteByUserId(userId);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = response.Message; // Lưu thông báo thành công
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteByVariantId(int variantId)
        {
            var response = await _cartService.DeleteByVariantId(variantId);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = response.Message; // Lưu thông báo thành công
            return RedirectToAction("Index");
        }
    }
}
