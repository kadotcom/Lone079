using Exiled.Events.Handlers;
using HarmonyLib;

namespace Lone079
{
	[HarmonyPatch(typeof(Scp079), nameof(Scp079.Recontained))]
	class OverchargePatch1
	{
		public static bool Prefix() => false;
	}

	[HarmonyPatch(typeof(Scp079), nameof(Scp079.Recontained))]
	class OverchargePatch2
	{
		public static bool Prefix() => false;
	}
}
