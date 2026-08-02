namespace BookDemo.Domain.Exceptions
{
    public abstract class BadRequestException : Exception
    {
        protected BadRequestException(string message) : base(message)
        {

        }

    }
    public sealed class InvalidCategoryPayloadException : BadRequestException
    {
        public InvalidCategoryPayloadException() : base("Category payload cannot be null.")
        {
        }
    }

    public sealed class InvalidBookPayloadException : BadRequestException
    {
        public InvalidBookPayloadException() : base("Book payload cannot be null.")
        {
        }
    }
}
