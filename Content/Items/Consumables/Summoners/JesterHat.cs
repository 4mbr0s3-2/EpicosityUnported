using Epicosity.NPCs.Bosses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Epicosity.Items.Tiles.Ores;
using Epicosity.Items.CraftingMaterials.Essences;

namespace Epicosity.Items.Consumable.Summoners
{
	public class JesterHat : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Jester Hat");
			Tooltip.SetDefault("'You feel a chaotic energy'\nSummons Jevil");
		}
		public override void SetDefaults()
		{
			item.rare = ItemRarityID.Orange;
			item.useAnimation = 30;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.consumable = true;
		}
		public override bool UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Jevil>());
			if (Main.netMode != NetmodeID.Server)
			{
				Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);
			}
			return true;
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Chaossence>(), 20);
			recipe.AddIngredient(ModContent.ItemType<MemeiteBar>(), 10);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
