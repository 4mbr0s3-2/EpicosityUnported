using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using Terraria;
using Terraria.ModLoader;

namespace Epicosity.Projectiles.Weapons.Magic
{
	public class Icicle : ModProjectile
	{
		bool init = false;
		IEnumerator stateAI;
		public float AITimer
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
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.aiStyle = -1;
			projectile.width = 20;
			projectile.height = 20;
			projectile.friendly = true;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 255);
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (AITimer < 10 / 60f)
			{
				float progress = AITimer / (10 / 60f);
				Texture2D orb = ModContent.GetTexture($"{nameof(Epicosity)}/Items/Weapons/Magic/IcicleOrb");
				spriteBatch.Draw(orb, projectile.Center - Main.screenPosition, null, Color.White * progress * 0.4f, 0, orb.Size() / 2f, 3 - progress * 2, SpriteEffects.None, 0);
				return false;
			}
			Texture2D tex = ModContent.GetTexture(Texture);
			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White * (1 - (projectile.alpha / 255f)), projectile.rotation, tex.Size() / 2f, 1, SpriteEffects.None, 0);
			return false;
		}
		public IEnumerator StateAI()
		{
			float speedMult = projectile.ai[1];
			float slowSpeed = 2f;
			float fastSpeed = 5f;
			while (AITimer < 10 / 60f)
			{
				projectile.velocity = projectile.rotation.ToRotationVector2() * slowSpeed * speedMult;
				yield return null;
			}
			while (AITimer < 25 / 60f)
			{
				projectile.velocity = projectile.rotation.ToRotationVector2() * fastSpeed * speedMult;
				yield return null;
			}
			float slowFactor = 1;
			while (AITimer < 40 / 60f)
			{
				slowFactor -= 1f / 15;
				projectile.velocity = projectile.rotation.ToRotationVector2() * fastSpeed * slowFactor;
				yield return null;
			}
			int negator = projectile.velocity.X > 0 ? 1 : -1;
			projectile.rotation += MathHelper.PiOver2 * negator;
			while (AITimer < 10)
			{
				projectile.velocity = projectile.rotation.ToRotationVector2() * slowSpeed;
				yield return null;
			}
			while (projectile.alpha < 255)
			{
				projectile.alpha += 5;
				yield return null;
			}
			projectile.Kill();
			yield break;
		}
		public override void AI()
		{
			if (!init)
			{
				projectile.rotation = projectile.velocity.ToRotation();
				init = true;
				stateAI = StateAI();
			}
			stateAI.MoveNext();
			AITimer += 1 / 60f;
		}
	}
}
