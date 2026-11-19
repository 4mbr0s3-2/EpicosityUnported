using Epicosity.Buffs.Summons;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.Projectiles.Minions
{
	public class KilleroombaMinion : ModProjectile
	{
		bool spawnedKnife;
		public override string Texture => "Epicosity/Projectiles/Minions/KilleroombaMinion";
		public Vector2 targetCenter;
		public bool foundTarget;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Killeroomba");
			Main.projFrames[projectile.type] = 1;
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
			Main.projPet[projectile.type] = false;
			ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
			ProjectileID.Sets.Homing[projectile.type] = true; // Damage reduction
		}
		public override bool MinionContactDamage()
		{
			return true;
		}
		public sealed override void SetDefaults()
		{
			projectile.width = 34;
			projectile.height = 12;
			// Prevent the minion from dying on collision
			projectile.tileCollide = false;
			// These below are needed for a minion weapon
			// Only controls if it deals damage to enemies on contact (more on that later)
			projectile.friendly = true;
			// Only determines the damage type
			projectile.minion = true;
			// Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			projectile.minionSlots = 1f;
			// Needed so the minion doesn't despawn on collision with enemies or tiles
			projectile.penetrate = -1;
			projectile.damage = 30; // Modified by the summoning item itself
			projectile.ignoreWater = true;
		}
		public override void AI()
		{
			Player player = Main.player[projectile.owner];
			if (!spawnedKnife)
			{
				spawnedKnife = true;
				if (Main.myPlayer == projectile.owner)
				{
					KilleroombaKnife killeroombaKnife = Main.projectile[Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<KilleroombaKnife>(), projectile.damage, projectile.knockBack, projectile.owner)].modProjectile as KilleroombaKnife;
					killeroombaKnife.killeroombaMinionIndex = projectile.whoAmI;
					killeroombaKnife.projectile.netUpdate2 = true;
				}
			}

			#region Active check
			// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
			if (player.dead || !player.active)
			{
				player.ClearBuff(ModContent.BuffType<KilleroombaBuff>());
			}
			if (player.HasBuff(ModContent.BuffType<KilleroombaBuff>()))
			{
				projectile.timeLeft = 2; // Automatically dies if this isn't maintained
			}
			#endregion

			#region Targeting and idle position
			Vector2 idlePosition = player.Bottom + new Vector2(0, -projectile.height); // Spawns flat on ground

			idlePosition.X += projectile.minionPos * projectile.width * 0.8f * -player.direction;

			Vector2 vectorToIdlePosition = idlePosition - projectile.Center;
			float distanceToIdlePosition = vectorToIdlePosition.Length();
			if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > 1000f)
			{
				projectile.position = idlePosition;
				projectile.velocity *= 0.1f;
				projectile.netUpdate = true;
			}
			#endregion
			#region Find target
			float distanceFromTarget = 700f;
			targetCenter = projectile.position;
			foundTarget = false;

			// The player has specifically targeted an NPC
			if (player.HasMinionAttackTargetNPC)
			{
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, projectile.Center);
				if (between < 700f)
				{
					distanceFromTarget = between;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}
			// If there was no specific target, look for one
			if (!foundTarget)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					if (npc.CanBeChasedBy())
					{
						float between = Vector2.Distance(npc.Center, projectile.Center);
						bool closest = Vector2.Distance(projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
						bool closeThroughWall = between < 300f;
						// If the NPC is the closest so far and is within range and can be seen, use that target
						if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
						{
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}
			#endregion
			#region Movement and collision
			float speedCap = 5;
			if (projectile.velocity.X == 0 && Framing.GetTileSafely(projectile.Right).active())
			{
				projectile.velocity.Y = -3;
			}
			if (projectile.velocity.X == 0 && Framing.GetTileSafely(projectile.Left + new Vector2(-8, 0)).active())
			{
				projectile.velocity.Y = -3;
			}
			if (!foundTarget)
			{
				if (projectile.Center.X > idlePosition.X)
				{
					projectile.velocity.X -= 0.1f;
					projectile.velocity.X = Math.Max(-speedCap, projectile.velocity.X);
				}
				else
				{
					projectile.velocity.X += 0.1f;
					projectile.velocity.X = Math.Min(speedCap, projectile.velocity.X);
				}
			}
			else
			{
				if (Framing.GetTileSafely(projectile.Bottom).active() && targetCenter.Y < projectile.Center.Y - 20 && projectile.velocity.Y == 0)
				{
					projectile.velocity.Y = -6;
				}
				if (projectile.Center.X > targetCenter.X)
				{
					projectile.velocity.X -= 0.1f;
					projectile.velocity.X = Math.Max(-speedCap, projectile.velocity.X);
				}
				else
				{
					projectile.velocity.X += 0.1f;
					projectile.velocity.X = Math.Min(speedCap, projectile.velocity.X);
				}
				// Jump if the target is higher and if there's proper ground below
			}
			if (Collision.WetCollision(projectile.position, projectile.width, projectile.height))
			{
				projectile.velocity.Y -= 0.6f;
			}
			if (projectile.velocity.X != 0)
			{
				projectile.direction = (int)(Math.Abs(projectile.velocity.X) / projectile.velocity.X);
			}
			projectile.rotation = projectile.velocity.Y * 0.1f * projectile.direction;
			projectile.velocity.Y += 0.4f;
			// Perform tile collision
			Vector4 slopeCollision = Collision.SlopeCollision(projectile.position, projectile.velocity, projectile.width, projectile.height);
			projectile.position = slopeCollision.XY();
			projectile.velocity = slopeCollision.ZW();
			projectile.velocity = Collision.TileCollision(projectile.position, projectile.velocity, projectile.width, projectile.height, true);
			#endregion
		}
	}
}
