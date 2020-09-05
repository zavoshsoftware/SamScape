using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace Helpers
{
    public static class ShopConfig
    {
        private static DatabaseContext db = new DatabaseContext();
        public static decimal GetFreeShipingLimit()
        {
            ShopConfiguration shopConfiguration =
                db.ShopConfigurations.FirstOrDefault(current => current.Name.ToLower() == "freeshipinglimit");

            if (shopConfiguration != null)
                return Convert.ToDecimal(shopConfiguration.Value);

            return 0;
        }

        public static decimal GetShipingAmount()
        {
            ShopConfiguration shopConfiguration =
                db.ShopConfigurations.FirstOrDefault(current => current.Name.ToLower() == "shipingamount");

            if (shopConfiguration != null)
                return Convert.ToDecimal(shopConfiguration.Value);

            return 0;
        }
        public static string GetZarinpalCode()
        {
            ShopConfiguration shopConfiguration =
                db.ShopConfigurations.FirstOrDefault(current => current.Name.ToLower() == "zarinpal");

            if (shopConfiguration != null)
                return shopConfiguration.Value;

            return string.Empty;
        }
    }
}