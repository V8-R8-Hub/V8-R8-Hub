using System.Net.Http.Headers;
using V8_R8_Hub.Models.DB;
using V8_R8_Hub.Models.Exceptions;
using V8_R8_Hub.Models.Internal;
using V8_R8_Hub.Repositories;

namespace V8_R8_Hub.Services {
	public interface IFileService {
		Task<ObjectIdentifier> CreateFileFrom(VirtualFile file, ISet<string> allowedMimeTypes);
	}
	public class FileService : IFileService {
		private readonly IFileRepository _fileRepository;

		public FileService(IFileRepository fileRepository) {
			_fileRepository = fileRepository;

		}

		public async Task<ObjectIdentifier> CreateFileFrom(VirtualFile file, ISet<string> allowedMimeTypes) {
			if (file.MimeType == null || !allowedMimeTypes.Contains(file.MimeType)) {
				throw new DisallowedMimeTypeException(file.MimeType, "Mime type of " + file.MimeType + " is not allowed");
			}
			using (var memoryStream = new MemoryStream()) {
				using var stream = file.GetStream();
				stream.CopyTo(memoryStream);
				return await _fileRepository.CreateFile(file.FileName, file.MimeType, memoryStream.ToArray());
			}
		}

		public async Task<FileData?> GetFile(Guid guid) {
			return await _fileRepository.GetFile(guid);
		}
	}
}
