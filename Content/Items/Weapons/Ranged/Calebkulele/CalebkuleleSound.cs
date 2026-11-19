using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Epicosity.Projectiles.Weapons.Ranged
{
	public abstract class CalebkuleleSound : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			projectile.extraUpdates = 2;
			projectile.width = 14;
			projectile.height = 14;
			projectile.penetrate = -1;
			projectile.aiStyle = 1;
			projectile.alpha = 20;
			projectile.timeLeft = 200;
			projectile.friendly = true;
			aiType = ProjectileID.Bullet;
		}
		//public override void AI()
		//{
		//	Lighting.AddLight(base.projectile.position, 0.05f, 0.3f, 0.45f);
		//}
		public sealed override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Vector2 origin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, Main.projectileTexture[projectile.type].Height * 0.5f);
			// Trail
			for (int oldPosIndex = projectile.oldPos.Length - 1; oldPosIndex > 0; oldPosIndex--)
			{
				Vector2 drawPosition = projectile.oldPos[oldPosIndex] - Main.screenPosition + origin;
				Color color =
					new Color(255, 255, 255, 127)
					* 0.5f 
					* (projectile.timeLeft / 200f) 
					* ((255 - projectile.alpha) / 255f) 
					* ((projectile.oldPos.Length - oldPosIndex) / (float)projectile.oldPos.Length);
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPosition, null, color, projectile.oldRot[oldPosIndex], origin, projectile.scale, SpriteEffects.None, 0f);
			}
			spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.position - Main.screenPosition + origin, null, new Color(255, 255, 255, 255) * (projectile.timeLeft / 200f), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
		public sealed override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = -oldVelocity.X;
			}
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.velocity.Y = -oldVelocity.Y;
			}
			return false;
		}
	}
	public class CalebkuleleSmall : CalebkuleleSound 
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Calebic 'Now' Wave");
			base.SetStaticDefaults();
		}
	}
	public class CalebkuleleBig : CalebkuleleSound
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Calebic 'Yes' Wave");
			base.SetStaticDefaults();
		}
		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.width = 24;
			projectile.height = 24;
		}
	}
}
