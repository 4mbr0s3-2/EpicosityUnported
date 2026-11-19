using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Epicosity.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.DataStructures;

namespace Epicosity.Items.Rarity
{
	public partial class EpicosityItemRarity
	{
		public class RarityWorkInProgress : EpicosityRarity
		{
			public override Color? GetColor()
			{
				return Color.White;
			}
			public override void PostDrawRarityText()
			{
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
				MiscShaderData wipRarity = GameShaders.Misc[ShaderNames.RarityWIP];
				DrawData rarity = new DrawData(rarityRenderTarget, new Rectangle(Main.mouseX, Main.mouseY, 200, 200), Color.White);
				wipRarity.Apply(rarity);
				rarity.Draw(Main.spriteBatch);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
			}
			public override bool HideNameTooltip()
			{
				return true;
			}
		}
	}
}
