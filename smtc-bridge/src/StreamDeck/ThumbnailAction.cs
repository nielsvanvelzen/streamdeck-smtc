using Windows.Media.Control;
using BarRaider.SdTools;
using SmtcBridge.MediaSession;

namespace SmtcBridge.StreamDeck;

[PluginActionId("nl.ndat.win-smtc.thumbnail")]
public partial class ThumbnailAction : KeypadBase
{
	private readonly MediaSessionListener _mediaSessionListener;

	public ThumbnailAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
	{
		var sessionManagerTask = GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
		sessionManagerTask.Wait();
		var sessionManager = sessionManagerTask.GetResults();
		_mediaSessionListener = new MediaSessionListener(sessionManager, dto => Update(dto).Wait());
	}

	public override void KeyPressed(KeyPayload payload)
	{
	}

	public override void KeyReleased(KeyPayload payload)
	{
	}

	public override void ReceivedSettings(ReceivedSettingsPayload payload)
	{
	}

	public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
	{
	}

	public override void OnTick()
	{
	}

	private Task Update(MediaPropertiesDto? dto)
	{
		if (dto?.ThumbnailContent == null || dto.ThumbnailContentType == null)
		{
			return Task.WhenAll(
				Connection.SetDefaultImageAsync(),
				Connection.SetStateAsync(0),
				Connection.SetTitleAsync(dto?.Title)
			);
		}
		else
		{
			var mediaType = dto.ThumbnailContentType.Split(',').First();
			var dataUrl = $"data:{mediaType};base64,{dto.ThumbnailContent}";

			return Task.WhenAll(
				Connection.SetImageAsync(dataUrl, 1),
				Connection.SetStateAsync(1),
				Connection.SetTitleAsync(null)
			);
		}
	}

	public override void Dispose()
	{
		_mediaSessionListener.Dispose();
	}
}
