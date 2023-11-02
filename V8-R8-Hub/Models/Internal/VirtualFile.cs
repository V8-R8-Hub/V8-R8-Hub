using System.Net.Http.Headers;

namespace V8_R8_Hub.Models.Internal {
	public class VirtualFile {
		public required string FileName { get; set; }
		public string? MimeType { get; set; }
		private Func<Stream> _openStreamFunc;
		public static VirtualFile From(IFormFile file) {
			var parsedMediaType = MediaTypeHeaderValue.Parse(file.ContentType);
			return new VirtualFile {
				FileName = file.FileName,
				MimeType = parsedMediaType.MediaType,
				_openStreamFunc = file.OpenReadStream
			};
		}

		public Stream GetStream() {
			return _openStreamFunc();
		}
	}
}
