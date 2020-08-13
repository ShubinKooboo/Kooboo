﻿using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Service
{
    public interface ICustomerService : IEcommerceService<Customer>
    {
        Customer Login(string nameOrEmail, string password);

        Customer CreatAccount(string UserName, string email, string password, string firstName, string LastName, string Telephone);

        bool IsUSerNameAvailable(string username);

        bool IsEmailAddressAvailable(string emailaddress);

        Customer GetFromContext(RenderContext context);

        bool Logout();

        bool ChangePassword(string oldPassword, string newPassword);
    }
}
