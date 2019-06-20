using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stripe;

namespace new_websub.Models
{
    public class SubscriptionList
    {
        public List<TaxRate> DefaultTaxRates { get; set; }
        public Discount Discount { get; set; }
        public DateTime? EndedAt { get; set; }
        public StripeList<SubscriptionItem> Items { get; set; }
        public string LatestInvoiceId { get; set; }
        public Invoice LatestInvoice { get; set; }
        public bool Livemode { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public Plan Plan { get; set; }
        public long? Quantity { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? StartDate { get; set; }
        public string Status { get; set; }
        public decimal? TaxPercent { get; set; }
        public SubscriptionTransferData TransferData { get; set; }
        public IPaymentSource DefaultSource { get; set; }
        public DateTime? TrialEnd { get; set; }
        public string DefaultSourceId { get; set; }
        public string DefaultPaymentMethodId { get; set; }
        public string Id { get; set; }
        public string Object { get; set; }
        public decimal? ApplicationFeePercent { get; set; }
        public Billing? Billing { get; set; }
        public DateTime? BillingCycleAnchor { get; set; }
        public SubscriptionBillingThresholds BillingThresholds { get; set; }
        public DateTime? CancelAt { get; set; }
        public bool CancelAtPeriodEnd { get; set; }
        public DateTime? CanceledAt { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? CurrentPeriodEnd { get; set; }
        public DateTime? CurrentPeriodStart { get; set; }
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
        public long? DaysUntilDue { get; set; }
        public PaymentMethod DefaultPaymentMethod { get; set; }
        public DateTime? TrialStart { get; set; }
    }
}
