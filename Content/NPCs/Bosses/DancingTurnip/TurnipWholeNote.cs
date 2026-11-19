using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Epicosity.Projectiles.Enemy
{
	public class TurnipWholeNote : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.hostile = true;
			projectile.aiStyle = -1;
			projectile.width = 24;
			projectile.height = 24;
			projectile.tileCollide = false;
		}
		public override void AI()
		{
			projectile.velocity /= 1.1f;
			if (projectile.velocity.Length() < 1f)
			{
				for (int x = 0; x < 10; x++)
				{
					float rotation = MathHelper.TwoPi * x / 10f;
					Vector2 rotationVector = rotation.ToRotationVector2();
					Dust.NewDust(projectile.position, projectile.width, projectile.height, 74, rotationVector.X * 5, rotationVector.Y * 5);
				}
				if (projectile.owner == Main.myPlayer || projectile.owner == 255)
				{
					Split();
				}
				projectile.Kill();
			}
		}
		public virtual void Split()
		{
			for (int x = 0; x < 2; x++)
			{
				Projectile.NewProjectile(projectile.position, new Vector2(15, 0).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<TurnipHalfNote>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
			}
		}
	}
	public class TurnipHalfNote : TurnipWholeNote
	{
		public override void Split()
		{
			for (int x = 0; x < 2; x++)
			{
				Projectile.NewProjectile(projectile.position, new Vector2(15, 0).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<TurnipQuarterNote>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
			}
		}
	}
	public class TurnipQuarterNote : TurnipWholeNote
	{
		public override void Split()
		{
			for (int x = 0; x < 2; x++)
			{
				Projectile.NewProjectile(projectile.position, new Vector2(15, 0).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<TurnipEighthNote>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
			}
		}
	}
	public class TurnipEighthNote : TurnipWholeNote
	{
		public override void Split()
		{
		}
	}
}
