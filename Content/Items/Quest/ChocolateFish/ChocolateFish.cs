using Epicosity.Environment;
using Epicosity.Environment.ChocolateRain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.Items.Quest
{
	public class ChocolateFish : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chocolate Fish");
		}

		public override void SetDefaults()
		{
			item.questItem = true;
			item.maxStack = 1;
			item.width = 32;
			item.height = 32;
			item.uniqueStack = true;
			item.rare = ItemRarityID.Quest;
		}

		public override bool IsQuestFish()
		{
			return true;
		}

		public override bool IsAnglerQuestAvailable() {
			return ChocolateRainManager.IsRainingChocolate;
		}

		public override void AnglerQuestChat(ref string description, ref string catchLocation)
		{
			description = "During the Chocolate Rain, my hand started to slowly dissolve! I knew the nurse could fix me, so I kept on fishing! I saw something strange during that experience though, when I reeled up half of a fish that looked liked chocolate! It tasted amazing, so guess what I want out of you? I need to get that fish again, and you'll get it for me!";
			catchLocation = "Caught anywhere during Chocolate Rain";
		}
	}
	public class ModPlayerCatchChocolateFish : ModPlayer
	{

		public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
		{
			if (junk)
			{
				return;
			}
			if (questFish == ModContent.ItemType<ChocolateFish>() && Main.rand.Next(9) == 0)   // layer.ZoneBeach (is the zone required to catch the fish(zonebeach is the ocean biome))|||||||||| (player.FindBuffIndex(BuffID.BabySlime) > -1 defines the codition(example: if the player has that buff then you can catch that fish otherwise you can't)||||, the liquid where type where you can catch the fish (== 0 is water, 1 is lava, 2 is honey)||||, the time(if it's day or night to be able to catch the fish(Main.dayTime is day and !Main.dayTime is night))|||||, questFish == mod.ItemType("CustomFishingQuest") this tells the game that you need to have the customquestfish to be able to cath the fish||||, and  Main.rand.Next(1) == 0) is the chatch % chance to catch the customfish
			{
				caughtType = ModContent.ItemType<ChocolateFish>(); //what fish to catch under these conditions.
			}
		}

	}

}