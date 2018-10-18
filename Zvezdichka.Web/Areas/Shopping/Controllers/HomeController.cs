﻿using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Zvezdichka.Common;
using Zvezdichka.Services.Contracts.Entity;
using Zvezdichka.Web.Areas.Api.Models.CartItems;
using Zvezdichka.Web.Infrastructure.Extensions;
using Zvezdichka.Web.Models.ShoppingViewModels;
using Zvezdichka.Web.Services;

namespace Zvezdichka.Web.Areas.Shopping.Controllers
{
    public class HomeController : ShoppingBaseController
    {
        private readonly IProductsDataService products;
        private readonly IShoppingCartManager shoppingCartManager;

        public HomeController(IProductsDataService products, IShoppingCartManager shoppingCartManager)
        {
            this.products = products;
            this.shoppingCartManager = shoppingCartManager;
        }

        public IActionResult Cart()
        {
            var shoppingCartId = this.HttpContext.Session.GetShoppingCartId();

            var items = this.shoppingCartManager.GetCartItems(shoppingCartId);
            var itemIds = items.Select(x => x.ProductId).ToList();

            var itemQuantities = items.ToDictionary(i => i.ProductId, i => i.Quantity);

            var itemsWithDetails =
                this.products.GetAll()
                    .Where(x => itemIds.Contains(x.Id))
                    .AsQueryable()
                    .ProjectTo<CartItemViewModel>()
                    .ToList();

            itemsWithDetails.ForEach(x => x.Quantity = itemQuantities[x.Id]);

            return View(itemsWithDetails);
        }

        public PartialViewResult SidebarCart()
        {
            var shoppingCartId = this.HttpContext.Session.GetShoppingCartId();

            var items = this.shoppingCartManager.GetCartItems(shoppingCartId);
            var itemIds = items.Select(x => x.ProductId).ToList();

            var itemQuantities = items.ToDictionary(i => i.ProductId, i => i.Quantity);

            var itemsWithDetails =
                this.products.GetAll()
                    .Where(x => itemIds.Contains(x.Id))
                    .AsQueryable()
                    .ProjectTo<CartItemViewModel>()
                    .ToList();

            itemsWithDetails.ForEach(x => x.Quantity = itemQuantities[x.Id]);


            return PartialView("../Views/Shared/Components/SidebarShoppingCart/SidebarShoppingCart.cshtml", itemsWithDetails);
        }

        /// <summary>
        /// Handles an Ajax request to add products to cart
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var shoppingCartId = this.HttpContext.Session.GetShoppingCartId();

            //check quantity for this product
            var dbProduct = this.products.GetSingle(p => p.Id == productId);

            if (dbProduct.Stock <= 0 || quantity > dbProduct.Stock)
                return BadRequest(string.Format(CommonConstants.StockAmountExceededForError, dbProduct.Name));

            var cartItem = this.shoppingCartManager.GetCartItem(shoppingCartId, productId);

            if (cartItem == null)
            {
                this.shoppingCartManager.AddToCart(shoppingCartId, productId, quantity);

                return Ok(CommonConstants.SuccessfullyAddedCartItem);
            }

            //check if can add this quantity to the current quantity without exceeding the db stock
            if (cartItem.Quantity + quantity > dbProduct.Stock)
            {
                return BadRequest(CommonConstants.AddingAmountWillExceedOurStock);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            return Ok(CommonConstants.SuccessfullyAddedMoreOfThisItem);
        }

        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var shoppingCartId = this.HttpContext.Session.GetShoppingCartId();
            var cartItems = this.shoppingCartManager.GetCartItems(shoppingCartId);

            var cartItem = cartItems.SingleOrDefault(x => x.ProductId == id);

            if (cartItem == null)
                return NotFound();

            this.shoppingCartManager.RemoveFromCart(shoppingCartId, cartItem.ProductId);

            Success(string.Format(CommonConstants.DeletedCartItemSuccessfully));
            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(CartItemUpdateModel cartItem)
        {
            var shoppingCartId = this.HttpContext.Session.GetShoppingCartId();

            var toUpdate = this.shoppingCartManager.GetCartItems(shoppingCartId)
                .SingleOrDefault(x => x.ProductId == cartItem.Id);

            if (toUpdate == null)
                return NotFound();

            var dbProduct = this.products.GetSingle(x => x.Id == cartItem.Id);

            if (dbProduct.Stock < cartItem.Quantity)
            {
                Danger(CommonConstants.StockAmountExceededError);
                return RedirectToAction(nameof(Cart));
            }

            if (!this.ModelState.IsValid)
            {
                var errorMsg = string.Empty;

                foreach (var modelState in this.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errorMsg += error.ErrorMessage + "\n";
                    }
                }

                Danger(errorMsg);
                return RedirectToAction(nameof(Cart));
            }

            toUpdate.Quantity = cartItem.Quantity;

            Success(CommonConstants.UpdatedCartItemSuccessfully);
            return RedirectToAction(nameof(Cart));
        }
    }
}
