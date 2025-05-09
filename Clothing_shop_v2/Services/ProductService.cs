﻿using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Services
{
    public class ProductService : IProductService
    {
        private readonly ClothingShopV3Context _context;
        private readonly IProductImageService _productImageService;
        public ProductService(ClothingShopV3Context context, IProductImageService productImageService)
        {
            _context = context;
            _productImageService = productImageService;
        }

        public async Task<ResponseResult> Create(ProductCreateVModel product)
        {
            var response = new ResponseResult();
            try
            {
                var newProduct = ProductMapping.VModelToEntity(product);
                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();
                if (product.imageFiles != null && product.imageFiles.Count > 0)
                {
                    //var imageResponse = await _productImageService.AddImages(newProduct.Id ,product.imageFiles, null);
                    await _productImageService.AddImages(newProduct.Id, product.imageFiles, null);
                }
                response = new SuccessResponseResult(newProduct, "Thêm sản phẩm mới thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ResponseResult> Delete(int id)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
                if (product == null)
                {
                    return new ErrorResponseResult("Không tìm thấy sản phẩm");
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return new SuccessResponseResult(product, "Xóa sản phẩm thành công");
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult($"Không thể xóa sản phẩm: {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                // Xử lý lỗi khóa ngoại hoặc lỗi database
                return new ErrorResponseResult("Không thể xóa sản phẩm do có dữ liệu liên quan hoặc lỗi database.");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để debug
                Console.WriteLine($"Lỗi khi xóa sản phẩm ID {id}: {ex}");
                return new ErrorResponseResult("Đã xảy ra lỗi khi xóa sản phẩm. Vui lòng thử lại sau.");
            }
        }

        public async Task<ActionResult<PaginationModel<ProductGetVModel>>> GetAll(ProductFilterParams parameters)
        {
            IQueryable<Product> query = _context.Products
                .Include(p => p.Category)
                .Include(p=>p.ProductImages)
                .Include(p => p.Variants)
                .Where(BuildQueryable(parameters));

            var promotions = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(x => ProductMapping.EntityToVModel(x))
                .ToListAsync();

            var totalRecords = await query.CountAsync();

            return new PaginationModel<ProductGetVModel>
            {
                Records = promotions,
                TotalRecords = totalRecords,
                PageSize = parameters.PageSize,
                CurrentPage = parameters.PageNumber
                // TotalPages tự tính nên không cần gán!
            };
        }

        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .ThenInclude(v => v.Color)
                .Include(p => p.Variants)
                .ThenInclude(v => v.Size)
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return null;
            }
            //var productVModel = ProductMapping.EntityToVModel(product);
            //return productVModel;
            return product;
        }

        public async Task<ResponseResult> Update(ProductUpdateVModel product)
        {
            var response = new ResponseResult();
            try
            {
                var existingProduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == product.Id);
                if (existingProduct == null)
                {
                    return new ErrorResponseResult("Không tìm thấy sản phẩm");
                }
                var updatedProduct = ProductMapping.VModelToEntity(product, existingProduct);
                _context.Products.Update(updatedProduct);
                await _context.SaveChangesAsync();
                if (product.imageFiles != null && product.imageFiles.Count > 0)
                {
                    var imageResponse = await _productImageService.AddImages(updatedProduct.Id, product.imageFiles, null);
                }
                response = new SuccessResponseResult(updatedProduct, "Cập nhật sản phẩm thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        private Expression<Func<Product, bool>> BuildQueryable(ProductFilterParams fParams)
        {
            return x =>
                (fParams.SearchString == null || (x.ProductName != null && x.ProductName.Contains(fParams.SearchString)));
        }
    }
}
