using Dev.Sepp.Idler.Console.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dev.Sepp.Idler.Console.Logging;

public sealed class CustomConsoleFormatter : ConsoleFormatter, IDisposable
{
	private LoggerOptions _loggerOptions;
	private readonly IDisposable? _optionsReloadToken;

	public CustomConsoleFormatter(IOptionsMonitor<LoggerOptions> options)
		: base(nameof(CustomConsoleFormatter))
	{
		_loggerOptions = options.CurrentValue ?? throw new ArgumentNullException(nameof(options));
		_optionsReloadToken = options.OnChange(ReloadLoggerOptions);
	}

	public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
	{
		var message = logEntry.Formatter(logEntry.State, logEntry.Exception);

		if (logEntry.Exception is null && string.IsNullOrWhiteSpace(message))
		{
			return;
		}

		var logLevelString = GetLogLevelString(logEntry.LogLevel);

		if (_loggerOptions.UseColor)
		{
			var logLevelColor = GetLogLevelConsoleColors(logEntry.LogLevel);
			var colorCode = GetForegroundColorEscapeCode(logLevelColor);
			textWriter.Write(colorCode);
		}

		if (string.IsNullOrWhiteSpace(_loggerOptions.TimestampFormat) is false)
		{
			var dateTimeOffset = GetCurrentDateTime();
			var timestamp = dateTimeOffset.ToString(_loggerOptions.TimestampFormat);
			textWriter.Write(timestamp);
		}

		textWriter.Write(" [{0}] ", logLevelString);
		textWriter.Write(message);
		textWriter.Write(Environment.NewLine);
	}

	public void Dispose()
	{
		_optionsReloadToken?.Dispose();
	}

	private DateTimeOffset GetCurrentDateTime()
	{
		return _loggerOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
	}

	private void ReloadLoggerOptions(LoggerOptions options)
	{
		ArgumentNullException.ThrowIfNull(options);
		_loggerOptions = options;
	}

	private static string GetLogLevelString(LogLevel logLevel) => logLevel switch
	{
		LogLevel.Trace => "TRC",
		LogLevel.Debug => "DBG",
		LogLevel.Information => "INF",
		LogLevel.Warning => "WRN",
		LogLevel.Error => "ERR",
		LogLevel.Critical => "CRT",
		_ => throw new ArgumentOutOfRangeException(nameof(logLevel))
	};

	private static ConsoleColor GetLogLevelConsoleColors(LogLevel logLevel) => logLevel switch
	{
		LogLevel.Trace => ConsoleColor.DarkGray,
		LogLevel.Debug => ConsoleColor.Gray,
		LogLevel.Information => ConsoleColor.White,
		LogLevel.Warning => ConsoleColor.Yellow,
		LogLevel.Error => ConsoleColor.DarkYellow,
		LogLevel.Critical => ConsoleColor.Red,
		_ => ConsoleColor.Black
	};

	private static string GetForegroundColorEscapeCode(ConsoleColor color) => color switch
	{
		ConsoleColor.Black => "\x1B[30m",
		ConsoleColor.DarkRed => "\x1B[31m",
		ConsoleColor.DarkGreen => "\x1B[32m",
		ConsoleColor.DarkYellow => "\x1B[33m",
		ConsoleColor.DarkBlue => "\x1B[34m",
		ConsoleColor.DarkMagenta => "\x1B[35m",
		ConsoleColor.DarkCyan => "\x1B[36m",
		ConsoleColor.Gray => "\x1B[37m",
		ConsoleColor.Red => "\x1B[1m\x1B[31m",
		ConsoleColor.Green => "\x1B[1m\x1B[32m",
		ConsoleColor.Yellow => "\x1B[1m\x1B[33m",
		ConsoleColor.Blue => "\x1B[1m\x1B[34m",
		ConsoleColor.Magenta => "\x1B[1m\x1B[35m",
		ConsoleColor.Cyan => "\x1B[1m\x1B[36m",
		ConsoleColor.White => "\x1B[1m\x1B[37m",
		_ => "\x1B[1m\x1B[37m"
	};
}
