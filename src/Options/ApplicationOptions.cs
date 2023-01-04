namespace Dev.Sepp.Idler.Console.Options;

public sealed class ApplicationOptions
{
	public const string Section = "Application";

	public int KeyboardInterval { get; set; }
	public ushort KeyboardKey { get; set; }

	public int MouseInterval { get; set; }
	public int MouseHorizontalShift { get; set; }
	public int MouseVerticalShift { get; set; }

	public int WindowInterval { get; set; }
	public int WindowStateDelay { get; set; }
	public string WindowProcessName { get; set; } = string.Empty;
}
