using Dapper;
using System.Data;
using System.Data.Common;
using V8_R8_Hub.Models.Internal;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Repositories {
	public interface IFileRepository {
		Task<ObjectIdentifier> CreateFile(string fileName, string mimeType, byte[] contentBlob);
		Task<FileData?> GetFile(Guid guid);
		Task<FileData?> GetFile(int id);
	}

	public class FileRepository : IFileRepository {
		private readonly IDbConnection _connection;

		public FileRepository(IDbConnector connector) {
			_connection = connector.GetDbConnection();
		}

		public async Task<ObjectIdentifier> CreateFile(string fileName, string mimeType, byte[] contentBlob) {
			return await _connection.QuerySingleAsync<ObjectIdentifier>(@"
				INSERT INTO public_files (file_name, mime_type, content_blob)
					VALUES (@FileName, @MimeType, @ContentBlob)
				RETURNING id, public_id as guid;
			", new {
				FileName = fileName,
				MimeType = mimeType,
				ContentBlob = contentBlob
			});
		}

		public async Task<FileData?> GetFile(Guid guid) {
			return await _connection.QuerySingleOrDefaultAsync<FileData>(@"
				SELECT mime_type, file_name, content_blob
					FROM public_files
					WHERE public_id = @PublicId;
			", new {
				PublicId = guid
			});
		}

		public async Task<FileData?> GetFile(int id) {
			return await _connection.QuerySingleOrDefaultAsync<FileData>(@"
				SELECT mime_type, file_name, content_blob
					FROM public_files
					WHERE id = @Id;
			", new {
				Id = id
			});
		}
	}
}
