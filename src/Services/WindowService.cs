using Idler.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Idler.Services;

public sealed class WindowService : BackgroundService
{
	private readonly ILogger<WindowService> _logger;
	private readonly ApplicationOptions _options;

	public WindowService(ILogger<WindowService> logger, IOptions<ApplicationOptions> options)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_options = options.Value ?? throw new ArgumentNullException(nameof(options));
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (_options.WindowInterval <= 0)
		{
			return;
		}

		while (stoppingToken.IsCancellationRequested is false)
		{
			var processes = Process.GetProcessesByName(_options.WindowProcessName);

			foreach (var process in processes)
			{
				if (string.IsNullOrWhiteSpace(process.MainWindowTitle))
				{
					continue;
				}

				await ManipulateWindowAsync(process.MainWindowHandle, stoppingToken);
				_logger.LogInformation("Toggled state of window '{Name}'.", process.MainWindowTitle);
			}

			await Task.Delay(_options.WindowInterval, stoppingToken);
		}
	}

	[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
	private async ValueTask ManipulateWindowAsync(nint handleId, CancellationToken cancellationToken)
	{
		var handle = new HWND(handleId);

		PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_SHOWMINIMIZED);
		await Task.Delay(_options.WindowStateDelay, cancellationToken);
		PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED);
	}
}
