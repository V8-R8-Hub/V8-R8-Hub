namespace V8_R8_Hub.Models.Response {
	public record GameBriefResponse {
		public required Guid Guid { get; set; }
		public required string Name { get; set; }
		public required string Description { get; set; }
		public required string ThumbnailUrl { get; set; }
		public required string GameBlobUrl { get; set; }
		public required List<string> Tags { get; set; }
	}
}
