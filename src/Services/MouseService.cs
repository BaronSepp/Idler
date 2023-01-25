using Dev.Sepp.Idler.Console.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Dev.Sepp.Idler.Console.Services;

public sealed class MouseService : BackgroundService
{
	private readonly ILogger<MouseService> _logger;
	private readonly ApplicationOptions _applicationOptions;
	private readonly MouseOptions _mouseOptions;

	public MouseService(ILogger<MouseService> logger, IOptions<MouseOptions> mouseOptions, IOptions<ApplicationOptions> applicationOptions)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_applicationOptions = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
		_mouseOptions = mouseOptions.Value ?? throw new ArgumentNullException(nameof(mouseOptions));
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (_mouseOptions.Interval <= 0)
		{
			return;
		}

		var shiftLeft = new INPUT
		{
			type = 0
		};
		shiftLeft.Anonymous.mi.time = 0;
		shiftLeft.Anonymous.mi.dx = _mouseOptions.HorizontalShift * -1;
		shiftLeft.Anonymous.mi.dy = _mouseOptions.VerticalShift;
		shiftLeft.Anonymous.mi.dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE;

		var shiftRight = new INPUT
		{
			type = 0
		};
		shiftRight.Anonymous.mi.time = 0;
		shiftRight.Anonymous.mi.dx = _mouseOptions.HorizontalShift;
		shiftRight.Anonymous.mi.dy = _mouseOptions.VerticalShift * -1;
		shiftRight.Anonymous.mi.dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE;

		while (stoppingToken.IsCancellationRequested is false)
		{
			await Task.Delay(_mouseOptions.Interval, stoppingToken);

			if (_applicationOptions.IsItTimeToIdle() is false)
			{
				continue;
			}

			ManipulateMouse(shiftLeft);
			ManipulateMouse(shiftRight);
			_logger.LogInformation("Shifted mouse by {PixelCountX}x{PixelCountY} pixels.", _mouseOptions.HorizontalShift, _mouseOptions.VerticalShift);
		}
	}

	[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
	private static unsafe uint ManipulateMouse(INPUT input)
	{
		var inputs = stackalloc INPUT[1] { input };
		return PInvoke.SendInput(1, inputs, Marshal.SizeOf<INPUT>());
	}
}
