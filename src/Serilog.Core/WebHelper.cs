using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Serilog.Core
{
    public static class WebHelper
    {
        public static void LogWebUsage(string product, string layer, string activityName,
            HttpContext context, Dictionary<string, object> additionalInfo = null)
        {
            var details = GetWebLogDetail(product, layer, activityName, context, additionalInfo);
            CLogger.WriteUsage(details);
        }

        public static void LogWebDiagnostics(string product, string layer, string activityName,
            HttpContext context, Dictionary<string, object> additionalInfo = null)
        {
            var details = GetWebLogDetail(product, layer, activityName, context, additionalInfo);
            CLogger.WriteDiagnostic(details);
        }

        public static void LogWebDiagnostics(string product, string layer, Exception ex, HttpContext context)
        {
            var details = GetWebLogDetail(product, layer, null, context, null);
            details.Exception = ex;

            CLogger.WriteError(details);
        }

        public static LogDetail GetWebLogDetail(string product, string layer, string activityName,
            HttpContext context, Dictionary<string, object> additionalInfo = null)
        {
            var detail = new LogDetail
            {
                Product = product,
                Layer = layer,
                Message = activityName,
                Hostname = Environment.MachineName,
                CorrelationId = Activity.Current?.Id ?? context.TraceIdentifier,
                AdditionalInfo = additionalInfo ?? new Dictionary<string, object>()
            };

            GetUserData(detail);
            GetRequestData(detail, context);

            return detail;
        }

        private static void GetRequestData(LogDetail detail, HttpContext context)
        {
            var request = context.Request;
            if (request != null)
            {
                detail.Location = request.Path;

                detail.AdditionalInfo.Add("UserAgent", request.Headers["User-Agent"]);

                var queriesString = Microsoft.AspNetCore.WebUtilities
                    .QueryHelpers.ParseQuery(request.QueryString.ToString());
                foreach (var key in queriesString.Keys)
                {
                    detail.AdditionalInfo.Add($"QueryString-{key}", queriesString[key]);
                }
            }
        }

        private static void GetUserData(LogDetail detail)
        {
            var userId = "";
            var userName = "";

            var user = ClaimsPrincipal.Current;
            if (user != null)
            {
                var i = 1;
                foreach (var claim in user.Claims)
                {
                    switch (claim.Type)
                    {
                        case ClaimTypes.NameIdentifier:
                            userId = claim.Value;
                            break;
                        case ClaimTypes.Name:
                            userName = claim.Value;
                            break;
                        default:
                            detail.AdditionalInfo.Add($"UserClaim-{i++}-{claim.Type}", claim.Value);
                            break;
                    }
                }
            }

            detail.UserId = userId;
            detail.UserName = userName;
        }
    }
}
