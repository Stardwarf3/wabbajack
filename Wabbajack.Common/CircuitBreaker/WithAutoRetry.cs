﻿using System;
using System.Threading.Tasks;

namespace Wabbajack.Common
{
    public static class CircuitBreaker
    {
        public static TimeSpan DEFAULT_DELAY = TimeSpan.FromMilliseconds(100);
        public static int DEFAULT_DELAY_MULTIPLIER = 2;
        public static int DEFAULT_RETRIES = 5;

        public static async ValueTask<TR> WithAutoRetry<TR, TE>(Func<ValueTask<TR>> f, TimeSpan? delay = null, int? multipler = null, int? maxRetries = null) where TE : Exception
        {
            int retries = 0;
            delay ??= DEFAULT_DELAY;
            multipler ??= DEFAULT_DELAY_MULTIPLIER;
            maxRetries ??= DEFAULT_RETRIES;

            TOP:
            try
            {
                return await f();
            }
            catch (TE ex)
            {
                retries += 1;
                if (retries > maxRetries)
                    throw;
                Utils.Log($"(Retry {retries} of {maxRetries}), got exception {ex.Message}, waiting {delay.Value.TotalMilliseconds}ms");
                await Task.Delay(delay.Value);
                delay = delay * multipler;
                goto TOP;
            }
        }

    }
}
