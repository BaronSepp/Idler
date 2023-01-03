using Idler.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Idler.Services;

public sealed class MouseService : BackgroundService
{
	private readonly ILogger<MouseService> _logger;
	private readonly ApplicationOptions _options;

	public MouseService(ILogger<MouseService> logger, IOptions<ApplicationOptions> options)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_options = options.Value ?? throw new ArgumentNullException(nameof(options));
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (_options.MouseInterval <= 0)
		{
			return;
		}

		var shiftLeft = new INPUT
		{
			type = 0
		};
		shiftLeft.Anonymous.mi.time = 0;
		shiftLeft.Anonymous.mi.dx = _options.MouseHorizontalShift * -1;
		shiftLeft.Anonymous.mi.dy = _options.MouseVerticalShift;
		shiftLeft.Anonymous.mi.dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE;

		var shiftRight = new INPUT
		{
			type = 0
		};
		shiftRight.Anonymous.mi.time = 0;
		shiftRight.Anonymous.mi.dx = _options.MouseHorizontalShift;
		shiftRight.Anonymous.mi.dy = _options.MouseVerticalShift * -1;
		shiftRight.Anonymous.mi.dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE;

		while (stoppingToken.IsCancellationRequested is false)
		{
			ManipulateMouse(shiftLeft);
			ManipulateMouse(shiftRight);
			_logger.LogInformation("Shifted mouse by {PixelCountX}x{PixelCountY} pixels.", _options.MouseHorizontalShift, _options.MouseVerticalShift);
			await Task.Delay(_options.MouseInterval, stoppingToken);
		}
	}

	[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
	private static unsafe uint ManipulateMouse(INPUT input)
	{
		var inputs = stackalloc INPUT[1] { input };
		return PInvoke.SendInput(1, inputs, Marshal.SizeOf<INPUT>());
	}
}
