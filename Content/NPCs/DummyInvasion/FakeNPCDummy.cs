using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Epicosity.Utilities;

namespace Epicosity.Projectiles.Enemy
{
	public class FakeNPCDummy : ModProjectile
	{
		public override string Texture => PathConstants.PlaceholderTexture;
		public int copyNPCType = -1;
		public int frame = 0;
		public int spriteDirection = 1;
		public DummyAnimation dummyAnimation = DummyAnimation.IdleForTime;
		public enum DummyAnimation
		{
			IdleForTime,
			Knockout
		}
		public Color npcColor = Color.White;
		public Texture2D npcTexture = null;
		public Rectangle drawSourceRect = new Rectangle();
		public Rectangle npcRect = new Rectangle();
		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
			projectile.aiStyle = -1;
			projectile.timeLeft = 360;
			projectile.tileCollide = false;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (npcTexture == null && copyNPCType >= 0)
			{
				if (!Main.NPCLoaded[copyNPCType])
				{
					Main.instance.LoadNPC(copyNPCType);
				}
				npcTexture = Main.npcTexture[copyNPCType];
			}
			else if (npcTexture != null && copyNPCType >= 0)
			{
				SpriteEffects effect = spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
				if (dummyAnimation == DummyAnimation.Knockout)
				{
					effect = SpriteEffects.FlipVertically;
				}
				int frames = Main.npcFrameCount[copyNPCType];
				int npcFrameHeight = npcTexture.Height / frames;
				Rectangle spriteRect = new Rectangle((int)(projectile.position.X - Main.screenPosition.X), (int)(projectile.position.Y - Main.screenPosition.Y), (int)(npcTexture.Width * projectile.scale), (int)(npcFrameHeight * projectile.scale));
				spriteBatch.Draw
				(
					npcTexture,
					spriteRect,
					drawSourceRect,
					lightColor.MultiplyRGBA(npcColor),
					projectile.rotation,
					new Vector2(spriteRect.Width, spriteRect.Height) / 2,
					effect,
					0
				);
			}
			return false;
		}
		public override void AI()
		{
			if (dummyAnimation == DummyAnimation.Knockout)
			{
				projectile.velocity.Y += 0.2f;
			}
		}
	}
}
