using System;
using System.Collections.Generic;
using System.Text;

namespace BudgetTracker.Models
{
    public class Transaction
    {
        public decimal Amount { get; set; }
        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }

    }
}
