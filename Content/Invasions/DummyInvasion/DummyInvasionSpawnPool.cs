using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Epicosity.NPCs.DummyInvasion;

namespace Epicosity.Invasions
{
	public class DummyInvasionSpawnPool : GlobalNPC
	{	
		public override void EditSpawnPool(IDictionary< int, float > pool, NPCSpawnInfo spawnInfo)
        {
			if(DummyInvasionWorld.dummyInvasionUp && (Main.invasionX == (double)Main.spawnTileX))
			{
				//Clear pool so that only the stuff you want spawns
				pool.Clear();
				bool hasMediumCorePlayerOrHigher = false;
				foreach (Player player in Main.player)
				{
					if (player.difficulty >= 1)
					{
						hasMediumCorePlayerOrHigher = true;
					}
				}
				if (!spawnInfo.sky)
				{
					pool.Add(ModContent.NPCType<DummySoldier>(), 0.01f);
					pool.Add(ModContent.NPCType<RogueDummy>(), 0.2f);
					if (hasMediumCorePlayerOrHigher)
					{
						pool.Add(ModContent.NPCType<FrenchDummy>(), 0.01f);
					}
					pool.Add(ModContent.NPCType<PyroDummy>(), 0.01f);
					pool.Add(ModContent.NPCType<PilotDummy>(), 0.005f);
					if (Epicosity.loadedCompatibilityMods.HasFlag(Epicosity.CompatibilityMods.ThoriumMod))
					{
						pool.Add(ModContent.NPCType<DJDummy>(), 0.01f);
					}
				}
				else
				{
					pool.Add(ModContent.NPCType<PilotDummy>(), 0.02f);
				}
			}
		}
		
		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			//Change spawn stuff if invasion up and invasion at spawn
			if(DummyInvasionWorld.dummyInvasionUp && (Main.invasionX == (double)Main.spawnTileX))
			{
				spawnRate = 33;
				maxSpawns = 20;
			}
		}
		
		public override void PostAI(NPC npc)
		{
			//Changes NPCs so they do not despawn when invasion up and invasion at spawn
			if(DummyInvasionWorld.dummyInvasionUp && (Main.invasionX == (double)Main.spawnTileX))
			{
				npc.timeLeft = 1000;
			}
		}
		
		public override void NPCLoot(NPC npc)
		{
			//When an NPC (from the invasion list) dies, add progress by decreasing size
			if(DummyInvasionWorld.dummyInvasionUp)
			{
				int[] FullList = DummyInvasion.GetFullInvaderList();
				foreach(int invader in FullList)
				{
					if(npc.type == invader)
					{
						Main.invasionSize -= 1;
					}
				}
			}
		}
	}
}