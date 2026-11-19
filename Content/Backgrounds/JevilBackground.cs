using Epicosity.Invasions;
using Epicosity.NPCs.Bosses;
using Epicosity.Utilities;
using IL.Terraria.Achievements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Epicosity.Backgrounds
{
	public class JevilBackground : CustomSky
	{
		private bool isActive;
		private float intensity;
		public override void Update(GameTime gameTime)
		{
			if (isActive && intensity < 1f)
			{
				intensity += 0.01f;
			}
			else if (!isActive && intensity > 0f)
			{
				intensity -= 0.01f;
			}
		}
		
		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			//Texture2D frame = ModContent.GetTexture("Epicosity/Backgrounds/JevilBackground/JevilBackground" + frameNumber.ToString().PadLeft(3, '0'));
			Texture2D frame = ModContent.GetTexture("Epicosity/Backgrounds/JevilCarousel");
			if (maxDepth >= 6)
			{
				float speed = 0.3f;
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.instance.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);
				DrawData jevilBackground = new DrawData(frame, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0f, 0f, 0f, 0f));
				GameShaders.Misc[ShaderNames.JevilMarquee].Apply(jevilBackground);
				(GameShaders.Misc[ShaderNames.JevilBackgroundDistortion] as JevilBackgroundShaderData).UseColor(new Color(speed, intensity, 0f)).Apply(jevilBackground);
				//GameShaders.Misc[ShaderNames.JevilMarquee].Apply(jevilBackground);
				jevilBackground.Draw(spriteBatch);
				spriteBatch.End();
				// Same line from Terraria.Main.DoDraw()
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.instance.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);
			}
		}

		public override float GetCloudAlpha()
		{
			return 0f;
		}

		public override void Activate(Vector2 position, params object[] args)
		{
			isActive = true;
		}

		public override void Deactivate(params object[] args)
		{
			isActive = false;
		}

		public override void Reset()
		{
			isActive = false;
		}

		public override bool IsActive()
		{
			return isActive || intensity > 0f;
		}
	}
	public class ModPlayerJevil : ModPlayer
	{
		public override void UpdateBiomeVisuals()
		{
			bool jevil = false;
			foreach (NPC npc in Main.npc)
			{
				if (npc.type == ModContent.NPCType<Jevil>() && npc.active)
				{
					if (!((Jevil)npc.modNPC).fleeing)
					{
						jevil = true;
					}
				}
			}
			player.ManageSpecialBiomeVisuals(ShaderNames.JevilBackground, jevil);
		}
	}
}