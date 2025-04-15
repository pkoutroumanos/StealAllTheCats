using Microsoft.Extensions.Primitives;

namespace StealAllTheCats.Infastracture
{
    public interface ICacheTokenProvider
    {
        CancellationChangeToken GetToken();
        void ExpireToken();
    }
}
