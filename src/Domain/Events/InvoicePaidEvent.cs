namespace InvoiceManagementAPI.Domain.Events;

public class InvoicePaidEvent : BaseEvent
{
    public InvoicePaidEvent(Invoice invoice)
    {
        Invoice = invoice;
    }

    public Invoice Invoice { get; }
}
