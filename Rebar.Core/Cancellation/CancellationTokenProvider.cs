using System;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace Rebar.Core.Cancellation
{
    public class CancellationTokenProvider : ICancellationTokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CancellationTokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public CancellationToken GetCancellationToken()
        {
            return _httpContextAccessor.HttpContext.RequestAborted;
        }
    }
}
