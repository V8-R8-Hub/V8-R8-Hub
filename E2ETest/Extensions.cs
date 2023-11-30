using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E2ETest {
	public static class Extensions {
		public static async Task<HttpResponseMessage> PostJsonContent(this HttpClient client, string url, object content) {
			var request = new HttpRequestMessage(HttpMethod.Post, url);
			request.Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
			return await client.SendAsync(request);
		}
	}
}
