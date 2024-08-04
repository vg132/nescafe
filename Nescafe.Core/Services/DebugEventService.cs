namespace Nescafe.Services;

public static class DebugEventService
{
	public static void Warning(string message)
	{
		System.Diagnostics.Debug.WriteLine(message);
	}

	public static void FrameFinished(long frame)
	{
	}

	public static void FrameStart(long frame)
	{
	}
}
