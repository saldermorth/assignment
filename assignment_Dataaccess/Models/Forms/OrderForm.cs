﻿using assignment_Dataaccess.Models.Enities;

namespace assignment_Dataaccess.Models.Forms
{
    public class OrderForm
    {
        
        public int CustomerID { get; set; }


        //public int OrderItemId { get; set; }
        public ICollection<CartItemUpdate> OrderItem { get; set; } = null!;
        public DateTime OrderDate { get; set; }



    }
}
