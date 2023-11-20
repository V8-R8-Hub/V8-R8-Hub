namespace V8_R8_Hub.Models.Internal {
	public record GameBriefQueryModel {
		public required Guid Guid { get; set; }
		public required string Name { get; set; }
		public required string Description { get; set; }
		public required Guid ThumbnailGuid { get; set; }
		public required Guid GameBlobGuid { get; set; }
		public required string? CommaSeperatedTags { get; set; }
	}
}
