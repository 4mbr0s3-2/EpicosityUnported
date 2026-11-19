using Microsoft.Xna.Framework;
using System;

namespace Epicosity.Items.Rarity
{
	public partial class EpicosityItemRarity
	{
		public class RarityPulsatingBlackAndWhite : EpicosityRarity
		{
			public override Color? GetColor()
			{
				int color = (int)((Math.Sin(rarityAnimationTime * 6 * (float)Math.PI / 360f * 2) * 0.5f + 0.5f) * 200) + 35;
				return new Color(color, color, color);
			}
		}
	}
}
