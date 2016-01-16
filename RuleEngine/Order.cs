using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleEngine {
    /// <summary>
    /// Order representation
    /// Feel free to add additional fields to the order which you feel are important
    /// </summary>
    public class Order {
        public string Id { get; set; }
        public int Value { get; set; }
        public State State { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Total { get; set; }
        public bool IsSpecialInterest { get; set; }
        public DateTime CaptureDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public IEnumerable<OrderItem> Items { get; set; }
        public string CardNumber { get; set; }

        public Order(string id, int value, State state, decimal total, PaymentMethod paymentMethod) {
            Id = id;
            Value = value;
            State = state;
            Total = total;
            PaymentMethod = paymentMethod;
        }
    }

    /// <summary>
    /// State of an item
    /// </summary>
    public enum State {
        Open,
        InProgress,
        Closed,
    }

    public enum PaymentMethod {
        Cash,
        CreditCard,
        DebitCard
    }

    public class OrderItem {
        public int ItemNumber { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}
