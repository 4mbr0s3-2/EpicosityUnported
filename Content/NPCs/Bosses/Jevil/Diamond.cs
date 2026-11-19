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
using System.IO;

namespace Epicosity.Projectiles.Enemy
{
	public class Diamond : JevilProjectile
	{
		int moveTimer = 0;
		public int delay = 0;
		public int speed = 1;
		public override bool Autoload(ref string name)
		{
			return true;
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Diamond");
		}
		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(speed);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			speed = reader.ReadInt32();
		}
		public override void AI()
		{
			base.AI();
			if (projectile.ai[1] < 1)
			{
				projectile.scale = 2;
				projectile.alpha = 255;
			}
			if (projectile.ai[1] < 20)
			{
				projectile.scale -= 0.05f;
				projectile.alpha -= 10;
				projectile.velocity *= 0.5f;
			}
			if (projectile.ai[1] >= delay)
			{
				moveTimer++;
				if (moveTimer > 1 && moveTimer < 31)
				{
					projectile.alpha -= 10;
					projectile.velocity -= new Vector2(0, -0.2f * speed).RotatedBy(projectile.rotation);
				}
				if (moveTimer > 31)
				{
					projectile.velocity += new Vector2(0, -0.13f * speed).RotatedBy(projectile.rotation);
				}
			}
			projectile.ai[1] += 1;
			if (projectile.ai[1] >= 600)
			{
				projectile.Kill();
			}
		}
	}
}