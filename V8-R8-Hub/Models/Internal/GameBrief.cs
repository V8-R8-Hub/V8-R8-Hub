﻿namespace V8_R8_Hub.Models.Internal {
	public record GameBrief {
		public required Guid Guid { get; set; }
		public required string Name { get; set; }
		public required string Description { get; set; }
		public required Guid ThumbnailGuid { get; set; }
		public required Guid GameBlobGuid { get; set; }
		public required IEnumerable<string> Tags { get; set; }
	}
}
