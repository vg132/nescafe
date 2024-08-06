using System.Runtime.CompilerServices;

namespace Nescafe.Core;

public static class Utility
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe byte AsByte(this bool to) => *((byte*)&to);
}