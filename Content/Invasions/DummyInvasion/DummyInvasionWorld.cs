using System;
using System.IO;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using Terraria.ModLoader;

namespace Epicosity.Invasions
{
    public class DummyInvasionWorld : ModWorld
    {
		//Setting up variables for invasion
		public static bool dummyInvasionUp = false;
		public static bool downedDummyInvasion = false;

		//Initialize all variables to their default values
		public override void Initialize()
		{
			Main.invasionSize = 0;
			Main.invasionType = 0;
			NetMessage.SendData(MessageID.WorldData);
			dummyInvasionUp = false;
			downedDummyInvasion = false;
		}

		//Save downed data
		public override TagCompound Save()
		{
			var downed = new List<string>();
			if (downedDummyInvasion) downed.Add("dummyInvasion");

			return new TagCompound {
				{"downed", downed}
			};
		}

		//Load downed data
		public override void Load(TagCompound tag)
		{
			var downed = tag.GetList<string>("downed");
			downedDummyInvasion = downed.Contains("dummyInvasion");
		}

		//Sync downed data
		public override void NetSend(BinaryWriter writer)
		{
			BitsByte flags = new BitsByte();
			flags[0] = downedDummyInvasion;
			writer.Write(flags);
			writer.Write(dummyInvasionUp);
			writer.Write(Main.invasionX);
		}

		//Sync downed data
		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			downedDummyInvasion = flags[0];
			dummyInvasionUp = reader.ReadBoolean();
			Main.invasionX = reader.ReadDouble();
		}
		
		//Allow to update invasion while game is running
		public override void PostUpdate()
		{
			if(dummyInvasionUp)
			{
				if(Main.invasionX == (double)Main.spawnTileX)
				{
					//Checks progress and reports progress only if invasion at spawn
					DummyInvasion.CheckDummyInvasionProgress();
				}
				//Updates the dungeon invasion while it heads to spawn point and ends it
				DummyInvasion.UpdateDummyInvasion();
			}
		}
	}
}

