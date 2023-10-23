using System.Data;
using Dapper;
using V8_R8_Hub.Models.DB;
using V8_R8_Hub.Models.Internal;

namespace V8_R8_Hub.Services
{
    public interface IPublicFileService {
		Task<CreatedFileIdentifierDbModel> CreateFile(string fileName, string mimeType, byte[] contentBlob);
		Task<FileData?> GetFile(Guid guid);
	}

	public class PublicFileService : IPublicFileService {
		private readonly IDbConnection _connection;
		
		public PublicFileService(IDbConnector connector) {
			_connection = connector.GetDbConnection();
		}

		public async Task<CreatedFileIdentifierDbModel> CreateFile(string fileName, string mimeType, byte[] contentBlob) {
			return await _connection.QuerySingleAsync<CreatedFileIdentifierDbModel>(@"
				INSERT INTO public_files (file_name, mime_type, content_blob)
					VALUES (@FileName, @MimeType, @ContentBlob)
				RETURNING id, public_id;
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
	}
}
