using System;

namespace Crawler;

internal sealed class UpdateResult
{
	public static UpdateResult NotChanged(string channel) => new()
	{
		Channel = channel,
		Result = "not_changed",
		Message = $"Release of channel '{channel}' did not changed."
	};

	public static UpdateResult Succeeded(string channel) => new()
	{
		Channel = channel,
		Result = "succeeded",
		Message = $"Successfully updated config of channel '{channel}' and queued a new build."
	};

	public static UpdateResult Failed(string channel, string status, Exception error) => new()
	{
		Channel = channel,
		Result = "failed",
		Message = $"Failed to update channel '{channel}', an exception occurred while {status}: \r\n{error.Message}",
		Error = error
	};

	public required string Channel { get; init; }

	public required string Result { get; init; }

	public required string Message { get; init; }
			
	public Exception? Error { get; init; }
}