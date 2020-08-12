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
        public Order Create(string addressId)
        {
            Guid id;
            bool parseok = Guid.TryParse(addressId, out id);

            if (parseok)
            {
                var cartservice = ServiceProvider.GetService<CartService>(this.context);
                var cart = cartservice.GetOrCreateCart();
                if (cart.Items.Any())
                {
                    var order = this.service.CreateOrder(cart, id);
                    return order;
                }
            }
            return null;
        }

        public Order Get(string orderId)
        {
            if(Guid.TryParse(orderId, out var id))
            {
                return service.Get(id);
            }
            return null;
        }

        public Order CreateSelected(object[] selected, string addressId)
        {
            Guid addressguid;
            bool parseok = Guid.TryParse(addressId, out addressguid);

            if (parseok)
            {
                List<Guid> selectedCartItem = new List<Guid>();
                foreach (var item in selected)
                {
                    if (item != null)
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
                    var order = this.service.CreateOrder(cart.Items.FindAll(o => selectedCartItem.Contains(o.Id)).ToList(), addressguid);
                    if (order != null)
                    {
                        foreach (var item in order.Items)
                        {
                            cartservice.RemoveItem(item.ProductVariantId);
                        }
                    }
                    return order;
                }
            }

            return null;
        }

        public void DeleteOrder(object[] OrderIds)
        {
            foreach (var OrderId in OrderIds)
            {
                var guid = Lib.Helper.IDHelper.GetGuid(OrderId);
                if (guid != default(Guid))
                {
                    var order = this.service.Get(guid);
                    if (order != null)
                    {
                        order.DeleteByCustomer = true;

                    }

                    this.service.AddOrUpdate(order);
                }
            }
        }

        public void Cancel(object[] OrderIds)
        {
            foreach (var OrderId in OrderIds)
            {
                var guid = Lib.Helper.IDHelper.GetGuid(OrderId);
                if (guid != default(Guid))
                {
                    ChangeOrderStatus(guid, "cancel");
                }
            }
        }

        private void ChangeOrderStatus(Guid OrderId, string status)
        {
            var order = this.service.Get(OrderId);
            if (order != null)
            {
                if (string.Equals(status, "paid", StringComparison.OrdinalIgnoreCase))
                {
                    order.Status = OrderStatus.Paid;
                }

                if (string.Equals(status, "cancel", StringComparison.OrdinalIgnoreCase))
                {
                    if ((int)order.Status < (int)OrderStatus.Paid)
                        order.Status = OrderStatus.Cancel;
                }
            }

            this.service.AddOrUpdate(order);
        }

        public void Paid(object[] OrderIds)
        {
            foreach (var OrderId in OrderIds)
            {
                var guid = Lib.Helper.IDHelper.GetGuid(OrderId);
                if (guid != default(Guid))
                {
                    ChangeOrderStatus(guid, "paid");
                }
            }
        }

        public OrderViewModel[] GetOrderList(int skip, int take)
        {
            var orders = this.service.ListByCustomerId(skip, take)
                  .Where(it => !it.DeleteByCustomer)
                  .Select(it => new OrderViewModel(it, this.context)).ToArray();
            return orders;
        }
    }
}
