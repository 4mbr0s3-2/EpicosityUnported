using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
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
	public class BulletBill : ModProjectile
	{
		public bool DeadEffect = false;
		public override void SetStaticDefaults()
		{
		}
		public override void SetDefaults()
		{
			projectile.width = 32; // Hitbox should be square
			projectile.height = 32;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.timeLeft = 300;
		}
		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(DeadEffect);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			DeadEffect = reader.ReadBoolean();
		}
		public override void Kill(int timeLeft)
		{
			if (timeLeft <= 0) // Kill silently without spawning new projectiles if died from timeLeft
			{
				return;
			}
			if (Main.myPlayer == projectile.owner && !DeadEffect)
			{
				// Summon dead effect
				int projIndex = Projectile.NewProjectile(projectile.Center, new Vector2(-projectile.velocity.X / Math.Abs(projectile.velocity.X) * 3, -5), ProjectileType<BulletBill>(), 0, 0, projectile.owner);
				if (Main.projectile[projIndex].modProjectile != null)
				{
					((BulletBill)Main.projectile[projIndex].modProjectile).DeadEffect = true;
					Main.projectile[projIndex].timeLeft = 120;
					Main.projectile[projIndex].netUpdate2 = true;
				}
				if (Main.netMode != NetmodeID.Server)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Weapons/Ranged/BillyGunHit"), projectile.Center);
				}
			}
		}
		public override void AI()
		{
			if (!DeadEffect)
			{
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
				if (projectile.velocity.X < 0)
				{
					projectile.spriteDirection = -1;
				}
			}
			else
			{
				projectile.tileCollide = false;
				projectile.velocity.Y += 0.2f;
				if (projectile.velocity.X < 0)
				{
					projectile.rotation = MathHelper.PiOver2 * 1;
					projectile.spriteDirection = -1;
				}
				else
				{
					projectile.rotation = MathHelper.PiOver2 * 3;
				}
			}
			// Alpha transition for polish
			if (projectile.timeLeft < 30)
			{
				projectile.alpha = (int)(((30 - projectile.timeLeft) / 30f) * 255);
			}
		}
	}
}
