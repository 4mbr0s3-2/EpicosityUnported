using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Epicosity.Projectiles.Throwables
{
	public class SolitaireCard : ModProjectile
	{
		int time = 0;
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailingMode[projectile.type] = 2; // Does rotation as well
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
		}
		public override void SetDefaults()
		{
			projectile.alpha = 255;
			projectile.scale = 1f;
			projectile.thrown = true;
			projectile.friendly = true;
			projectile.aiStyle = -1;
			projectile.width = 142;
			projectile.height = 192;
			projectile.tileCollide = false;
			projectile.frame = Main.rand.Next(52);
			projectile.penetrate = -1;
			time = 300;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 32;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = GetTexture(Texture);
			for (int oldPosIndex = 0; oldPosIndex < projectile.oldPos.Length; oldPosIndex++)
			{
				spriteBatch.Draw
				(
					texture,
					new Vector2
					(
						projectile.oldPos[projectile.oldPos.Length - oldPosIndex - 1].X - Main.screenPosition.X + texture.Width / 2,
						projectile.oldPos[projectile.oldPos.Length - oldPosIndex - 1].Y - Main.screenPosition.Y + projectile.height - texture.Height * 0.5f + 2f
					),
					new Rectangle((int)(texture.Width / 52f * projectile.frame), 0, texture.Width / 52, texture.Height),
					Color.White * (1 - projectile.alpha / 255.0f),
					projectile.oldRot[oldPosIndex],
					texture.Size() * 0.5f,
					projectile.scale,
					SpriteEffects.None,
					0f
				);
			}
			spriteBatch.Draw
			(
				texture,
				new Vector2
				(
					projectile.position.X - Main.screenPosition.X + texture.Width / 2,
					projectile.position.Y - Main.screenPosition.Y + projectile.height - texture.Height * 0.5f + 2f
				),
				new Rectangle((int)(texture.Width / 52f * projectile.frame), 0, texture.Width / 52, texture.Height),
				Color.White * (1 - projectile.alpha / 255.0f),
				projectile.rotation,
				texture.Size() * 0.5f,
				projectile.scale,
				SpriteEffects.None,
				0f
			);
			return false;
		}
		public override void AI()
		{
			projectile.velocity.Y += 0.3f;
			time--;
			if (time <= 0)
			{
				projectile.Kill();
			}
			if (time >= 200)
			{
				Vector2 oppositeVector = Collision.TileCollision(projectile.position, projectile.velocity, projectile.width, projectile.height, false);
				Vector2 differenceVector = oppositeVector - projectile.velocity;
				if (differenceVector != Vector2.Zero) differenceVector.Normalize(); // Prevents NaNs
				projectile.velocity += differenceVector * 10 + oppositeVector - projectile.velocity;
			}
			if (projectile.alpha > 0)
			{
				projectile.alpha -= 3;
				if (projectile.alpha < 0)
				{
					projectile.alpha = 0;
				}
			}
		}

	}
}
