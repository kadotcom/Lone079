using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs;
using Exiled.Events.EventArgs.Cassie;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lone079
{
	class EventHandlers
	{
		private System.Random rand = new System.Random();

		private Vector3 scp939pos;

		private bool is106Contained, canChange;

		private List<RoleTypeId> scp079Respawns = new List<RoleTypeId>()
		{
			RoleTypeId.Scp049,
			RoleTypeId.Scp096,
			RoleTypeId.Scp106,
			RoleTypeId.Scp939
		};

		private List<RoomType> scp079RespawnLocations = new List<RoomType>()
		{
			RoomType.Hcz049,
			RoomType.Hcz096,
			RoomType.Hcz939
		};

		private IEnumerator<float> Check079(float delay = 1f)
		{
			if (Generator.Get(Exiled.API.Enums.GeneratorState.Engaged).Count() != 3 && canChange)
			{
				yield return Timing.WaitForSeconds(delay);
				IEnumerable<Player> enumerable = Player.List.Where(x => x.Role.Team == Team.SCPs);
				if (!Lone079.instance.Config.CountZombies) enumerable = enumerable.Where(x => x.Role != RoleTypeId.Scp0492);
				List<Player> pList = enumerable.ToList();
				if (pList.Count == 1 && pList[0].Role == RoleTypeId.Scp079)
				{
					Player player = pList[0];
					Scp079Role scp079 = (Scp079Role)player.Role;
					int level = scp079.Level;
					RoleTypeId role = scp079Respawns[rand.Next(scp079Respawns.Count)];
					if (is106Contained && role == RoleTypeId.Scp106) role = RoleTypeId.Scp939;
					player.Role.Set(role);
					Timing.CallDelayed(1f, () => player.Position = scp939pos);
					player.Health = !Lone079.instance.Config.ScaleWithLevel ? player.MaxHealth * (Lone079.instance.Config.HealthPercent / 100f) : player.MaxHealth * ((Lone079.instance.Config.HealthPercent + ((level - 1) * 5)) / 100f);
					player.Broadcast(10, "<i>You have been respawned as a random SCP with half health because all other SCPs have died.</i>");
				}
			}
		}

		// no work
		public void OnPlayerLeave(LeftEventArgs ev)
		{
			if (ev.Player.Role.Team == Team.SCPs) Timing.RunCoroutine(Check079(3f));
		}

		public void OnDetonated() => canChange = false;

		public void OnRoundStart()
		{			
			Timing.CallDelayed(1f, () => scp939pos = Room.Get(scp079RespawnLocations[rand.Next(scp079RespawnLocations.Count)]).transform.position);
			is106Contained = false;
			canChange = true;
		}

		public void OnPlayerDied(DiedEventArgs ev)
		{
			//if (ev.Player.Role.Team == Team.SCPs) Timing.RunCoroutine(Check079(3f));
			Timing.RunCoroutine(Check079(3f));
		}

		public void OnScp106Contain(DyingEventArgs ev)
		{
			if (ev.Player == null) return;
			if(ev.Player.Role.Type == RoleTypeId.Scp106)
			{
                is106Contained = true;
            }
        }

		public void OnCassie(SendingCassieMessageEventArgs ev)
		{
			if (ev.Words.Contains("allgeneratorsengaged")) ev.IsAllowed = false;
		}
	}
}
