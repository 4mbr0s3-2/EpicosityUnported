using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Epicosity.Projectiles.Enemy
{
	public class DeadDummy : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dead Dummy");
		}
		public override void SetDefaults()
		{
			projectile.width = 34;
			projectile.height = 48;
			projectile.scale = 1;
			projectile.aiStyle = -1;
			projectile.tileCollide = false;
		}
		public override void AI()
		{
			projectile.spriteDirection = -(int)(projectile.velocity.X / Math.Abs(projectile.velocity.X));
			if (projectile.scale == 1)
			{
				projectile.velocity.X += Main.rand.NextFloat(-1, 1);
			}
			projectile.scale += 0.04f;
			projectile.velocity.Y += 0.6f;
			projectile.rotation += projectile.velocity.X / 20.0f;
			projectile.alpha += 2;
			if (projectile.scale > 5)
			{
				projectile.Kill();
			}
		}
	}
}
