using NSubstitute;
using System.Collections.Generic;
using V8_R8_Hub.Models.Exceptions;
using V8_R8_Hub.Models.Internal;
using V8_R8_Hub.Repositories;
using V8_R8_Hub.Services;

namespace UnitTest {
	public class FileServiceTest {
		private readonly IFileRepository _fileRepository;
		private readonly FileService _fileService;

		private readonly ObjectIdentifier _exampleFileIdentifier;
		private readonly FileData _exampleFileData;

		public FileServiceTest() {
			_fileRepository = Substitute.For<IFileRepository>();
			_fileService = new FileService(_fileRepository);

			_exampleFileIdentifier = new ObjectIdentifier() {
				Guid = Guid.Parse("9149f85d-6dbe-49b2-af99-9cb203d8f0f4"),
				Id = 13
			};

			_exampleFileData = new FileData {
				MimeType = "text/plain",
				FileName = "example.txt",
				ContentBlob = new byte[] { 1, 2, 3, 2, 1 }
			};

			_fileRepository
				.CreateFile(
					_exampleFileData.FileName,
					_exampleFileData.MimeType,
					_exampleFileData.ContentBlob
				)
				.Returns(_exampleFileIdentifier);

			_fileRepository.GetFile(_exampleFileIdentifier.Guid)
				.Returns(_exampleFileData);

			_fileRepository.GetFile(_exampleFileIdentifier.Id)
				.Returns(_exampleFileData);
		}

		[Fact]
		public async Task CreateFile_IncorrectMimeType() {
			await Assert.ThrowsAsync<DisallowedMimeTypeException>(async () => {
				await _fileService.CreateFileFrom(
					VirtualFile.From("test.html", "text/html", Array.Empty<byte>()),
					new HashSet<string>() { "text/nothtml", "image/jpeg" }
				);
			});
		}

		[Fact]
		public async Task CreateFile_NoAllowedMimeType() {
			await Assert.ThrowsAsync<DisallowedMimeTypeException>(async () => {
				await _fileService.CreateFileFrom(
					VirtualFile.From("test.html", "text/html", Array.Empty<byte>()),
					new HashSet<string>() { }
				);
			});
		}

		[Fact]
		public async Task CreateFile_InvalidMimeType() {
			await Assert.ThrowsAnyAsync<Exception>(async () => {
				await _fileService.CreateFileFrom(
					VirtualFile.From("test.html", "", Array.Empty<byte>()),
					new HashSet<string>() { "" }
				);
			});
		}

		[Fact]
		public async Task CreateFile_AddedToRepository() {
			await _fileService.CreateFileFrom(VirtualFile.From(
				_exampleFileData.FileName,
				_exampleFileData.MimeType,
				_exampleFileData.ContentBlob
			), new HashSet<string>() { _exampleFileData.MimeType });

			await _fileRepository.Received().CreateFile(
				_exampleFileData.FileName,
				_exampleFileData.MimeType,
				Arg.Is<byte[]>((a) => a.SequenceEqual(_exampleFileData.ContentBlob))
			);
		}
	}
}