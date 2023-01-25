namespace Dev.Sepp.Idler.Console.Options;

public sealed class WindowOptions
{
	public const string Section = "Window";

	public int Interval { get; set; } = -1;
	public int StateDelay { get; set; } = 1000;
	public string ProcessName { get; set; } = string.Empty;
}
