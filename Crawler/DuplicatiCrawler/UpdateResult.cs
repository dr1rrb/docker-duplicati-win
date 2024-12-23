using System;
using System.Linq;

namespace Crawler
{
	internal sealed class UpdateResult
	{
		public static UpdateResult NotChanged(string channel) => new UpdateResult
		{
			Channel = channel,
			Result = "not_changed",
			Message = $"Release of channel '{channel}' did not changed."
		};

		public static UpdateResult Succeeded(string channel) => new UpdateResult
		{
			Channel = channel,
			Result = "succeeded",
			Message = $"Successfully updated config of channel '{channel}' and queued a new build."
		};

		public static UpdateResult Failed(string channel, string status, Exception error) => new UpdateResult
		{
			Channel = channel,
			Result = "failed",
			Message = $"Failed to update channel '{channel}', an exception occurred while {status}: \r\n{error.Message}",
			Error = error
		};

		public required string Channel { get; set; }

		public required string Result { get; set; }

		public required string Message { get; set; }
			
		public Exception? Error { get; set; }
	}
}