using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E2ETest {
	internal static class ProjectHelper {
		public static readonly string SampleGameName = "E2E test game";
		public static readonly string SampleGameDescription = "E2E test game description";
		public static readonly string SampleGameHtmlContent = "<!DOCTYPE html>";
		public static readonly string SampleGameThumbnailFile = "123321";

		public static async Task<Guid> CreateSampleGame(HttpClient client) {
			var form = new MultipartFormDataContent {
				{ new StringContent(SampleGameName), "Name" },
				{ new StringContent(SampleGameDescription), "Description" },
				{ new StringContent(SampleGameHtmlContent, null, "text/html"), "GameFile", "gamefile.html" },
				{ new StringContent(SampleGameThumbnailFile, null, "image/jpeg"), "ThumbnailFile", "mainfile.html" },
			};

			var response = await client.PostAsync("/api/Game", form);
			var deserialized = JsonSerializer.Deserialize<string>(
				await response.Content.ReadAsStringAsync()
			);

			Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
			Assert.NotNull(deserialized);
			Assert.True(Guid.TryParse(deserialized, out var gameGuid));
			return gameGuid;
		}
	}
}
