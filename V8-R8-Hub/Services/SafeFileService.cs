using System.Net.Http.Headers;
using V8_R8_Hub.Models.DB;
using V8_R8_Hub.Models.Exceptions;
using V8_R8_Hub.Models.Internal;

namespace V8_R8_Hub.Services {
	public interface ISafeFileService {
		Task<ObjectIdentifier> CreateFileFrom(VirtualFile file, ISet<string> allowedMimeTypes);
	}
	public class SafeFileService : ISafeFileService {
		private readonly IPublicFileService _fileService;
		private readonly ILogger<SafeFileService> _logger;

		public SafeFileService(ILogger<SafeFileService> logger, IPublicFileService fileService) {
			_logger = logger;
			_fileService = fileService;
		}

		public async Task<ObjectIdentifier> CreateFileFrom(VirtualFile file, ISet<string> allowedMimeTypes) {
			if (file.MimeType == null || !allowedMimeTypes.Contains(file.MimeType)) {
				throw new DisallowedMimeTypeException("Mime type of " + file.MimeType + " is not allowed");
			}
			using (var memoryStream = new MemoryStream()) {
				using var stream = file.GetStream();
				stream.CopyTo(memoryStream);
				return await _fileService.CreateFile(file.FileName, file.MimeType, memoryStream.ToArray());
			}
		}
	}
}
