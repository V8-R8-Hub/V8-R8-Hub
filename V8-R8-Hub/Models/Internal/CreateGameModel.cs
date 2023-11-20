namespace V8_R8_Hub.Models.Internal {
    public record CreateGameModel {
        public required int GameFileId { get; set; }
        public required int ThumbnailFileId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}
