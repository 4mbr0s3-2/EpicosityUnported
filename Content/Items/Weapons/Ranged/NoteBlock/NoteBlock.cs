using Epicosity.Projectiles.Weapons.Ranged;
using Epicosity.Systems.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Epicosity.Items.Weapons.Ranged
{
	public class NoteBlock : SpecialItem
	{
		public class SpecialItemUIState : UIState
		{
			public override void Draw(SpriteBatch spriteBatch)
			{
				Vector2 mousePos = Main.MouseScreen + new Vector2(30, 30);
				Terraria.GameInput.PlayerInput.SetZoom_MouseInWorld();
				int distance = (int)GetDistance(Main.LocalPlayer);
				Vector2 stringSize = Main.fontMouseText.MeasureString(distance.ToString());
				ChatManager.DrawColorCodedStringWithShadow
				(
					spriteBatch,
					Main.fontMouseText,
					distance.ToString(),
					mousePos,
					MinecraftNote.colors[distance],
					0,
					stringSize / 2f,
					Vector2.One
				);
			}
		}
		public override void SetDefaults()
		{
			item.ranged = true;
			item.damage = 20;
			item.width = 16;
			item.height = 16;
			item.rare = ItemRarityID.Green;
			item.shoot = ModContent.ProjectileType<MinecraftNote>();
			item.shootSpeed = 8;
			item.useAnimation = 10;
			item.useTime = 10;
			item.useTurn = true;
			item.holdStyle = ItemHoldStyleID.Default;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.autoReuse = true;
		}
		public static float GetDistance(Player player)
		{
			float relativeDistance = (Main.MouseWorld - player.MountedCenter).Length() / Main.screenHeight * 2;
			if (relativeDistance > 1)
			{
				relativeDistance = 1;
			}
			relativeDistance *= 24;
			return relativeDistance;
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int note = (int)GetDistance(player);
			Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, note);
			return false;
		}
	}
}
