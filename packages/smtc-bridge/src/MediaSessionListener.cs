﻿using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.Media.Control;

namespace SmtcBridge;

public class MediaSessionListener : IDisposable
{
	private static readonly JsonSerializerOptions? JsonSerializerOptions = new()
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
		TypeInfoResolver = SourceGenerationContext.Default,
		PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
	};

	private readonly GlobalSystemMediaTransportControlsSessionManager _sessionManager;
	private readonly Action<string> _callback;
	private GlobalSystemMediaTransportControlsSession? _currentSession;
	private MediaPropertiesDto? _currentMediaPropertiesDto;

	public MediaSessionListener(GlobalSystemMediaTransportControlsSessionManager sessionManager,
		Action<string> callback)
	{
		_sessionManager = sessionManager;
		_callback = callback;

		SetSession(sessionManager.GetCurrentSession());
		sessionManager.CurrentSessionChanged += OnSessionChanged;
	}

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
		if (dto == _currentMediaPropertiesDto) return;

		var json = JsonSerializer.Serialize(dto, JsonSerializerOptions);
		_callback.Invoke(json);

		_currentMediaPropertiesDto = dto;
	}

	public void Dispose()
	{
		SetSession(null);
		_sessionManager.CurrentSessionChanged -= OnSessionChanged;
	}
}
