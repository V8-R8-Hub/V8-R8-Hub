namespace V8_R8_Hub.Models.DB {
	public record GameAssetDbModel {
		public required int id;
		public required int game_id;
		public required int file_id;
		public required string path;
	}
}
