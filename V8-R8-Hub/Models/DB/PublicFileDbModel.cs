namespace V8_R8_Hub.Models.DB {
	public record PublicFileDbModel {
		public required int Id { get; set; }
		public required Guid PublicId { get; set; }
		public required string FileName { get; set; }
		public required string MimeType { get; set; }
		public required byte[] ContentBlob { get; set; }
	}
}
