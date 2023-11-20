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

		public static VirtualFile From(string fileName, string contentType, byte[] blob) {
			var parsedMediaType = MediaTypeHeaderValue.Parse(contentType);
			return new VirtualFile {
				FileName = fileName,
				MimeType = parsedMediaType.MediaType,
				_openStreamFunc = () => new MemoryStream(blob)
			};
		}

		public Stream GetStream() {
			return _openStreamFunc();
		}
	}
}
