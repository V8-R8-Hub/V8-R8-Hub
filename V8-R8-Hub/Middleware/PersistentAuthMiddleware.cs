using Dapper;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Middleware {
	public class PersistentAuthMiddleware {
		private readonly RequestDelegate _next;

		public PersistentAuthMiddleware(RequestDelegate next) {
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, IDbConnector dbConnector) {
			if (
				!context.Session.Keys.Contains("UserId") &&
				context.Request.Cookies.TryGetValue("AuthCookie", out string? authCookieValue) &&
				authCookieValue == null
			) {
				var db = dbConnector.GetDbConnection();

				int? userId = await db.QuerySingleOrDefaultAsync<int?>("""
					SELECT user_id
						FROM auth_keys
						WHERE key = @Key
					""", new {
					Key = authCookieValue
				});

				if (userId == null) {
					context.Response.Cookies.Delete("AuthCookie");
				} else {
					context.Session.SetInt32("UserId", userId.Value);
				}
			}
			await _next(context);
		}
	}

	public static class PersistentAuthMiddlewareExtension {
		public static IApplicationBuilder UsePersistentAuth(
			this IApplicationBuilder builder) {
			return builder.UseMiddleware<PersistentAuthMiddleware>();
		}
	}
}
