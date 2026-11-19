using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using System;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;
using ReLogic.Graphics;
using System.IO;

namespace Epicosity.Projectiles.Weapons.Ranged
{
	public class DiscordMessage : ModProjectile
	{
		public int notificationsLeft = 3;
		public int originalDamage = -1;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Discord Message");
		}

		public override void SetDefaults()
		{
			//projectile.arrow = true;
			projectile.width = 16;
			projectile.height = 16;
			projectile.aiStyle = 0;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.light = 0.2f;
			projectile.penetrate = 3;
		}
		public override void AI()
		{
			Lighting.AddLight(projectile.position, 0.9f, 0.1f, 0.1f);
			int dust = Dust.NewDust(projectile.position, 16, 16, DustID.PortalBolt, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, Color.Red, 0.7f);
			Main.dust[dust].noGravity = true;
			//Dust.NewDust(projectile.position, 10, 10, DustID.AmberBolt, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, Microsoft.Xna.Framework.Color.Red, 0.7f);
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			for (int x = 0; x < 5; x++) Dust.NewDust(projectile.position, 10, 10, DustID.PortalBoltTrail, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, Color.Red, 0.7f);
			return base.OnTileCollide(oldVelocity);
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.Aqua;
		}
		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(notificationsLeft);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			notificationsLeft = reader.ReadInt32();
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			for (int x = 0; x < 5; x++) Dust.NewDust(projectile.position, 10, 10, DustID.PortalBoltTrail, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, Color.Red, 0.7f);
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/DiscordMessage"));
			notificationsLeft--;
			projectile.netUpdate = true;
			//base.OnHitNPC(target, damage, knockback, crit);
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = mod.GetTexture("Projectiles/Weapons/Ranged/DiscordMessage");
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			spriteBatch.Draw
			(
				texture,
				new Vector2
				(
					projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f,
					projectile.position.Y - Main.screenPosition.Y + projectile.height - texture.Height * 0.5f + 2f
				),
				null,
				new Color(255, 255, 255, 255 - projectile.alpha),
				projectile.rotation,
				texture.Size() * 0.5f,
				projectile.scale,
				SpriteEffects.None,
				0f
			);
			spriteBatch.DrawString(
				mod.GetFont("Fonts/Whitney Medium"),
				notificationsLeft.ToString(),
				new Vector2
				(
					projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f + 3,
					projectile.position.Y - Main.screenPosition.Y + projectile.height - texture.Height * 0.5f + 1f
				),
				Color.White,
				0,
				texture.Size() * 0.5f,
				1f,
				SpriteEffects.None,
				0
			);
			//Main.GameViewMatrix.Effects = SpriteEffects.FlipHorizontally;
			return false;
		}

		// Additional hooks/methods here.
	}
}