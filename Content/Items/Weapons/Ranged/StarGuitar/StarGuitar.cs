using Epicosity.Items.Tiles.Ores;
using Epicosity.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using static Terraria.ModLoader.ModContent;

namespace Epicosity.Items.Weapons.Ranged
{
    public class ModPlayerStarGuitarGlowMask : ModPlayer
    {
        // TODO: Make helper for other weapons that will also use glow mask
        public void DrawStarGuitarGlowMask(PlayerDrawInfo info)
        {
            Player player = info.drawPlayer;
            bool symphonicStarGuitar = false;
            Mod epicosityThorium = ModLoader.GetMod("EpicosityThorium");
            if (epicosityThorium != null)
            {
                symphonicStarGuitar = player.HeldItem.type == epicosityThorium.ItemType("SymphonicStarGuitar");
            }
            bool holdingStarGuitar = player.HeldItem.type == ModContent.ItemType<StarGuitar>() || symphonicStarGuitar;
            if (holdingStarGuitar && !player.dead)
            {
                Texture2D tex = ModContent.GetTexture("Epicosity/Items/Weapons/Ranged/StarGuitar");
                Vector2 actualPosition = info.itemLocation;
                Main.playerDrawData.Add
                (
                    new DrawData
                    (
                        tex,
                        new Vector2((int)(actualPosition.X - Main.screenPosition.X), (int)(actualPosition.Y - Main.screenPosition.Y)),
                        tex.Frame(),
                        Color.White * (1 - info.shadow),
                        player.itemRotation,
                        new Vector2(player.direction == 1 ? 0 : tex.Width, tex.Height),
                        player.HeldItem.scale,
                        info.spriteEffects,
                        0
                    )
                );
            }
        }
        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            PlayerLayer heldItem = new PlayerLayer("Epicosity", "StarGuitarHeldItem", PlayerLayer.HeldItem, (info) => DrawStarGuitarGlowMask(info));
            layers.Insert(layers.IndexOf(layers.FirstOrDefault(n => n.Name == "Arms")), heldItem);
        }
    }
    public class StarGuitar : ModItem
    {
        public override bool Autoload(ref string name)
        {
            return false;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solar Guitar");
            Tooltip.SetDefault("Right click to toggle rocking to 'BRODYQUEST' by Neil Cicierega");
        }
		public override void SetDefaults()
		{
			SetDefaults(item);
		}
		public static void SetDefaults(Item item)
        {
            item.ranged = true;
            item.damage = 130;
            item.width = 74;
            item.height = 78;
            item.useStyle = CustomItemUseStyleID.Custom;
            item.useTime = 10;
            item.useAnimation = 10;
            item.holdStyle = CustomItemHoldStyleID.Custom;
            item.shoot = ProjectileID.FallingStar;
            item.shootSpeed = 10;
            item.autoReuse = true;
            item.rare = ItemRarityID.Red;
        }
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			return Shoot(player);
		}
		public static bool Shoot(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.GetModPlayer<ModPlayerStarGuitarAnimation>().enabledBrodyquest = !player.GetModPlayer<ModPlayerStarGuitarAnimation>().enabledBrodyquest;
                return false;
            }
            float percentDist = (Main.MouseWorld - player.Center).Length() / (Main.screenHeight / 2);
            string note;
            if (percentDist < 0.25)
            {
                note = "A";
            }
            else if (percentDist < 0.5)
            {
                note = "C";
            }
            else if (percentDist < 0.75)
            {
                note = "C2";
            }
            else
            {
                note = "F";
            }
            LegacySoundStyle chord = Epicosity.inst.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Item, "Sounds/Item/Weapons/Ranged/StarGuitar" + note);
            Main.PlaySound(chord, player.Center);
            return true;
        }
		public override void HoldStyle(Player player)
		{
            HoldStyle(item, player);
		}
		public static void HoldStyle(Item item, Player player)
        {
            player.itemLocation = player.MountedCenter + new Microsoft.Xna.Framework.Vector2(-18 * player.direction, 35);
            player.itemRotation = (float)Math.Sin(player.itemAnimation) * 0.1f * ((float)player.itemAnimation / (float)item.useAnimation);
            Animation(player);
        }
		public override bool UseItemFrame(Player player)
		{
			return UseItemFrame(item, player);
		}
		public static bool UseItemFrame(Item item, Player player)
        {
            // For some reason UseStyle leaves a one frame gap before being called so this is the only solution
            player.itemLocation = player.MountedCenter + new Microsoft.Xna.Framework.Vector2(-18 * player.direction, 35);
            player.itemRotation = (float)Math.Sin(player.itemAnimation) * 0.1f * ((float)player.itemAnimation / (float)item.useAnimation);
            Animation(player);
            return true;
        }
		public override void UpdateInventory(Player player)
		{
			UpdateInventory(item, player);
		}
		public static void UpdateInventory(Item item, Player player)
        {
            if (player.HeldItem == item)
            {
                player.showItemIcon = true;
                player.showItemIcon2 = -1;
                float percentDist = (Main.MouseWorld - player.Center).Length() / (Main.screenHeight / 2);
                if (percentDist < 0.25)
                {
                    player.showItemIconText = "A5";
                }
                else if (percentDist < 0.5)
                {
                    player.showItemIconText = "C5";
                }
                else if (percentDist < 0.75)
                {
                    player.showItemIconText = "C6no3";
                }
                else
                {
                    player.showItemIconText = "F5";
                }
            }
        }
        public static void Animation(Player player)
        {
            ModPlayerStarGuitarAnimation animation = player.GetModPlayer<ModPlayerStarGuitarAnimation>();
            animation.animationFrame++;
            if (animation.animationFrame > 0)
            {
                animation.targetRotation = 0f;
            }
            if (animation.animationFrame > 38 / 2)
            {
                animation.targetRotation = 0.2f;
            }
            if (animation.animationFrame > 77 / 2)
            {
                animation.targetRotation = 0f;
            }
            if (animation.animationFrame > 115 / 2)
            {
                animation.targetRotation = -0.2f;
            }
            if (animation.animationFrame >= 153 / 2)
            {
                animation.animationFrame = 0;
            }
        }
		public override bool AltFunctionUse(Player player)
		{
			return AltFunctionUse(item, player);
		}
		public static bool AltFunctionUse(Item item, Player player)
        {
            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarBar, 12);
            recipe.AddIngredient(ItemID.FragmentSolar, 36);
            recipe.AddIngredient(ModContent.ItemType<MemeiteBar>(), 20);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
    public class ModPlayerStarGuitarAnimation : ModPlayer
    {
        public int animationFrame = 0;
        public float targetRotation = 0;
        public bool enabledBrodyquest = false;
        bool heldItem = false;
        public override void PostUpdate()
        {
            bool symphonicStarGuitar = false;
            Mod epicosityThorium = ModLoader.GetMod("EpicosityThorium");
            if (epicosityThorium != null)
			{
                symphonicStarGuitar = player.HeldItem.type == epicosityThorium.ItemType("SymphonicStarGuitar");
            }
            bool holdingStarGuitar = player.HeldItem.type == ModContent.ItemType<StarGuitar>() || symphonicStarGuitar;
            if (holdingStarGuitar)
            {
                player.bodyFrame.Y = player.bodyFrame.Height * 4;
            }
            if (holdingStarGuitar && enabledBrodyquest)
            {
                player.fullRotationOrigin = new Microsoft.Xna.Framework.Vector2(player.width / 2, player.height);
                heldItem = true;
                if (player.fullRotation > targetRotation)
                {
                    player.fullRotation -= 0.03f;
                }
                if (player.fullRotation < targetRotation)
                {
                    player.fullRotation += 0.03f;
                }
                player.gfxOffY = Math.Abs(player.fullRotation) * 20;
            }
            else if (heldItem)
            {
                if (player.fullRotation != 0 || player.gfxOffY != 0)
                {
                    player.fullRotation = 0;
                    player.gfxOffY = 0;
                }
                animationFrame = 0;
                heldItem = false;
            }
        }
    }
}
