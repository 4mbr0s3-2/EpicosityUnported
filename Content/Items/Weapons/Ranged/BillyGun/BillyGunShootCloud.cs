using Microsoft.Xna.Framework;
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

namespace Epicosity.Projectiles.Weapons.Ranged
{
	public class BillyGunShootCloud : ModProjectile
	{
		public bool DeadEffect = false;
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.width = 32; // Hitbox should be square
			projectile.height = 32;
			projectile.aiStyle = -1;
			projectile.tileCollide = false;
		}
		public override void AI()
		{
			projectile.frameCounter++;
			if (projectile.frameCounter > 5)
			{
				projectile.frameCounter = 0;
				projectile.frame++;
				if (projectile.frame >= 4)
				{
					projectile.Kill();
				}
			}
		}
	}
}
