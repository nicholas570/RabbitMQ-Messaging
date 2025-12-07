namespace Publisher
{
    public readonly record struct OrderPayedMessage(Guid Id, DateTime Date);
}
