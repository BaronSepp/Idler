namespace Dev.Sepp.Idler.Console.Options;

public sealed class KeyboardOptions
{
	public const string Section = "Keyboard";

	public int Interval { get; set; } = 10_000;
	public ushort Key { get; set; } = 125;
}
