using Windows.Media.Control;
using BarRaider.SdTools;
using SmtcBridge.MediaSession;

namespace SmtcBridge.StreamDeck;

[PluginActionId("nl.ndat.win-smtc.thumbnail")]
public partial class ThumbnailAction : KeypadBase
{
	private readonly MediaSessionListener _mediaSessionListener;
	private bool _shouldUpdate = true;

	public ThumbnailAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
	{
		var sessionManager = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetResults();
		_mediaSessionListener = new MediaSessionListener(sessionManager, _ => { _shouldUpdate = true; });
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

	public override async void OnTick()
	{
		if (_shouldUpdate)
		{
			var dto = _mediaSessionListener.CurrentMediaPropertiesDto;
			if (dto?.ThumbnailContent == null || dto.ThumbnailContentType == null)
			{
				await Connection.SetDefaultImageAsync();
				await Connection.SetStateAsync(0);
				await Connection.SetTitleAsync(dto?.Title);
			}
			else
			{
				var mediaType = dto.ThumbnailContentType.Split(',').First();
				var dataUrl = $"data:{mediaType};base64,{dto.ThumbnailContent}";
				await Connection.SetImageAsync(dataUrl, 1);
				await Connection.SetStateAsync(1);
				await Connection.SetTitleAsync(null);
			}

			_shouldUpdate = false;
		}
	}

	public override void Dispose()
	{
		_mediaSessionListener.Dispose();
	}
}
