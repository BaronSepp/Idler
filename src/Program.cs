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
					{ "-st", "Application:StartTime" },
					{ "--StartTime", "Application:StartTime" },
					{ "-et", "Application:EndTime" },
					{ "--EndTime", "Application:EndTime" },

					{ "-ki", "Keyboard:Interval" },
					{ "--KeyboardInterval", "Keyboard:Interval" },
					{ "-kk", "Keyboard:Key" },
					{ "--KeyboardKey", "Keyboard:Key" },

					{ "-mi", "Mouse:Interval" },
					{ "--MouseInterval", "Mouse:Interval" },
					{ "-mh", "Mouse:HorizontalShift" },
					{ "--MouseHorizontalShift", "Mouse:HorizontalShift" },
					{ "-mv", "Mouse:VerticalShift" },
					{ "--MouseVerticalShift", "Mouse:VerticalShift" },

					{ "-wi", "Window:Interval" },
					{ "--WindowInterval", "Window:Interval" },
					{ "-ws", "Window:StateDelay" },
					{ "--WindowStateDelay", "Window:StateDelay" },
					{ "-wn", "Window:ProcessName" },
					{ "--WindowProcessName", "Window:ProcessName" }
				});
				app.AddJsonFile("appsettings.json");
			})
			.ConfigureLogging(log =>
			{
				log.AddConsole(options => options.FormatterName = nameof(CustomConsoleFormatter));
				log.AddConsoleFormatter<CustomConsoleFormatter, LoggerOptions>(options =>
				{
					options.TimestampFormat = "HH:mm:ss";
					options.UseColor = true;
				});
			})
			.ConfigureServices((host, services) =>
			{
				services.AddOptions();
				services.Configure<ApplicationOptions>(host.Configuration.GetRequiredSection(ApplicationOptions.Section));
				services.Configure<KeyboardOptions>(host.Configuration.GetRequiredSection(KeyboardOptions.Section));
				services.Configure<MouseOptions>(host.Configuration.GetRequiredSection(MouseOptions.Section));
				services.Configure<WindowOptions>(host.Configuration.GetRequiredSection(WindowOptions.Section));

				services.AddHostedService<KeyboardService>();
				services.AddHostedService<MouseService>();
				services.AddHostedService<WindowService>();
			})
			.UseConsoleLifetime();
	}
}
