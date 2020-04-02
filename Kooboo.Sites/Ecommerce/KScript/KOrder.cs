﻿using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.Service;
using Kooboo.Sites.Ecommerce.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Kooboo.Sites.Ecommerce.KScript
{

    public class KOrder
    {
        public RenderContext context { get; set; }

        public CommerceContext commerceContext { get; set; }

        private OrderService service { get; set; }

        public KOrder(RenderContext context)
        {
            this.context = context;
            this.service = Kooboo.Sites.Ecommerce.ServiceProvider.GetService<OrderService>(this.context);
        }

        [Description("Create an order based on current shopping cart items")]
        public Order Create()
        {
            var cartservice = ServiceProvider.GetService<CartService>(this.context);
            var cart = cartservice.GetOrCreateCart();
            if (cart.Items.Any())
            {
                var order = this.service.CreateOrder(cart);
                return order;
            }
            return null;
        }
         
        public Order CreateSelected(object[] selected)
        {
            List<Guid> selectedCartItem = new List<Guid>();
            foreach (var item in selected)
            {
                if (item !=null)
                {
                    var stritem = item.ToString();
                    
                    if (System.Guid.TryParse(stritem, out Guid id))
                    {
                        selectedCartItem.Add(id); 
                    }
                }
            }

            var cartservice = ServiceProvider.GetService<CartService>(this.context);
            var cart = cartservice.GetOrCreateCart();
            if (cart.Items.Any() && selectedCartItem.Any())
            { 
                var order = this.service.CreateOrder(cart.Items.FindAll(o=>selectedCartItem.Contains(o.Id)).ToList());
                if(order!=null)
                {
                    foreach (var item in order.Items)
                    {
                        cartservice.RemoveItem(item.ProductVariantId);
                    }
                }
                return order;
            }

            return null;
        }
         
        public void Paid(object[] OrderIds)
        {
            foreach (var OrderId in OrderIds)
            {
                var guid = Lib.Helper.IDHelper.GetGuid(OrderId);
                if (guid != default(Guid))
                {
                    var order = this.service.Get(guid);
                    if (order != null)
                    {
                        order.Status = OrderStatus.Paid;
                    }

                    this.service.AddOrUpdate(order);
                }
            }
        }

        public OrderViewModel[] GetOrderList(int skip, int take)
        {
            var orders = this.service.ListByCustomerId(skip, take)
                  .Select(it => new OrderViewModel(it, this.context)).ToArray();
            return orders;
        }
    }





}
