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

namespace Epicosity.Projectiles.Weapons.Ranged
{
	public class BruhFastSFX : BruhSFX
	{
		public override Vector3 GetColor()
		{
			return Color.DarkRed.ToVector3() * 0.2f;
		}
	}
	public class BruhSFX : ModProjectile
	{
		public int originalDamage = -1;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bruh SFX #2 Soundwave");
		}

		public override void SetDefaults()
		{
			projectile.width = 15;
			projectile.height = 12;
			projectile.aiStyle = 0;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.tileCollide = false;
			projectile.light = 0.0f;
			projectile.penetrate = 5;
			originalDamage = -1;
		}
		public override void AI()
		{
			Lighting.AddLight(projectile.position, GetColor());
			if (originalDamage == -1)
			{
				originalDamage = projectile.damage;
			}
			projectile.damage = (int)((255 - projectile.alpha) / 255.0f * originalDamage);
			projectile.ai[1] += 1;
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
			if (projectile.ai[1] < 60)
			{
				projectile.scale = (projectile.scale * 10 + 5) / 11;
				projectile.alpha += 5;
			}
			else
			{
				projectile.scale -= 1f;
			}
			if (projectile.ai[1] > 60 && projectile.scale <= 0.1)
			{
				projectile.Kill();
			}
		}
		public virtual Vector3 GetColor()
		{
			return new Vector3(0, 162 / 255.0f * (255 - projectile.alpha) / 255.0f, 255 / 255.0f * (255 - projectile.alpha) / 255.0f);
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);
		}
		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
			if (projectile.scale < 6)
			{
				Rectangle rectangle = projectile.getRect();
				rectangle.Width = (int)(projectile.width * projectile.scale * 2);
				rectangle.Height = (int)(projectile.height * projectile.scale * 2);
				rectangle.X -= rectangle.Width / 2;
				rectangle.Y -= rectangle.Height / 2;
				hitbox = rectangle;
			}
			base.ModifyDamageHitbox(ref hitbox);
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			var sound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Weapons/Ranged/BruhSFX").WithVolume(Math.Min(0.6f, damage * 3 / (float)projectile.damage));
			Main.PlaySound(sound, target.position);
		}
	}
}