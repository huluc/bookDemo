namespace BookDemo.Application.Contracts
{
    public interface IBookCache
    {
        Task InvalidateAsync();
    }
}
