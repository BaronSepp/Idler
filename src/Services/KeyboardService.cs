using Dev.Sepp.Idler.Console.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Dev.Sepp.Idler.Console.Services;

public sealed class KeyboardService : BackgroundService
{
	private readonly ILogger<KeyboardService> _logger;
	private readonly ApplicationOptions _applicationOptions;
	private readonly KeyboardOptions _keyboardOptions;

	public KeyboardService(ILogger<KeyboardService> logger, IOptions<KeyboardOptions> keyboardOptions, IOptions<ApplicationOptions> applicationOptions)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_applicationOptions = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
		_keyboardOptions = keyboardOptions.Value ?? throw new ArgumentNullException(nameof(keyboardOptions));
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (_keyboardOptions.Interval <= 0)
		{
			return;
		}

		var input = new INPUT
		{
			type = INPUT_TYPE.INPUT_KEYBOARD
		};
		input.Anonymous.ki.wScan = _keyboardOptions.Key;
		input.Anonymous.ki.dwFlags = KEYBD_EVENT_FLAGS.KEYEVENTF_SCANCODE;

		while (stoppingToken.IsCancellationRequested is false)
		{
			await Task.Delay(_keyboardOptions.Interval, stoppingToken);

			if (_applicationOptions.IsItTimeToIdle() is false)
			{
				continue;
			}

			ManipulateKeyboard(input);
			_logger.LogInformation("Sent key 0x{Key} input to system.", _keyboardOptions.Key.ToString("X2"));
		}
	}

	[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
	private static unsafe uint ManipulateKeyboard(INPUT input)
	{
		var inputs = stackalloc INPUT[1] { input };
		return PInvoke.SendInput(1, inputs, Marshal.SizeOf<INPUT>());
	}
}
