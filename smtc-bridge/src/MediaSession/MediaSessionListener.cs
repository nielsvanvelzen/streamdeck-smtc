using Windows.Media.Control;

namespace SmtcBridge.MediaSession;

public class MediaSessionListener : IDisposable
{
	private readonly GlobalSystemMediaTransportControlsSessionManager _sessionManager;
	private readonly Action<MediaPropertiesDto> _callback;
	private GlobalSystemMediaTransportControlsSession? _currentSession;
	private MediaPropertiesDto? _currentMediaPropertiesDto = MediaPropertiesDto.Empty;

	public MediaSessionListener(GlobalSystemMediaTransportControlsSessionManager sessionManager,
		Action<MediaPropertiesDto> callback)
	{
		_sessionManager = sessionManager;
		_callback = callback;

		SetSession(sessionManager.GetCurrentSession());
		sessionManager.CurrentSessionChanged += OnSessionChanged;
	}

	public MediaPropertiesDto? CurrentMediaPropertiesDto  => _currentMediaPropertiesDto;

	private void OnSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender,
		CurrentSessionChangedEventArgs args)
	{
		var session = sender.GetCurrentSession();
		if (session != null)
		{
			session.MediaPropertiesChanged += OnMediaPropertiesChanged;
		}
	}

	private async void SetSession(GlobalSystemMediaTransportControlsSession? session)
	{
		if (_currentSession != null)
		{
			_currentSession.MediaPropertiesChanged -= OnMediaPropertiesChanged;
		}

		if (session != null)
		{
			var mediaProperties = await session.TryGetMediaPropertiesAsync();
			await UpdateMediaProperties(mediaProperties);
			session.MediaPropertiesChanged += OnMediaPropertiesChanged;
		}
		else
		{
			SetDto(MediaPropertiesDto.Empty);
		}

		_currentSession = session;
	}

	private async void OnMediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender,
		MediaPropertiesChangedEventArgs args)
	{
		var mediaProperties = await sender.TryGetMediaPropertiesAsync();
		await UpdateMediaProperties(mediaProperties);
	}

	private async Task UpdateMediaProperties(
		GlobalSystemMediaTransportControlsSessionMediaProperties? mediaProperties)
	{
		var dto = mediaProperties == null
			? MediaPropertiesDto.Empty
			: await MediaPropertiesDto.FromMediaProperties(mediaProperties);
		SetDto(dto);
	}

	private void SetDto(MediaPropertiesDto dto)
	{
		if (dto == _currentMediaPropertiesDto) return;
		_callback.Invoke(dto);
		_currentMediaPropertiesDto = dto;
	}

	public void Dispose()
	{
		SetSession(null);
		_sessionManager.CurrentSessionChanged -= OnSessionChanged;
	}
}
