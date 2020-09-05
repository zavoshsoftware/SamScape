using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace Eshop.Helpers
{
    public class CodeCreator
    {
        private DatabaseContext db = new DatabaseContext();
        public int? ReturnUserCode()
        {

            User user = db.Users.OrderByDescending(current => current.Code).FirstOrDefault();
            if (user != null)
            {
                return user.Code + 1;
            }
            else
            {
                return 20000;
            }
        }

        public int ReturnOrderCode()
        {

            Order order = db.Orders.OrderByDescending(current => current.Code).FirstOrDefault();
            if (order != null)
            {
                return order.Code + 1;
            }
            else
            {
                return 30000;
            }
        }

    }
}