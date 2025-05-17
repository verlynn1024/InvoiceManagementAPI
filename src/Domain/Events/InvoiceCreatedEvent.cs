namespace InvoiceManagementAPI.Domain.Events;

public class InvoiceCreatedEvent : BaseEvent
{
    public InvoiceCreatedEvent(Invoice invoice)
    {
        Invoice = invoice;
    }

    public Invoice Invoice { get; }
}
