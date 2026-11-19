using Epicosity.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Epicosity.Projectiles.Fun
{
	public class PrimazelaLight : ModProjectile
	{
		float Intensity
		{
			get
			{
				return projectile.ai[0];
			}
			set
			{
				projectile.ai[0] = value;
			}
		}
		public override void SetDefaults()
		{
			projectile.width = 32;
			projectile.height = 32;
			projectile.friendly = true;
			projectile.timeLeft = 300;
			projectile.tileCollide = false;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.Transform);
			//spriteBatch.Draw
			//(
			//	Main.projectileTexture[projectile.type],
			//	new Rectangle((int)projectile.position.X - (int)Main.screenPosition.X, (int)projectile.position.Y - (int)Main.screenPosition.Y, 64, 64),
			//	GetAlpha(lightColor)
			//);
			return true;
		}
		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.Transform);
		}
		public override void AI()
		{
			projectile.velocity *= 0.9f;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			ColorHelper.HsvToRgb(Intensity, 1, 1, out int r, out int g, out int b);
			return new Color(r, g, b, 255) * (projectile.timeLeft / 300f);
		}
	}
}
