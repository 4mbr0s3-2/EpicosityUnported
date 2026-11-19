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

namespace Epicosity.Projectiles.Enemy
{
	public class JevilProjectile : ModProjectile
	{
		public override bool Autoload(ref string name)
		{
			return false;
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Jevil Projectile");
		}

		public override void SetDefaults()
		{
			//projectile.arrow = true;
			projectile.width = 34;
			projectile.height = 36;
			projectile.aiStyle = -1;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.light = 0.0f;
			projectile.penetrate = -1;
			projectile.ai[1] = 0;
			projectile.timeLeft = 240;
		}
		public override void AI()
		{
			Lighting.AddLight(projectile.position, new Vector3((255 - projectile.alpha) / 255.0f, (255 - projectile.alpha) / 255.0f, (255 - projectile.alpha) / 255.0f));
			//mod.Logger.Info("bruh");
			//base.AI();d
			if (projectile.ai[1] < 3) // Give more leeway for syncing
			{
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
			}
			if (projectile.timeLeft < 60)
			{
				projectile.alpha = (int)((60 - projectile.timeLeft) / 60f * 255);
			}
		}
	}
}