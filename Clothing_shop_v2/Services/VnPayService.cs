﻿using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Libraries;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services.ISerivce;

namespace Clothing_shop_v2.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(Order model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnpayLibrary();
            var urlCallBack = _configuration["Vnpay:PaymentBackReturnUrl"];

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            //pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_Amount", ((int)model.TotalAmount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            //pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
            pay.AddRequestData("vnp_OrderInfo", $"{model.OrderDate} {model.TotalAmount}");
            //pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_OrderType", model.Status);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            //pay.AddRequestData("vnp_TxnRef", tick);
            pay.AddRequestData("vnp_TxnRef", model.Id.ToString());

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;

        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnpayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            return response;

        }
    }
}
