﻿using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.Service;
using System;

namespace Kooboo.Sites.Ecommerce
{
    public class CustomerService : ServiceBase<Customer>, ICustomerService
    {
        public Customer Login(string nameOrEmail, string password)
        { 
            var customer = this.Repo.Get(nameOrEmail);
            if (customer != null)
            { 
                bool loginok = Service.PasswordService.Verify(password, customer.Password);
                if (loginok)
                {
                    //write the cookie. 
                    this.Context.Response.AppendCookie(Constants.CustomerCookieName, customer.Id.ToString(), DateTime.Now.AddDays(30));
                    return customer;
                }
                else
                {
                    return null;
                }
            }

            if (nameOrEmail.Contains("@"))
            {
                var hash = Lib.Security.Hash.ComputeGuidIgnoreCase(nameOrEmail);
                customer = this.Repo.Query.Where(o => o.EmailHash == hash).FirstOrDefault();
                if (customer != null)
                {
                    var passwordhash = Lib.IOC.Service.GetSingleTon<IPasswordHash>();
                    bool loginok = passwordhash.Verify(password, customer.Password);
                    if (loginok)
                    {
                        //write the cookie. 
                        this.Context.Response.AppendCookie(Constants.CustomerCookieName, customer.Id.ToString(), DateTime.Now.AddDays(30));
                        return customer;
                    }

                }
            }
             

            return null;
        }

        public Customer CreatAccount(string UserName, string email, string password, string firstName, string LastName, string Telephone)
        {

            if (!IsUSerNameAvailable(UserName) || !IsEmailAddressAvailable(email))
            {
                var messge = Data.Language.Hardcoded.GetValue("Username or email address already exists", this.Context);
                throw new Exception(messge);
            } 

            Customer newcus = new Customer()
            {
                Name = UserName,
                EmailAddress = email,
                FirstName = firstName,
                LastName = LastName,
                Telephone = Telephone,
                Password = Service.PasswordService.Hash(password)
            };

            this.Repo.AddOrUpdate(newcus);

            return newcus;
        }

        public bool IsUSerNameAvailable(string username)
        {
            var find = this.Repo.Query.Where(o => o.Name == username).FirstOrDefault();
            return find == null;
        }

        public bool IsEmailAddressAvailable(string emailaddress)
        {
            var hash = Lib.Security.Hash.ComputeGuidIgnoreCase(emailaddress);

            var find = this.Repo.Query.Where(o => o.EmailHash == hash).FirstOrDefault();

            return find == null;
        }

        public Customer GetFromContext(RenderContext context)
        {
            return initCustomer(context); 
        }
         
        private Customer initCustomer(RenderContext context)
        {
            if (context.Request.Cookies.ContainsKey(Constants.CustomerCookieName))
            {
                var service = ServiceProvider.GetService<ICustomerService>(context);
                if (context.Request.Cookies.TryGetValue(Constants.CustomerCookieName, out string cookie))
                {
                    if (System.Guid.TryParse(cookie, out Guid customerid))
                    {
                        var customer = service.Get(customerid);
                        if (customer != null)
                        {
                            return customer;
                        }
                    }
                }
            }

            else if (context.Request.Cookies.ContainsKey(Constants.CustomerTempCookieName))
            {
                var idvalue = context.Request.Cookies[Constants.CustomerTempCookieName]; 
                if (System.Guid.TryParse(idvalue, out Guid guidvalue))
                {
                    var tempuser = new Customer() { NoLogin = true };
                    tempuser.Id = guidvalue;
                    return tempuser;
                }
            }
            var newtemp = new Customer() { NoLogin = true, Id = Lib.Helper.IDHelper.TempGuid() };
            context.Response.AppendCookie(Constants.CustomerTempCookieName, newtemp.Id.ToString(), DateTime.Now.AddDays(10));
            return newtemp;
        }
    }
}
