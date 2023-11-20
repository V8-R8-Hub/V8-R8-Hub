using Microsoft.AspNetCore.Mvc;
using V8_R8_Hub.Repositories;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Controllers {
	[Route("[Controller]")]
	[ApiController]
	public class PublicFileController : ControllerBase {
		private readonly IFileRepository _fileRepository;

		public PublicFileController(IFileRepository fileRepository) {
			_fileRepository = fileRepository;
		}

		/// <summary>
		/// Gets the file with the given guid
		/// </summary>
		/// <response code="200">Success and the file blob is the response</response>
		/// <response code="404">The file does not exist</response>
		[HttpGet("{fileGuid:guid}", Name = "GetFile")]
		[ProducesResponseType(typeof(FileContentResult), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetFile(Guid fileGuid) {
			var file = await _fileRepository.GetFile(fileGuid);
			if (file == null) {
				return NotFound();
			}
			return File(file.ContentBlob, file.MimeType, file.FileName);
		}
	}
}
	