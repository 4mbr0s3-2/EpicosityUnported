using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.NPCs.Bosses
{
    class Hijo : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hijo");
        }
        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 30;
            npc.aiStyle = -1;
            npc.damage = 60;
            npc.lifeMax = 300;
            npc.stepSpeed = 10;
            npc.noGravity = true;
			npc.knockBackResist = 0;
			npc.value = Item.buyPrice(0, 0, 10, 0);
        }
        public override void AI()
        {
			float num144 = 1f;
			float num145 = 0.011f;
			npc.TargetClosest(true);
			Vector2 vector15 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num146 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector15.X;
			float num147 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector15.Y;
			float num148 = (float)Math.Sqrt((double)(num146 * num146 + num147 * num147));
			float num149 = num148;
			npc.ai[1] += 1f;
			if (npc.ai[1] > 600f)
			{
				num145 *= 8f;
				num144 = 4f;
				if (npc.ai[1] > 650f)
				{
					npc.ai[1] = 0f;
				}
			}
			else if (num149 < 250f)
			{
				npc.ai[0] += 0.9f;
				if (npc.ai[0] > 0f)
				{
					npc.velocity.Y = npc.velocity.Y + 0.019f;
				}
				else
				{
					npc.velocity.Y = npc.velocity.Y - 0.019f;
				}
				if (npc.ai[0] < -100f || npc.ai[0] > 100f)
				{
					npc.velocity.X = npc.velocity.X + 0.019f;
				}
				else
				{
					npc.velocity.X = npc.velocity.X - 0.019f;
				}
				if (npc.ai[0] > 200f)
				{
					npc.ai[0] = -200f;
				}
			}
			if (num149 > 350f)
			{
				num144 = 5f;
				num145 = 0.3f;
			}
			else if (num149 > 300f)
			{
				num144 = 3f;
				num145 = 0.2f;
			}
			else if (num149 > 250f)
			{
				num144 = 1.5f;
				num145 = 0.1f;
			}
			num148 = num144 / num148;
			num146 *= num148;
			num147 *= num148;
			if (Main.player[npc.target].dead)
			{
				num146 = (float)npc.direction * num144 / 2f;
				num147 = -num144 / 2f;
			}
			if (npc.velocity.X < num146)
			{
				npc.velocity.X = npc.velocity.X + num145;
			}
			else if (npc.velocity.X > num146)
			{
				npc.velocity.X = npc.velocity.X - num145;
			}
			if (npc.velocity.Y < num147)
			{
				npc.velocity.Y = npc.velocity.Y + num145;
			}
			else if (npc.velocity.Y > num147)
			{
				npc.velocity.Y = npc.velocity.Y - num145;
			}
			if (num146 > 0f)
			{
				npc.spriteDirection = -1;
				npc.rotation = (float)Math.Atan2((double)num147, (double)num146);
			}
			if (num146 < 0f)
			{
				npc.spriteDirection = 1;
				npc.rotation = (float)Math.Atan2((double)num147, (double)num146) + 3.14f;
			}
		}
    }
}
