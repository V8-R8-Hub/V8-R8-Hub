namespace V8_R8_Hub.Models.Internal {
	public record FileData {
		public required string MimeType { get; set; }
		public required string FileName { get; set; }
		public required byte[] ContentBlob { get; set; }
	}
}
