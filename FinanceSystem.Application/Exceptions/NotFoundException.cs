namespace FinanceSystem.Application.Exceptions
{
    public class NotFoundException : InvalidOperationException
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
