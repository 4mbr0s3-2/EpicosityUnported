using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
	public class RudeBuster : ModProjectile
	{
		public bool trailing 
		{
			get
			{
				return projectile.ai[0] == 1 ? true : false;
			}
			set
			{
				projectile.ai[0] = value ? 1 : 0;
			}
		}
		int size = 0;
		bool initialized = false;
		public int time = 0;
		bool isKilled = false;
		public struct RudeBusterTrail
		{
			public Vector2 position;
			public float rotation;
			public int frame;
			public float time;
			public RudeBusterTrail(Vector2 position, float rotation, int frame)
			{
				this.position = position;
				this.rotation = rotation;
				this.frame = frame;
				this.time = 0;
			}
		}
		List<RudeBusterTrail> rudeBusterTrails = new List<RudeBusterTrail>();
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
		}
		public override void SetDefaults()
		{
			projectile.tileCollide = false;
			projectile.penetrate = 1;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.aiStyle = 0;
			projectile.width = 43;
			projectile.height = 62;
			Main.projFrames[projectile.type] = 18;
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (!isKilled)
			{
				if (!trailing)
				{
					if (Main.netMode != NetmodeID.Server)
					{
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Weapons/Ranged/RudeBusterBump"), projectile.position);
					}
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						for (int x = 0; x <= 3; x++)
						{
							int projIndex = Projectile.NewProjectile(projectile.position,
									Vector2.Zero,
									ProjectileType<RudeBuster>(),
									20,
									10,
									projectile.owner,
									1);
							Projectile proj = Main.projectile[projIndex];
							proj.rotation = (float)(Math.PI * 2f / 4f) * x + (float)Math.PI * 0.25f;
							proj.velocity = new Vector2(0, -5).RotatedBy(proj.rotation);
							proj.position = projectile.position;
							proj.frame = projectile.frame;
							proj.alpha = 135;
							proj.scale = 1.5f;
							proj.netUpdate2 = true;
						}
					}
				}
				isKilled = true;
			}
			return true;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = mod.GetTexture("Projectiles/Weapons/Ranged/RudeBuster");
			int width = texture.Width;
			int height = (int)(texture.Height / Main.projFrames[projectile.type] * projectile.scale);
			//if (!trailing)
			//{
			//	width = (int)(texture.Width * projectile.scale);
			//	height = texture.Height / Main.projFrames[projectile.type];
			//	for (int posIndex = 0; posIndex < rudeBusterTrails.Count; posIndex++)
			//	{
			//		RudeBusterTrail trail = rudeBusterTrails[posIndex];
			//		if (posIndex > 4)
			//		{
			//			width = (int)((1f - ((float)(posIndex - 4) / projectile.oldPos.Length)) * width);
			//		}
			//		Vector2 pos = projectile.oldPos[posIndex];
			//		spriteBatch.Draw
			//		(
			//			texture,
			//			new Rectangle
			//			(
			//				(int)(pos.X - Main.screenPosition.X + projectile.width * 0.5f),
			//				(int)(pos.Y - Main.screenPosition.Y + projectile.height * 0.5f),
			//				width,
			//				height
			//			),
			//			new Rectangle(0, texture.Height / Main.projFrames[projectile.type] * projectile.frame, texture.Width, texture.Height / Main.projFrames[projectile.type]),
			//			new Color(255, 255, 255, 255 - projectile.alpha) * (1 - ((float)posIndex / projectile.oldPos.Length)),
			//			projectile.rotation,
			//			new Vector2(texture.Width, texture.Height / Main.projFrames[projectile.type]) * 0.5f,
			//			SpriteEffects.None,
			//			0f
			//		);
			//	}
			//}

			//rudeBusterTrails.Add(new RudeBusterTrail(projectile.position, projectile.rotation, projectile.frame));

			spriteBatch.Draw
			(
				texture,
				new Rectangle
				(
					(int)(projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f),
					(int)(projectile.position.Y - Main.screenPosition.Y + projectile.height * 0.5f),
					width,
					height
				),
				new Rectangle(0, texture.Height / Main.projFrames[projectile.type] * projectile.frame, texture.Width, texture.Height / Main.projFrames[projectile.type]),
				new Color(255, 255, 255, 255 - projectile.alpha),
				projectile.rotation,
				new Vector2(texture.Width, texture.Height / Main.projFrames[projectile.type]) * 0.5f,
				SpriteEffects.None,
				0f
			);
			return false;
		}
		public override void AI()
		{
			if (isKilled)
			{
				projectile.velocity = Vector2.Zero;
			}
			time++;
			if (time == 20)
			{
				projectile.tileCollide = true;
			}
			if (!trailing)
			{
				if (!initialized)
				{
					initialized = true;
					projectile.frame = (int)(Main.rand.NextFloat() * Main.projFrames[projectile.type]);
				}
				projectile.scale = 1.5f;
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

				projectile.frameCounter++;
				if (projectile.frameCounter >= 10)
				{
					projectile.frameCounter = 0;
					projectile.frame++;
					projectile.frame %= 18;
				}

				Utilities.ColorHelper.HsvToRgb(projectile.frame * 20, 255, 255, out int r, out int g, out int b);
				Lighting.AddLight(projectile.position, r / 255.0f, g / 255.0f, b / 255.0f);
			}
			else // Trailing code
			{
				if (!initialized)
				{
					initialized = true;
					projectile.penetrate = 10;
				}
				size += 5;
				projectile.alpha = size + 135;
				projectile.scale = (1f - size / 120.0f) * 1.5f;
				projectile.tileCollide = false;
				if (size >= 120)
				{
					projectile.Kill();
				}
			}
		}
	}
}
