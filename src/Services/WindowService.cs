using Dev.Sepp.Idler.Console.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Dev.Sepp.Idler.Console.Services;

public sealed class WindowService : BackgroundService
{
	private readonly ILogger<WindowService> _logger;
	private readonly WindowOptions _windowOptions;
	private readonly ApplicationOptions _applicationOptions;

	public WindowService(ILogger<WindowService> logger, IOptions<WindowOptions> windowOptions, IOptions<ApplicationOptions> applicationOptions)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_windowOptions = windowOptions.Value ?? throw new ArgumentNullException(nameof(windowOptions));
		_applicationOptions = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (_windowOptions.Interval <= 0)
		{
			return;
		}

		while (stoppingToken.IsCancellationRequested is false)
		{
			await Task.Delay(_windowOptions.Interval, stoppingToken);

			if (_applicationOptions.IsItTimeToIdle() is false)
			{
				continue;
			}

			var processes = Process.GetProcessesByName(_windowOptions.ProcessName);
			foreach (var process in processes)
			{
				if (string.IsNullOrWhiteSpace(process.MainWindowTitle))
				{
					continue;
				}

				await ManipulateWindowAsync(process.MainWindowHandle, stoppingToken);
				_logger.LogInformation("Modified state of window '{Name}'.", process.MainWindowTitle);
			}
		}
	}

	[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
	private async ValueTask ManipulateWindowAsync(nint handleId, CancellationToken cancellationToken)
	{
		var handle = new HWND(handleId);

		PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_SHOWMINIMIZED);
		await Task.Delay(_windowOptions.StateDelay, cancellationToken);
		PInvoke.ShowWindow(handle, SHOW_WINDOW_CMD.SW_RESTORE);
	}
}
