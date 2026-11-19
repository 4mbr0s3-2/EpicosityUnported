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
	public class Club : JevilProjectile
	{
		float size = 0.5f;
		public override bool Autoload(ref string name)
		{
			return true;
		}
		public override void SetDefaults()
		{
			projectile.scale = size;
			base.SetDefaults();
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Club");
		}
	}
}