using Terraria;
using Terraria.ModLoader;

namespace Epicosity.Projectiles.Enemy
{
	public class Turnip : ModProjectile
	{
		int time = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Turnip");
			Main.projFrames[projectile.type] = 3;
		}
		public override void SetDefaults()
		{
			projectile.width = 16;
			projectile.height = 24;
			projectile.aiStyle = 1;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.tileCollide = false;
		}
		public override void AI()
		{
			time++;
			projectile.frameCounter++;
			if (projectile.frameCounter >= 5)
			{
				projectile.frameCounter = 0;
				projectile.frame++;
				if (projectile.frame >= Main.projFrames[projectile.type])
				{
					projectile.frame = 0;
				}
			}
			if (time > 60)
			{
				projectile.alpha += 10;
				if (projectile.alpha >= 255)
				{
					projectile.Kill();
				}
			}
		}
	}
}
