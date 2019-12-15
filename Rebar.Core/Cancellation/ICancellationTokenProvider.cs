using System;
using System.Threading;

namespace Rebar.Core.Cancellation
{
    public interface ICancellationTokenProvider
    {
        CancellationToken GetCancellationToken();
    }
}
