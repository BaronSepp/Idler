using Dev.Sepp.Idler.Console.Logging;
using Dev.Sepp.Idler.Console.Options;
using Dev.Sepp.Idler.Console.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dev.Sepp.Idler.Console;

public static class Program
{
	[STAThread]
	public static int Main(string[] args)
	{
		using var host = CreateHostBuilder(args).Build();
		host.Run();

		return Environment.ExitCode;
	}

	private static IHostBuilder CreateHostBuilder(string[] args)
	{
		return new HostBuilder()
			.ConfigureAppConfiguration(app =>
			{
				app.AddCommandLine(args, new Dictionary<string, string>
				{
					{ "-ki", "Application:KeyboardInterval" },
					{ "--KeyboardInterval", "Application:KeyboardInterval" },
					{ "-kk", "Application:KeyboardKey" },
					{ "--KeyboardKey", "Application:KeyboardKey" },

					{ "-mi", "Application:MouseInterval" },
					{ "--MouseInterval", "Application:MouseInterval" },
					{ "-mh", "Application:MouseHorizontalShift" },
					{ "--MouseHorizontalShift", "Application:MouseHorizontalShift" },
					{ "-mv", "Application:MouseVerticalShift" },
					{ "--MouseVerticalShift", "Application:MouseVerticalShift" },

					{ "-wi", "Application:WindowInterval" },
					{ "--WindowInterval", "Application:WindowInterval" },
					{ "-ws", "Application:WindowStateDelay" },
					{ "--WindowStateDelay", "Application:WindowStateDelay" },
					{ "-wn", "Application:WindowProcessName" },
					{ "--WindowProcessName", "Application:WindowProcessName" }
				});
				app.AddJsonFile("appsettings.json");
			})
			.ConfigureLogging(log =>
			{
				log.AddConsole(options => options.FormatterName = nameof(CustomConsoleFormatter));
				log.AddConsoleFormatter<CustomConsoleFormatter, LoggerOptions>(options =>
				{
					options.TimestampFormat = "HH:mm:ss.ff";
					options.IncludeScopes = false;
					options.UseUtcTimestamp = false;
					options.UseColor = false;
				});
			})
			.ConfigureServices((host, services) =>
			{
				services.AddOptions();
				services.Configure<ApplicationOptions>(host.Configuration.GetRequiredSection(ApplicationOptions.Section));

				services.AddHostedService<KeyboardService>();
				services.AddHostedService<MouseService>();
				services.AddHostedService<WindowService>();
			})
			.UseConsoleLifetime();
	}
}
