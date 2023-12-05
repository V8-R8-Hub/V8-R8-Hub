using Dapper;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using V8_R8_Hub.Repositories;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Middleware {
	public class UserTrackingMiddleware {
		private readonly RequestDelegate _next;

		public UserTrackingMiddleware(RequestDelegate next) {
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, IUserRepository userRepository) {
			 if (
				!context.Session.Keys.Contains("UserId") || 
				context.Request.Cookies["V8R8HubCookieAccept"] != null
			) {
				var user = await userRepository.CreateUser();
				context.Session.SetInt32("UserId", user.Id);

				var key = await userRepository.CreateAuthKey(user.Id);

				context.Response.Cookies.Append("AuthCookie", key, new CookieOptions {
					HttpOnly = true,
					MaxAge = TimeSpan.FromDays(400)
				});
			}
			await _next(context);
		}
	}

	public static class UserMiddlewareExtension {
		public static IApplicationBuilder UseUserTracking(
			this IApplicationBuilder builder) {
			return builder.UseMiddleware<UserTrackingMiddleware>();
		}

		/// <summary>
		/// Gets the user id, valid for any endpoint that has the UserTrackingMiddleware enabled
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static int GetUserId(this HttpContext context) {
			return context.Session.GetInt32("UserId") ?? throw new ArgumentException("The UserMiddleware is not enabled");
		}
	}
}
