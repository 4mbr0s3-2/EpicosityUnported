using Microsoft.Xna.Framework;
using System;

namespace Epicosity.Items.Rarity
{
	public partial class EpicosityItemRarity
	{
		public class RarityPulsatingMagenta : EpicosityRarity
		{
			public override Color? GetColor()
			{
				return new Color
				(
					255,
					(int)((Math.Sin(rarityAnimationTime * (float)Math.PI / 360f * 6) * 0.5f + 0.5f) * 255),
					255
				);
			}
		}
	}
}
