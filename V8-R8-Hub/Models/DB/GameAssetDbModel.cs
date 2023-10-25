namespace V8_R8_Hub.Models.DB {
	public record GameAssetDbModel {
		public required int Id;
		public required int GameId;
		public required int FileId;
		public required string Path;
	}
}
