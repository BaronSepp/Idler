using Idler.Options;
using Idler.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Idler;

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
				log.ClearProviders();
				log.AddSimpleConsole(c =>
				{
					c.TimestampFormat = "HH:mm:ss ";
					c.SingleLine = true;
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
