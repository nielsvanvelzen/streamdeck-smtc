using System.Security.Cryptography;
using Windows.Media;
using Windows.Media.Control;
using Windows.Storage.Streams;
using SmtcBridge.Util;

namespace SmtcBridge.MediaSession;

public record MediaPropertiesDto(
	string? AlbumArtist = null,
	string? AlbumTitle = null,
	int AlbumTrackCount = 0,
	string? Artist = null,
	string[]? Genres = null,
	MediaPlaybackType PlaybackType = MediaPlaybackType.Unknown,
	string? Subtitle = null,
	string? Title = null,
	int TrackNumber = 0,
	string? ThumbnailContentType = null,
	string? ThumbnailContent = null
)
{
	public static readonly MediaPropertiesDto Empty = new();

	public static async Task<MediaPropertiesDto> FromMediaProperties(
		GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties)
	{
		using var thumbnail =
			mediaProperties.Thumbnail != null ? await mediaProperties.Thumbnail.OpenReadAsync() : null;
		return new MediaPropertiesDto
		(
			AlbumArtist: mediaProperties.AlbumArtist.NullIfBlank(),
			AlbumTitle: mediaProperties.AlbumTitle.NullIfBlank(),
			AlbumTrackCount: mediaProperties.AlbumTrackCount,
			Artist: mediaProperties.Artist.NullIfBlank(),
			Genres: mediaProperties.Genres.ToArray(),
			PlaybackType: mediaProperties.PlaybackType ?? MediaPlaybackType.Unknown,
			Subtitle: mediaProperties.Subtitle.NullIfBlank(),
			ThumbnailContentType: thumbnail?.ContentType.NullIfBlank(),
			ThumbnailContent: thumbnail != null ? ReadBase64Thumbnail(thumbnail) : null,
			Title: mediaProperties.Title.NullIfBlank(),
			TrackNumber: mediaProperties.TrackNumber
		);
	}

	private static string ReadBase64Thumbnail(IRandomAccessStreamWithContentType thumbnail)
	{
		using var readStream = thumbnail.AsStreamForRead();
		using var cryptoStream = new CryptoStream(readStream, new ToBase64Transform(), CryptoStreamMode.Read);
		using var reader = new StreamReader(cryptoStream);
		return reader.ReadToEnd();
	}
}
