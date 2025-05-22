using System.ComponentModel.DataAnnotations;

namespace Clothing_shop_v2.VModels
{
    public class CheckoutVModel
    {
        // Địa chỉ thanh toán
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        // Phương thức thanh toán
        public string PaymentMethod { get; set; }

        // Checkbox giao hàng khác
        public bool ShipToDifferentAddress { get; set; }

        // Địa chỉ giao hàng (nếu khác)
        public string ShippingFullName { get; set; }
        public string ShippingEmail { get; set; }
        public string ShippingPhoneNumber { get; set; }
        public string ShippingAddress { get; set; }
    }
}
