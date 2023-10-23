namespace V8_R8_Hub.Models.Internal {
	public record GameBrief {
		public required Guid Guid { get; set; }
		public required string Name { get; set; }
		public required string Description { get; set; }
		public required string ThumbnailGuid { get; set; }
		public required string GameBlobGuid { get; set; }
	}
}
