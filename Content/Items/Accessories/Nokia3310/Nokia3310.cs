using Epicosity.Items.Tiles.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Epicosity.Items.Equipables.Accessories
{
    public class Nokia3310 : ModItem
    {
        // TODO: REDO CODE TO WORK IN MULTIPLAYER
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nokia 3310");
        }
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.PDA);
            item.width = 32;
            item.height = 32;
            item.rare = ItemRarityID.Yellow;
            item.accessory = true;
            item.defense = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 10;
            item.useAnimation = 10;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.accWatch = 3;
            player.accDepthMeter = 1;
            player.accCompass = 1;
            player.accFishFinder = true;
            player.accWeatherRadio = true;
            player.accCalendar = true;
            player.accThirdEye = true;
            player.accJarOfSouls = true;
            player.accCritterGuide = true;
            player.accStopwatch = true;
            player.accOreFinder = true;
            player.accDreamCatcher = true;
            if (player.GetModPlayer<ModPlayerNokiaRing>().phoneRinging)
            {
                item.defense = 0;
            }
            else
            {
                item.defense = 25;
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            ModPlayerNokiaRing nokiaPlayer = Main.LocalPlayer.GetModPlayer<ModPlayerNokiaRing>();
            if (nokiaPlayer.callersQueue.Count() > 0)
            {
                tooltips.Add(new TooltipLine(mod, "Tooltip1", "Will ring occasionally"));
            }
            else
            {
                tooltips.Add(new TooltipLine(mod, "Tooltip1", "Will no longer ring"));
            }
            if (nokiaPlayer.phoneRinging)
            {
                string caller = nokiaPlayer.caller;
                string beVerb = "is";
                if (caller == "Sky Cat and Ceiling Cat")
                {
                    beVerb = "are";
                }
                tooltips.Add(new TooltipLine(mod, "Tooltip2", "[c/FFFF00:" + caller + "] " + beVerb + " calling you!"));
                tooltips.Add(new TooltipLine(mod, "Tooltip3", "Use to answer"));
            }
        }
        public override bool UseItem(Player player)
        {
            ModPlayerNokiaRing nokiaPlayer = Main.LocalPlayer.GetModPlayer<ModPlayerNokiaRing>();
            if (nokiaPlayer.phoneRinging)
            {
                nokiaPlayer.phoneRinging = false;
                LegacySoundStyle phoneCall = mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Item, "Sounds/Item/PhoneCalls/" + nokiaPlayer.callerID);
                SoundEffectInstance sfx = Main.PlaySound(phoneCall, player.Center);
                sfx.Pitch = 0; // This is important!

                Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Item, "Sounds/Item/NokiaPickup"), player.Center);
                return true;
            }
            return false;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            float rotation = 0;
            ModPlayerNokiaRing nokiaPlayer = Main.LocalPlayer.GetModPlayer<ModPlayerNokiaRing>();
            if (nokiaPlayer.phoneRinging)
            {
                if (nokiaPlayer.phoneRingAnimationFrame < 90)
                {
                    rotation = (float)Math.Sin(nokiaPlayer.phoneRingAnimationFrame * 256 * Math.PI / 360) * 0.2f;
                }
            }
            spriteBatch.Draw
            (
                ModContent.GetTexture(Texture),
                new Rectangle((int)(position.X + 16 * scale), (int)(position.Y + 16 * scale), (int)(32 * scale), (int)(32 * scale)),
                new Rectangle(0, 0, 32, 32),
                Color.White,
                rotation,
                new Vector2(16, 16),
                SpriteEffects.None,
                0
            );
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PDA, 1);
            recipe.AddIngredient(ModContent.ItemType<MemeiteBar>(), 20);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
    public class ModPlayerNokiaRing : ModPlayer
    {
        public override bool CloneNewInstances => true;
		SoundEffectInstance ringtone = null;
        bool playingSound = false;
        public string callerID = "Hamood";
        public string caller = "Hamood";
        public bool phoneRinging = false;
        public int phoneRingAnimationFrame = 0;
        public bool addUnderwaterClip = false;
        public int timeTillRing = 180;
        /// <summary>
        /// Whether or not callersQueue has been filled yet.
        /// </summary>
        public bool initialized = false;
        /// <summary>
        /// The player's list of callers. The key is the sound file's name, and the value is the display name.
        /// </summary>
        public List<string> callersQueue = new List<string>();
        public List<string> callersNameQueue = new List<string>();
        public List<string> called = new List<string>();
        public override TagCompound Save()
        {
            return new TagCompound
            {
                {"callersQueue", callersQueue},
                {"initialized", initialized},
                {"addUnderwaterClip", addUnderwaterClip},
                {"timeTillRing", timeTillRing},
                {"called", called}
            };
        }
        public override void Load(TagCompound tag)
        {
            callersQueue = (List<string>)tag.GetList<string>("callersQueue");
            initialized = tag.GetBool("initialized");
            addUnderwaterClip = tag.GetBool("addUnderwaterClip");
            timeTillRing = tag.GetInt("timeTillRing");
            called = (List<string>)tag.GetList<string>("called");
        }
        public bool PhoneInInventory()
        {
            for (int x = 0; x < 10; x++)
            {
                Item item = player.armor[x];
                if (item.type == ModContent.ItemType<Nokia3310>() && item.stack > 0)
                {
                    return true;
                }
            }
            foreach (Item item in player.inventory)
            {
                if (item.type == ModContent.ItemType<Nokia3310>() && item.stack > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool CanRing()
        {
            if (callersQueue.Count <= 0 || phoneRinging) // Can't have multiple callers
            {
                return false;
            }
            return PhoneInInventory();
        }
        public override void PostUpdate()
        {
            if (!initialized && callersQueue.Count() == 0)
            {
                initialized = true;

                callersQueue.Add("Hamood");
                callersQueue.Add("DClass");
                callersQueue.Add("McDonaldsCustomer");
                callersQueue.Add("PhoneGuy");
                callersQueue.Add("RoccoBotte");
                callersQueue.Add("SkyAndCeilingCats");
            }
            if (player.wet && !addUnderwaterClip && Main.raining)
            {
                addUnderwaterClip = true;
                callersQueue.Add("Underwater");
                if (CanRing())
                {
                    RingPlayer(player);
                }
            }
            if (timeTillRing > 0)
            {
                timeTillRing--;
            }
            if (CanRing() && timeTillRing <= 0)
            {
                RingPlayer(player);
            }
            if (!PhoneInInventory() && phoneRinging)
            {
                phoneRinging = false;
            }
            if (phoneRinging)
            {
                phoneRingAnimationFrame++;
                if (phoneRingAnimationFrame > 180)
                {
                    phoneRingAnimationFrame = 0;
                }
            }
            #region Ring sound code
            if (phoneRinging)
            {
                if (ringtone == null || ringtone.State == Microsoft.Xna.Framework.Audio.SoundState.Stopped)
                {
                    Terraria.Audio.LegacySoundStyle ringtoneSound = mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Item, "Sounds/Item/NokiaArabicRingtone");
                    ringtone = Main.PlaySound(ringtoneSound.SoundId, (int)player.position.X, (int)player.position.Y, ringtoneSound.Style, 1, 0);
                    playingSound = true;
                }
            }
            else
            {
                if (playingSound)
                {
                    playingSound = false;
                    if (ringtone != null) ringtone.Stop();
                }
            }
            #endregion
        }
        public void RingPlayer(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
			{
                timeTillRing = Main.rand.Next(25200, 72000); // Between 1-10 minutes
			}
            // TODO: Sync phoneRinging and timeTillRing across multiplayer OR redo code
            // Zero callers and overlapping callers are accounted for in CanRing()
            callerID = callersQueue.First();
            switch (callerID)
            {
                case "Hamood":
                    caller = "Hamood";
                    break;
                case "DClass":
                    caller = "D-Class Personnel";
                    break;
                case "McDonaldsCustomer":
                    caller = "McDonald's Customer";
                    break;
                case "PhoneGuy":
                    caller = "Phone Guy";
                    break;
                case "RoccoBotte":
                    caller = "Rocco Botte";
                    break;
                case "SkyAndCeilingCats":
                    caller = "Sky Cat and Ceiling Cat";
                    break;
                case "Underwater":
                    caller = "Underwater";
                    break;
                default:
                    caller = "missingno.";
                    break;
            }
            callersQueue.Remove(callerID);
            called.Add(callerID);
            // Make the phone ring
            for (int x = 0; x < 10; x++)
            {
                Item item = player.armor[x];
                if (item.type == ModContent.ItemType<Nokia3310>() && item.stack > 0)
                {
                    phoneRinging = true;
                    break;
                }
            }
            if (!phoneRinging) // If not in accessory slots, check inventory
            {
                foreach (Item item in player.inventory)
                {
                    if (item.type == ModContent.ItemType<Nokia3310>() && item.stack > 0)
                    {
                        phoneRinging = true;
                    }
                }
            }
        }
    }
}
