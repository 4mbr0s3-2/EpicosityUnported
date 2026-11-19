using Epicosity.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.Projectiles.Weapons.Magic
{
	public class NineBall : ModProjectile
	{
		Microsoft.Xna.Framework.Audio.SoundEffectInstance sound;
		bool init = false;
		IEnumerator stateAI;
		float initAnim = 0;
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
			projectile.aiStyle = -1;
			projectile.width = 34;
			projectile.height = 34;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			return base.PreDraw(spriteBatch, lightColor);
		}
		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			var tex = ModContent.GetTexture(Texture);

			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition + new Vector2(2, projectile.gfxOffY), null, Color.White * (1 - initAnim), 0, tex.Size() / 2, new Vector2(1 + initAnim * 3), SpriteEffects.None, 0);
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 255);
		}
		public IEnumerator StateAI()
		{
			int numOfShoots = 11;
			int numOfPhases = 3;
			int icicle = ModContent.ProjectileType<Icicle>();
			for (int p = 0; p < numOfPhases; p++)
			{
				for (int s = 0; s < numOfShoots; s++)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						for (int j = 0; j < 3; j++)
						{
							for (int i = 0; i < 2; i++)
							{
								float start = MathHelper.Pi * 0.4f;
								float off = (MathHelper.PiOver4 * 3 - start) * ((float)s / numOfShoots);
								float dir = start + off - MathHelper.PiOver2;
								if (i == 1)
								{
									dir = MathHelper.Pi - dir;
								}
								float amp = j + 1;
								Projectile.NewProjectile(projectile.Center, dir.ToRotationVector2(), icicle, projectile.damage, 0, projectile.owner, 0, amp);
							}
						}
					}
					if (Main.netMode != NetmodeID.Server)
					{
						sound = ModContent.GetSound($"{nameof(Epicosity)}/Sounds/Item/Weapons/Magic/Icicle").CreateInstance();
						Utilities.SoundUtil.SetSoundInstancePanAndVolume(sound, (int)projectile.position.X, (int)projectile.position.Y);
						sound.Volume *= 0.3f;
						Main.PlaySoundInstance(sound);
					}
					for (int j = 0; j < 30; j++)
					{
						yield return null;
					}
				}
			}
			for (int p = 0; p < 10; p++)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Ice);
			}
			projectile.Kill();
			yield break;
		}
		public override void AI()
		{
			if (!init)
			{
				init = true;
				stateAI = StateAI();
			}
			if (initAnim < 1) initAnim += 1f / 60f;
			stateAI.MoveNext();
			AITimer += 1f / 60f;
			projectile.gfxOffY = (float)System.Math.Sin(AITimer / 2f * MathHelper.TwoPi) * 5;
		}
	}
}
