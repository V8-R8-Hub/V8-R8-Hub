namespace V8_R8_Hub.Models.DB {
	public record GameDbModel {
		public required int Id { get; set; }
		public required Guid PublicId { get; set; }
		public required string Name { get; set; }
		public required string Description { get; set; }
		public required int ThumbnailFileId { get; set; }
	}
}
