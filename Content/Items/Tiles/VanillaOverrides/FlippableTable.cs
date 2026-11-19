using Epicosity.Network;
using Epicosity.Utilities;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Epicosity.Tiles
{
	public class FlippableTable : GlobalTile
	{
		public override void MouseOver(int i, int j, int type)
		{
			Tile tile = Main.tile[i, j];
			if (tile.type == 14 && tile.frameNumber() == 0)
			{
				Player player = Main.LocalPlayer;
				player.noThrow = 2;
				player.showItemIcon = true;
				player.showItemIcon2 = -1;
				player.showItemIconText = player.direction == 1 ? "(╯°□°)╯" : "╰(°□°╰)";
			};
		}
		public override void RightClick(int i, int j, int type)
		{
			Tile tile = Main.tile[i, j];
			if (tile.type == 14 && tile.frameNumber() == 0)
			{
				FlipTable(Main.LocalPlayer.whoAmI, i, j);
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					ModNetHandler.GetPacketHandler<TableFlipPacketHandler>().SendTarget(-1, Main.myPlayer, i, j);
				}
				// Projectiles already sync across server
				Projectile.NewProjectile(new Microsoft.Xna.Framework.Vector2(i, j).ToWorldCoordinates(),
					Main.LocalPlayer.velocity + new Vector2(Main.LocalPlayer.direction * 2, -2f),
					ModContent.ProjectileType<Projectiles.Weapons.Melee.WoodenTable>(),
					50,
					2,
					Main.LocalPlayer.whoAmI);
			}
		}
		public static void FlipTable(int owner, int i, int j)
		{
			TileHelper.DestroyFrameImportantTilesWithoutDrops(owner, i, j);
		}
		internal class TableFlipPacketHandler : PacketHandler
		{
			public const byte SyncTarget = 1;

			public TableFlipPacketHandler(byte handlerType) : base(handlerType)
			{
			}

			public override void HandlePacket(BinaryReader reader, int fromWho)
			{
				switch (reader.ReadByte())
				{
					case (SyncTarget):
						ReceiveTarget(reader, fromWho);
						break;
				}
			}

			public void SendTarget(int toWho, int fromWho, int i, int j)
			{
				ModPacket packet = GetPacket(SyncTarget, fromWho);
				packet.Write(i);
				packet.Write(j);
				packet.Send(toWho, fromWho);
			}

			public void ReceiveTarget(BinaryReader reader, int fromWho)
			{
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					fromWho = reader.ReadByte(); // fromWho
				}
				int i = reader.ReadInt32();
				int j = reader.ReadInt32();
				if (Main.netMode == NetmodeID.Server)
				{
					SendTarget(-1, fromWho, i, j);
				}
				else
				{
					FlipTable(fromWho, i, j);
				}
			}
		}

	}
}
