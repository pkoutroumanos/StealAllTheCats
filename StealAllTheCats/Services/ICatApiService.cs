namespace StealAllTheCats.Services
{
    public interface ICatApiService
    {
        Task FetchCatsAsync(int count);
    }
}
