using Windows.Media.Control;
using SmtcBridge;

var sessionManager = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetResults();
using var mediaSessionListener = new MediaSessionListener(sessionManager, Console.WriteLine);

Thread.Sleep(Timeout.Infinite);
