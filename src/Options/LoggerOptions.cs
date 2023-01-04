using Microsoft.Extensions.Logging.Console;

namespace Dev.Sepp.Idler.Console.Options;

public sealed class LoggerOptions : ConsoleFormatterOptions
{
	public bool UseColor { get; set; } = true;
}
