using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Epicosity.NPCs;
using Epicosity.NPCs.DummyInvasion;

namespace Epicosity.Invasions
{
    public class DummyInvasion
    {
		// TODO: Unload static variables
		//List of PreHardmodeInvaders
		public static Dictionary<int, float> PreHardmodeInvaders = new Dictionary<int, float> {
			[ModContent.NPCType<DummySoldier>()] = 0.01f,
			[ModContent.NPCType<RogueDummy>()] = 0.2f,
			[ModContent.NPCType<FrenchDummy>()] = 0.01f,
			[ModContent.NPCType<PyroDummy>()] = 0.01f,
			[ModContent.NPCType<PilotDummy>()] = 0.005f,
		};
		public static Dictionary<int, float> HardmodeInvaders = new Dictionary<int, float>
		{
		};
		//Easy way to get full list
		public static int[] GetFullInvaderList()
		{
			//Creating a list
			List<int> list = new List<int>();
			
			//CopyTo(var arrayToCopyTo, index)
			foreach (KeyValuePair<int, float> i in PreHardmodeInvaders)
			{
				list.Add(i.Key);
			}
			if (Main.hardMode)
			{
				foreach (KeyValuePair<int, float> i in HardmodeInvaders)
				{
					list.Add(i.Key);
				}
			}
			return list.ToArray();
		}

		public static List<int> GetFullInvaderGenericList()
		{
			//Creating a list
			List<int> list = new List<int>();

			//CopyTo(var arrayToCopyTo, index)
			foreach (KeyValuePair<int, float> i in PreHardmodeInvaders)
			{
				list.Add(i.Key);
			}
			if (Main.hardMode)
			{
				foreach (KeyValuePair<int, float> i in HardmodeInvaders)
				{
					list.Add(i.Key);
				}
			}
			return list;
		}

		//Setup for an Invasion After setting up
		public static void Start()
		{
			//Set to no invasion
			if (Main.invasionType != 0 && Main.invasionSize == 0)
			{
				Main.invasionType = 0;
			}
			if (Main.invasionType == 0)
			{
				//Checks amount of players
				int num = 0;
				for (int i = 0; i < 255; i++)
				{
					if (Main.player[i].active && Main.player[i].statLifeMax >= 200)
					{
						num++;
					}
				}
				if (num > 0)
				{
					//Invasion setup
					Main.invasionType = -7; // Negative for modded
					DummyInvasionWorld.dummyInvasionUp = true;
					Main.invasionSize = 100 * num;
					Main.invasionSizeStart = Main.invasionSize;
					Main.invasionProgress = 0;
					Main.invasionProgressIcon = -7;
					Main.invasionProgressWave = 0;
					Main.invasionProgressMax = Main.invasionSizeStart;
					Main.invasionWarn = 10;
					if (Main.rand.Next(2) == 0)
					{
						Main.invasionX = 0.0;
						return;
					}
					Main.invasionX = (double)Main.maxTilesX; // Start from the right
				}
			}
		}

		//Text for Dungeons and syncing messages
		public static void DummyInvasionWarning()
		{
			String text = "";
			if (Main.invasionX == (double)Main.spawnTileX)
			{
				text = "The dummies are taking revenge!";
			}
			if(Main.invasionSize <= 0)
			{
				text = "The dummies has been defeated!";
			}
			if (Main.netMode == NetmodeID.SinglePlayer) // Singleplayer
			{
				Main.NewText(text, 255, 255, 255, false);
				return;
			}
			if (Main.netMode == NetmodeID.Server) // Server
			{
				//Sync with net
				NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.White);
			}
			// Sync start of invasion
			NetMessage.SendData(MessageID.WorldData);
		}
		
		public static void UpdateDummyInvasion()
		{
			if(DummyInvasionWorld.dummyInvasionUp)
			{
				//End invasion if size less or equal to 0
				if(Main.invasionSize <= 0)
				{
					DummyInvasionWorld.dummyInvasionUp = false;
					DummyInvasionWarning();
					Main.invasionType = 0;
					Main.invasionDelay = 0;
					DummyInvasionWorld.downedDummyInvasion = true;
					NetMessage.SendData(MessageID.WorldData);
				}
				
				//Do not do the rest if invasion already at spawn
				if (Main.invasionX == (double)Main.spawnTileX)
				{
					return;
				}
				
				//Update when the invasion gets to Spawn
				float num = (float)Main.dayRate;
				if (Main.invasionX > (double)Main.spawnTileX)
				{
					Main.invasionX -= (double)num;
					if (Main.invasionX <= (double)Main.spawnTileX)
					{
						Main.invasionX = (double)Main.spawnTileX;
						DummyInvasionWarning();
					}
					else
					{
						Main.invasionWarn--;
					}
				}
				else
				{
					if (Main.invasionX < (double)Main.spawnTileX)
					{
						Main.invasionX += (double)num;
						if (Main.invasionX >= (double)Main.spawnTileX)
						{
							Main.invasionX = (double)Main.spawnTileX;
							DummyInvasionWarning();
						}
						else
						{
							Main.invasionWarn--;
						}
					}
				}
			}
		}
		
		public static void CheckDummyInvasionProgress()
		{
			//Get full list of invaders
			int[] FullList = GetFullInvaderList();
			
			//Not really sure what this is
			if (Main.invasionProgressMode != 2)
			{
				Main.invasionProgressNearInvasion = false;
				return;
			}
			bool invadersOnScreenFlag = false;
			Player player = Main.player[Main.myPlayer];
			Rectangle rectangle = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
			int num = 5000;
			int icon = 0;
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].active)
				{
					icon = 0;
					int type = Main.npc[i].type;
					for(int n = 0; n < FullList.Length; n++)
					{
						if(type == FullList[n])
						{
							Rectangle value = new Rectangle((int)(Main.npc[i].position.X + (float)(Main.npc[i].width / 2)) - num, (int)(Main.npc[i].position.Y + (float)(Main.npc[i].height / 2)) - num, num * 2, num * 2);
							if (rectangle.Intersects(value))
							{
								invadersOnScreenFlag = true; // Check if any invaders are on screen
								break;
							}
						}
					}
				}
			}
			Main.invasionProgressNearInvasion = invadersOnScreenFlag;
			int progressMax3 = 1;
			if (DummyInvasionWorld.dummyInvasionUp)
			{
				progressMax3 = Main.invasionSizeStart;
			}
			if (DummyInvasionWorld.dummyInvasionUp && (Main.invasionX == (double)Main.spawnTileX))
			{
				//Shows the UI for the invasion
				Main.ReportInvasionProgress(Main.invasionSizeStart - Main.invasionSize, progressMax3, Main.invasionProgressIcon, 0);
			}
			
			//Syncing start of invasion
			foreach (Player p in Main.player)
			{
				NetMessage.SendData(MessageID.InvasionProgressReport, p.whoAmI, -1, null, Main.invasionSizeStart - Main.invasionSize, (float)Main.invasionSizeStart, (float)(Main.invasionType + 3), 0f, 0, 0, 0);
			}
		}
    }
}
