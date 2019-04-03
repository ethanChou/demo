using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpBroker
{
    public static class HotlinkingExtensions
    {
        public static IApplicationBuilder UseHotlinkingPreventionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HotlinkingMiddleware>();
        }
    }
}
