namespace Dev.Sepp.Idler.Console.Options;

public sealed class MouseOptions
{
	public const string Section = "Mouse";

	public int Interval { get; set; } = 30_000;
	public int HorizontalShift { get; set; } = 1;
	public int VerticalShift { get; set; } = 1;
}
