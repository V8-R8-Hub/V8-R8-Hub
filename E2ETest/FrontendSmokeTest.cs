using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using V8_R8_Hub.Models.Response;

namespace E2ETest {
	public class FrontendSmokeTest {
		[Fact]
		public async Task Index_Get_SmokeTest() {
			await using var application = new WebApplicationFactory<Program>();
			var client = application.CreateDefaultClient();

			var response = await client.GetAsync("/");

			Assert.True(response.StatusCode == HttpStatusCode.OK);
		}
	}
}