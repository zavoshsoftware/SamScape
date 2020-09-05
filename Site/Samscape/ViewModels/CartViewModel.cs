﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class CartViewModel : _BaseViewModel
    {
        public List<ProductInCart> Products { get; set; }
        public string SubTotal { get; set; }
        public string Total { get; set; }
        public string DiscountAmount { get; set; }
        public string ShipmentAmount { get; set; }
        public List<Province> Provinces { get; set; }
    }

    public class ProductInCart
    {
        Helpers.BaseViewModelHelper _baseViewModel = new Helpers.BaseViewModelHelper();

        public Product Product { get; set; }
        public int Quantity { get; set; }
        public string Amount
        {

            get
            {
               
                    if (Product.IsInPromotion)
                        return Product.DiscountAmount.Value.ToString("n0") + " تومان";

                    return Product.Amount.ToString("n0") + " تومان";
               
            }
        }
        public string RowAmount
        {
            get
            {
                
                    if (Product.IsInPromotion && Product.DiscountAmount != null)
                        return (Product.DiscountAmount.Value * Quantity).ToString("n0") + " تومان";

                    return (Product.Amount * Quantity).ToString("n0") + " تومان";
            }
        }

       
    }
}