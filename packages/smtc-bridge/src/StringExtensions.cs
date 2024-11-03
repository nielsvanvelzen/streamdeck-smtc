namespace SmtcBridge;

public static class StringExtensions
{
	public static string? NullIfBlank(this string source) => string.IsNullOrWhiteSpace(source) ? null : source;
}
