using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.Projectiles.Weapons.Melee
{
	public class WoodenTable : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.aiStyle = 1;
		}
		public override void AI()
		{
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}
		public override void Kill(int timeLeft)
		{
			Gore.NewGore(projectile.position, projectile.velocity, mod.GetGoreSlot("Gores/WoodenTable1"), 1f);
			Gore.NewGore(projectile.position, projectile.velocity, mod.GetGoreSlot("Gores/WoodenTable2"), 1f);
			Gore.NewGore(projectile.position, projectile.velocity, mod.GetGoreSlot("Gores/WoodenTable1"), 1f);
			Gore.NewGore(projectile.position, projectile.velocity, mod.GetGoreSlot("Gores/WoodenTable2"), 1f);
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPCs/BarrelDestroy"), projectile.position);
			for (int x = 0; x <= 5; x++)
			{
				Projectile.NewProjectile(projectile.position,
					projectile.velocity + new Vector2(projectile.velocity.X, -1.5f).RotatedBy(Main.rand.NextFloat()),
					ModContent.ProjectileType<Projectiles.Weapons.Melee.Splinter>(),
					3,
					2,
					projectile.owner);
			}
		}
	}
}
