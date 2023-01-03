using Idler.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Idler.Services;

public sealed class KeyboardService : BackgroundService
{
	private readonly ILogger<KeyboardService> _logger;
	private readonly ApplicationOptions _options;

	public KeyboardService(ILogger<KeyboardService> logger, IOptions<ApplicationOptions> options)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_options = options.Value ?? throw new ArgumentNullException(nameof(options));
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (_options.KeyboardInterval <= 0)
		{
			return;
		}

		var input = new INPUT
		{
			type = INPUT_TYPE.INPUT_KEYBOARD
		};
		input.Anonymous.ki.wScan = _options.KeyboardKey;
		input.Anonymous.ki.dwFlags = KEYBD_EVENT_FLAGS.KEYEVENTF_SCANCODE;

		while (stoppingToken.IsCancellationRequested is false)
		{
			ManipulateKeyboard(input);
			_logger.LogInformation("Sent input of key 0x{Key} to system.", _options.KeyboardKey.ToString("X2"));
			await Task.Delay(_options.KeyboardInterval, stoppingToken);
		}
	}

	[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
	private static unsafe uint ManipulateKeyboard(INPUT input)
	{
		var inputs = stackalloc INPUT[1] { input };
		return PInvoke.SendInput(1, inputs, Marshal.SizeOf<INPUT>());
	}
}
