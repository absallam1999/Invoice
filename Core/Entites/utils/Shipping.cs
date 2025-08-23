using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using invoice.Core.Entites;
using invoice.Core.Enums;
using Stripe.Terminal;

namespace invoice.Models.Entites.utils
{
    public class Shipping
    {
        public bool FromStore { get; set; } = true;
        public PaymentType PaymentType { get; set; }

        //Delivery fees
    }
}
