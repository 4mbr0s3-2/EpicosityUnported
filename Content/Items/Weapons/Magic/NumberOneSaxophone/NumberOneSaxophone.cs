using Epicosity.Items.Rarity;
using Epicosity.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
namespace Epicosity.Items.Weapons.Magic
{
    class NumberOneSaxophone : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Number One Saxophone");
			Tooltip.SetDefault("'From the most villanous of villains'");
		}
		public override void HoldStyle(Player player)
		{
			player.itemLocation += new Vector2(0, 12);
		}
		public override void UseStyle(Player player)
		{
			player.itemLocation += new Vector2(0, 12);
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips[1].text = Regex.Replace(tooltips[1].text, "magic", "villanous magic");
		}
		public override void SetDefaults()
        {
            item.rare = EpicosityItemRarity.PulsatingMagenta;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useAnimation = 5;
			item.useTime = 5;
			item.width = 12;
			item.height = 28;
			item.shoot = ProjectileID.WoodenArrowFriendly;
			item.holdStyle = 3;
			item.autoReuse = true;
			item.damage = 50;
			item.shootSpeed = 6.5f;
			item.noMelee = true;
			item.value = 200000;
			item.mana = 4;
			item.magic = true;
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 rotatedRelativePoint = player.RotatedRelativePoint(player.MountedCenter, true);
			float xToMouse = (float)Main.mouseX + Main.screenPosition.X - rotatedRelativePoint.X;
			float yToMouse = (float)Main.mouseY + Main.screenPosition.Y - rotatedRelativePoint.Y;
			if (player.gravDir == -1f)
			{
				yToMouse = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - rotatedRelativePoint.Y;
			}
			float centerToMouseDist = (float)Math.Sqrt((double)(xToMouse * xToMouse + yToMouse * yToMouse));
			float centerToMouseDist2 = centerToMouseDist;
			int noteType = 76;
			noteType += Main.rand.Next(3);
			centerToMouseDist2 /= (float)(Main.screenHeight / 2);
			if (centerToMouseDist2 > 1f)
			{
				centerToMouseDist2 = 1f;
			}
			float projSpeedX = xToMouse * 0.1f + (float)Main.rand.Next(-40, 41) * 0.01f;
			float projSpeedY = yToMouse * 0.1f + (float)Main.rand.Next(-40, 41) * 0.01f;
			projSpeedX *= centerToMouseDist2 + 0.25f;
			projSpeedY *= centerToMouseDist2 + 0.25f;
			if (new Vector2(projSpeedX, projSpeedY).Length() > 10)
			{
				Vector2 limited = new Vector2(projSpeedX, projSpeedY);
				limited.Normalize();
				limited *= 10;
				projSpeedX = limited.X;
				projSpeedY = limited.Y;
			}
			int num88 = Projectile.NewProjectile(rotatedRelativePoint.X, rotatedRelativePoint.Y, projSpeedX, projSpeedY, noteType, item.damage, item.knockBack, player.whoAmI, 0f, 0f);
			//Main.projectile[num88].ai[1] = 1f; // Activate sound on spawn
			centerToMouseDist2 = centerToMouseDist2 * 2f - 1f; // Calculate pitch
			if (centerToMouseDist2 < -1f)
			{
				centerToMouseDist2 = -1f;
			}
			if (centerToMouseDist2 > 1f)
			{
				centerToMouseDist2 = 1f;
			}
			LegacySoundStyle saxSound = mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Item, "Sounds/Item/Weapons/Magic/NumberOneSaxophoneC");
			Main.PlaySound(saxSound, player.position).Pitch = centerToMouseDist2;
			
			return false;
		}
	}
	internal class NumberOneSaxophoneNotePacketHandler : PacketHandler
	{
		public const byte SyncTarget = 1;

		public NumberOneSaxophoneNotePacketHandler(byte handlerType) : base(handlerType)
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

		public void SendTarget(int toWho, int fromWho, float noteValue)
		{
			ModPacket packet = GetPacket(SyncTarget, fromWho);
			packet.Write(noteValue);
			packet.Send(toWho, fromWho);
		}

		public void ReceiveTarget(BinaryReader reader, int fromWho)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				fromWho = reader.ReadByte(); // fromWho
			}
			float noteValue = reader.ReadSingle();
			if (Main.netMode == NetmodeID.Server)
			{
				SendTarget(-1, fromWho, noteValue);
			}
			else
			{
				LegacySoundStyle saxSound = Epicosity.inst.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Item, "Sounds/Item/Weapons/Magic/NumberOneSaxophoneC");
				Main.PlaySound(saxSound, Main.player[fromWho].position).Pitch = noteValue;
			}
		}
	}
}
