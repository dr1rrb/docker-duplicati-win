using System;

namespace Crawler.Extensions;

internal static class StringExtensions
{
	public static string TrimStart(this string text, string value, StringComparison comparison)
		=> text.StartsWith(value, comparison) 
			? text.Substring(value.Length) 
			: text;
}