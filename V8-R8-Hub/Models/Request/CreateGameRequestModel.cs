namespace V8_R8_Hub.Models.Request
{
    public record CreateGameRequestModel
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required IFormFile GameFile { get; set; }
        public required IFormFile ThumbnailFile { get; set; }
    }
}
