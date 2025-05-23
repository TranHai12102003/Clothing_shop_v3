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
    public class OrderService : IOrderService
    {
        private readonly ClothingShopV3Context _context;
        public OrderService(ClothingShopV3Context context)
        {
            _context = context;
        }

        public async Task<ResponseResult> Create(OrderCreateVModel vModel)
        {
            var response = new ResponseResult();
            try
            {
                var order = OrderMapping.VModelToEntity(vModel);
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(order, "Đơn hàng được tạo thành công");
                return response;
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ResponseResult> Delete(int id)
        {
            var response = new ResponseResult();
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
                if (order == null)
                {
                    return new ErrorResponseResult("Không tìm thấy đơn hàng");
                }
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(order, "Xóa đơn hàng thành công");
                return response;
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ActionResult<PaginationModel<OrderGetVModel>>> GetAll(OrderFilterParams parameters)
        {
            IQueryable<Order> query = _context.Orders
                .Include(x => x.User)
                .Include(x => x.OrderDetails)
                .Where(BuildQueryable(parameters));
            var orders = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(x => OrderMapping.EntityGetVModel(x))
                .ToListAsync();
            var totalRecords = await query.CountAsync();
            return new PaginationModel<OrderGetVModel>
            {
                Records = orders,
                TotalRecords = totalRecords,
                PageSize = parameters.PageSize,
                CurrentPage = parameters.PageNumber
            };
        }

        public async Task<ActionResult<OrderGetVModel>> GetById(int id)
        {
            var order = await _context.Orders
                .Include(x => x.User)
                .Include(x => x.OrderDetails).ThenInclude(x => x.Variant).ThenInclude(x => x.Product).ThenInclude(x => x.ProductImages)
                .Include(x => x.OrderDetails).ThenInclude(x => x.Variant).ThenInclude(x => x.Size)
                .Include(x => x.OrderDetails).ThenInclude(x => x.Variant).ThenInclude(x => x.Color)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
            {
                return null;
            }
            var orderVModel = OrderMapping.EntityGetVModel(order);
            return orderVModel;
        }

        public async Task<ResponseResult> Update(OrderUpdateVModel vModel)
        {
            var response = new ResponseResult();
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == vModel.Id);
                if (order == null)
                {
                    return new ErrorResponseResult("Không tìm thấy đơn hàng");
                }
                order = OrderMapping.VModelToEntity(vModel, order);
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(order, "Cập nhật đơn hàng thành công");
                return response;
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        private Expression<Func<Order, bool>> BuildQueryable(OrderFilterParams fParams)
        {
            return x =>
                (string.IsNullOrEmpty(fParams.FullName) || (x.User.FullName != null && x.User.FullName.Contains(fParams.FullName))) &&
                (fParams.IsActive == null || x.IsActive == fParams.IsActive) &&
                (fParams.OrderDate == null || (x.OrderDate != null && x.OrderDate == fParams.OrderDate)) &&
                (fParams.Status == null || (x.Status != null && x.Status.Contains(fParams.Status)));
        }
    }
}
