﻿using System.Diagnostics;

namespace Nescafe.Core
{
	public class PreciseSleep
	{
		private static int estimate = 5;
		private static int mean = 5;
		private static int m2 = 0;
		private static int count = 1;

		public static void Sleep(int milliseconds)
		{
			try
			{
				var stopwatch = new Stopwatch();

				while (milliseconds > estimate)
				{
					stopwatch.Restart();
					Thread.Sleep(1);
					stopwatch.Stop();

					var observed = stopwatch.Elapsed.Milliseconds;
					milliseconds -= observed;

					count++;
					var delta = observed - mean;
					mean += delta / count;
					m2 += delta * (observed - mean);
					var stddev = (int)Math.Sqrt(m2 / (count - 1));
					estimate = mean + stddev;
				}

				// Spin lock
				stopwatch.Restart();
				while (stopwatch.Elapsed.Milliseconds < milliseconds)
				{
					// Just spinning
				}
			}
			catch { }
		}
	}
}