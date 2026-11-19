using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Epicosity.Projectiles.Minions
{
	public class KilleroombaKnife : ModProjectile
	{
		public override string Texture => "Epicosity/Projectiles/Minions/KilleroombaKnife";
		public int killeroombaMinionIndex = -1;
		int direction = 1;
		public override void SetDefaults()
		{
			projectile.width = 24;
			projectile.height = 26;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.aiStyle = -1;
			projectile.penetrate = -1;
		}
		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(killeroombaMinionIndex);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			killeroombaMinionIndex = reader.ReadInt32();
		}
		public override void AI()
		{
			//Player player = Main.player[projectile.owner];
			if (killeroombaMinionIndex >= 0)
			{
				Projectile killeroomba = Main.projectile[killeroombaMinionIndex];
				if (killeroomba.type != ModContent.ProjectileType<KilleroombaMinion>() || !killeroomba.active)
				{
					projectile.Kill();
				}
				else
				{
					projectile.timeLeft = 2;
					KilleroombaMinion killeroombaMinion = killeroomba.modProjectile as KilleroombaMinion;
					float radius = 20;
					projectile.Center = killeroomba.Center;
					if (killeroombaMinion.foundTarget)
					{
						Vector2 rotationVector = killeroombaMinion.targetCenter - killeroomba.Center;
						projectile.rotation = rotationVector.SafeNormalize(Vector2.Zero).ToRotation();
					}
					else
					{
						if (killeroomba.velocity.X != 0)
						{
							// Update takes place before the killeroomba updates its own position, so we have to calculate it ourselves
							direction = (int)(Math.Abs(killeroomba.velocity.X) / killeroomba.velocity.X);
						}
						projectile.rotation = direction > 0 ? 0 : (float)Math.PI;
					}
					projectile.Center += new Vector2(radius, 0).RotatedBy(projectile.rotation);
					projectile.rotation += (float)Math.PI / 4 + (projectile.Center.X > killeroomba.Center.X ? 0 : (float)Math.PI / 2);
					projectile.spriteDirection = projectile.Center.X > killeroomba.Center.X ? 1 : -1;
				}
			}
		}
	}
}
