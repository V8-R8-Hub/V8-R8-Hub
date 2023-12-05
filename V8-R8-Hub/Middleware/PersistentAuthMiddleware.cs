using Dapper;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using V8_R8_Hub.Repositories;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Middleware {
	public class PersistentAuthMiddleware {
		private readonly RequestDelegate _next;

		public PersistentAuthMiddleware(RequestDelegate next) {
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, UserRepository userRepository) {
			if (
				!context.Session.Keys.Contains("UserId") &&
				context.Request.Cookies.TryGetValue("AuthCookie", out string? authCookieValue)
			) {
				var user = await userRepository.GetUserFromAuthKey(authCookieValue);

				if (user == null) {
					context.Response.Cookies.Delete("AuthCookie");
				} else {
					context.Session.SetInt32("UserId", user.Id);
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
