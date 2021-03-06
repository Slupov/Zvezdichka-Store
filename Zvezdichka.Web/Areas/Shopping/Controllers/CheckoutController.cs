﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zvezdichka.Common;
using Zvezdichka.Services.Contracts.Entity;
using Zvezdichka.Services.Contracts.Entity.Checkout;
using Zvezdichka.Web.Areas.Shopping.Models;
using Zvezdichka.Web.Areas.Shopping.Models.Checkout;
using Zvezdichka.Web.Infrastructure.Extensions;
using Zvezdichka.Web.Services;

namespace Zvezdichka.Web.Areas.Shopping.Controllers
{
    [Authorize]
    public class CheckoutController : ShoppingBaseController
    {
        private readonly IShoppingCartManager shoppingCartManager;
        private readonly IProductsDataService products;
        private readonly IDeliveryOptionsDataService deliveryOptions;
        private readonly IPaymentOptionsDataService paymentOptions;

        public CheckoutController(IShoppingCartManager shoppingCartManager, IProductsDataService products,
            IDeliveryOptionsDataService deliveryOptions, IPaymentOptionsDataService paymentOptions)
        {
            this.shoppingCartManager = shoppingCartManager;
            this.products = products;
            this.deliveryOptions = deliveryOptions;
            this.paymentOptions = paymentOptions;
        }

        public async Task<IActionResult> Index()
        {
            //get cart items from session
            var shoppingCartId = this.HttpContext.Session.GetShoppingCartId();
            var cartItems = this.shoppingCartManager.GetCartItems(shoppingCartId).ToList();

            var productIds = cartItems.Select(y => y.ProductId).ToList();
            var dbProducts = this.products.GetList(x => productIds.Contains(x.Id));

            string errorMsg = string.Empty;
            bool hasErrors = false;

            //check for errors
            for (int i = 0; i < cartItems.Count(); i++)
            {
                var product = dbProducts.SingleOrDefault(x => x.Id == cartItems[i].ProductId);

                if (product == null)
                {
                    return NotFound();
                }

                if (product.Stock < cartItems[i].Quantity)
                {
                    hasErrors = true;
                    errorMsg += string.Format(CommonConstants.StockAmountExceededForError, product.Name) +
                                Environment.NewLine;
                }
            }

            if (hasErrors)
            {
                Danger(errorMsg);
                return RedirectToAction("Cart", "Home", new {area = "Shopping"});
            }

            List<CheckoutProductsModel> vm = dbProducts.AsQueryable().ProjectTo<CheckoutProductsModel>().ToList();
            vm.ForEach(x => x.Quantity = (byte) cartItems.Single(y => y.ProductId == x.Id).Quantity);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> SecureCheckout(List<string> name, List<byte> quantity)
        {
            return View();
        }

        public async Task<IActionResult> Delivery(List<string> name, List<byte> quantity)
        {
            return View(this.deliveryOptions.GetAll());
        }

        public async Task<IActionResult> CardDetails(List<string> name, List<byte> quantity)
        {
            return View();
        }

        public async Task<IActionResult> Payment(List<string> name, List<byte> quantity)
        {
            return View(this.paymentOptions.GetAll());
        }
    }
}