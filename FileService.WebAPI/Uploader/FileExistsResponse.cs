namespace FileService.WebAPI.Uploader
{
    public record FileExistsResponse(bool IsExists, Uri? Url);
}
