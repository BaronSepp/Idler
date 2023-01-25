namespace Dev.Sepp.Idler.Console.Options;

public sealed class ApplicationOptions
{
	public const string Section = "Application";

	public TimeOnly StartTime { get; set; }

	public TimeOnly EndTime { get; set; }



	public bool IsItTimeToIdle()
	{
		var currentTime = TimeOnly.FromDateTime(DateTime.Now);
		return currentTime >= StartTime && currentTime <= EndTime;
	}
}
