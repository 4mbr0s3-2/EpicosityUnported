using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Epicosity.Tiles
{
    class BreakableChest
    {
        internal static void DestroyChest(ILContext il)
        {
            var c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchRet()))
            {
                Epicosity.inst.Logger.Info("Unable to patch breakable chest!");
                return; // Patch unable to be applied
            }
            c.Index--;
            c.RemoveRange(2);
            c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc_1); // Chest
            c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc_2); // Item index
            c.EmitDelegate<Action<Chest, int>>((chest, itemIndex) =>
            {
                int index = Item.NewItem(new Microsoft.Xna.Framework.Vector2(chest.x, chest.y).ToWorldCoordinates(), 1, 1, chest.item[itemIndex].type, chest.item[itemIndex].stack, false, chest.item[itemIndex].prefix);
                Main.item[index].CloneWithModdedDataFrom(chest.item[itemIndex]);
            });
        }
        internal static void CanDestroyChest(ILContext il)
        {
            var c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchRet()))
            {
                Epicosity.inst.Logger.Info("Unable to patch breakable chest!");
                return; // Patch unable to be applied
            }
            c.Index--;
            c.RemoveRange(1);
            c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc_1); // Load argument 1 as a parameter
            c.EmitDelegate<Func<Chest, bool>>((chest) =>
            {
                return !Chest.isLocked(chest.x, chest.y);
            }); // The game then returns the return value from here
        }
    }
}
