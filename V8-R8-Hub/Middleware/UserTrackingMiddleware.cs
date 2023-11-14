using Dapper;
using System.Security.Cryptography;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Middleware {
	public class UserTrackingMiddleware {
		private readonly RequestDelegate _next;

		public UserTrackingMiddleware(RequestDelegate next) {
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, IDbConnector dbConnector) {
			 if (!context.Session.Keys.Contains("UserId")) {
				var db = dbConnector.GetDbConnection();

				var userId = await db.QuerySingleAsync<int>("INSERT INTO users DEFAULT VALUES RETURNING id");
				context.Session.SetInt32("UserId", userId);

				var key = Convert.ToHexString(RandomNumberGenerator.GetBytes(8));

				await db.ExecuteAsync("INSERT INTO auth_keys (user_id, key) VALUES (@UserId, @Key)", new {
					UserId = userId,
					Key = key
				});

				context.Response.Cookies.Append("AuthCookie", key, new CookieOptions {
					HttpOnly = true
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
