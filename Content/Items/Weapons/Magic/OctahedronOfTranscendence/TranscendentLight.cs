using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using System;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;
using System.Collections.Generic;

namespace Epicosity.Projectiles.Weapons.Ranged
{
	public class TranscendentLight : ModProjectile
	{
		float rotationTime = 0;
		int localPlayer = Main.myPlayer;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Transcendent Light");
		}

		public override void SetDefaults()
		{
			//projectile.arrow = true;
			projectile.width = 1;
			projectile.height = 1;
			projectile.aiStyle = 0;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.tileCollide = false;
			projectile.light = 0.0f;
			projectile.penetrate = 100;
			projectile.alpha = 0;
		}
		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			DrawOctahedron(spriteBatch, finalPoints, 20, Color.Yellow);
			List<Vector2> prevPoints = new List<Vector2>();
			if (rotationTime > 0.5)
			{
				foreach (Vector3 point in Rotate(points, rotationTime - 0.5f))
				{
					prevPoints.Add(new Vector2(point.X, point.Y) * projectile.scale + projectile.position);
				}
				var fadedColor = Color.Yellow;
				fadedColor.A = 100;
				DrawOctahedron(spriteBatch, prevPoints, 20, fadedColor, false);
			}
			prevPoints = new List<Vector2>();
			if (rotationTime > 1)
			{
				foreach (Vector3 point in Rotate(points, rotationTime - 1f))
				{
					prevPoints.Add(new Vector2(point.X, point.Y) * projectile.scale + projectile.position);
				}
				var fadedColor = Color.Yellow;
				fadedColor.A = 50;
				DrawOctahedron(spriteBatch, prevPoints, 20, fadedColor, false);
			}
			return false;
		}
		void DrawOctahedron(SpriteBatch spriteBatch, List<Vector2> finalPoints, int lines, Color color, bool drawDust = true)
		{
			DrawLine(spriteBatch, finalPoints[0], finalPoints[3], 20, color, drawDust);
			DrawLine(spriteBatch, finalPoints[3], finalPoints[1], 20, color, drawDust);
			DrawLine(spriteBatch, finalPoints[1], finalPoints[2], 20, color, drawDust);
			DrawLine(spriteBatch, finalPoints[2], finalPoints[0], 20, color, drawDust);

			DrawLine(spriteBatch, finalPoints[0], finalPoints[4], 20, color, drawDust);
			DrawLine(spriteBatch, finalPoints[1], finalPoints[4], 20, color, drawDust);
			DrawLine(spriteBatch, finalPoints[2], finalPoints[4], 20, color, drawDust);
			DrawLine(spriteBatch, finalPoints[3], finalPoints[4], 20, color, drawDust);

			DrawLine(spriteBatch, finalPoints[0], finalPoints[5], 20, color, drawDust);
			DrawLine(spriteBatch, finalPoints[1], finalPoints[5], 20, color, drawDust);
			DrawLine(spriteBatch, finalPoints[2], finalPoints[5], 20, color, drawDust);
			DrawLine(spriteBatch, finalPoints[3], finalPoints[5], 20, color, drawDust);
		}
		private List<Vector3> RotateZ3D(List<Vector3> points, float theta)
		{
			List<Vector3> result = new List<Vector3>();
			var sinTheta = (float)Math.Sin(theta);
			var cosTheta = (float)Math.Cos(theta);
			for (var n = 0; n < points.Count; n++)
			{
				var node = points[n];
				var x = node.X;
				var y = node.Y;
				node.X = x * cosTheta - y * sinTheta;
				node.Y = y * cosTheta + x * sinTheta;
				result.Add(node);
			}
			return result;
		}
		private List<Vector3> RotateX3D(List<Vector3> points, float theta)
		{
			List<Vector3> result = new List<Vector3>();
			var sinTheta = (float)Math.Sin(theta);
			var cosTheta = (float)Math.Cos(theta);
			for (var n = 0; n < points.Count; n++)
			{
				var node = points[n];
				var y = node.Y;
				var z = node.Z;
				node.Y = y * cosTheta - z * sinTheta;
				node.Z = z * cosTheta + y * sinTheta;
				result.Add(node);
			}
			return result;
		}
		private List<Vector3> RotateY3D(List<Vector3> points, float theta)
		{
			List<Vector3> result = new List<Vector3>();
			var sinTheta = (float)Math.Sin(theta);
			var cosTheta = (float)Math.Cos(theta);
			for (var n = 0; n < points.Count; n++)
			{
				var node = points[n];
				var x = node.X;
				var z = node.Z;
				node.X = x * cosTheta - z * sinTheta;
				node.Z = z * cosTheta + x * sinTheta;
				result.Add(node);
			}
			return result;
		}
		private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, int points, Color color, bool drawDust)
		{
			points = 20;
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Utils.DrawLine(spriteBatch, start, end, color, color, 2f);
			//return;
			for (int x = 0; x < points; x++)
			{
				Vector2 relativeEndPos = end - start;
				relativeEndPos /= points; // Segment it
				Vector2 finalPos = relativeEndPos * x + start;
				if (drawDust)
				{
					Lighting.AddLight(finalPos, new Vector3(0.08f, 0.08f, 0));
					if (Main.rand.Next(50) == 0)
					{
						int dustIndex = Dust.NewDust(finalPos - new Vector2(10, 10), 20, 20, DustID.AncientLight, 0, 0, 0, new Color(255, 255, 0), 1f);
						Dust dust = Main.dust[dustIndex];
						dust.noGravity = true;
						dust.velocity = -Main.player[Main.myPlayer].velocity;
						dust.noLight = true;
					}
				}
			}
			spriteBatch.End();
			spriteBatch.Begin();
		}
		List<Vector3> points = new List<Vector3>() {
			new Vector3(1, 0, 0),
			new Vector3(-1, 0, 0),
			new Vector3(0, 1, 0),
			new Vector3(0, -1, 0),
			new Vector3(0, 0, 1),
			new Vector3(0, 0, -1),
		};
		List<Vector2> finalPoints = new List<Vector2>();
		List<Vector3> Rotate(List<Vector3> points, float rotationTime)
		{
			var drawPoints = RotateX3D(points, rotationTime / 10.0f);
			drawPoints = RotateY3D(drawPoints, rotationTime / 10.0f);
			drawPoints = RotateZ3D(drawPoints, rotationTime / 10.0f);
			return drawPoints;
		}
		public override void AI()
		{
			localPlayer = projectile.owner;
			Main.player[localPlayer].manaRegenDelay = 300;
			bool hasMana = Main.player[localPlayer].CheckMana(1, true, false);
			projectile.position = Main.player[localPlayer].getRect().Center();
			Lighting.AddLight(projectile.position, new Vector3(0.003f * projectile.scale, 0.003f * projectile.scale, 0));
			rotationTime += 0.1f;
			if (rotationTime < 2)
			{
				projectile.scale = (projectile.scale * 4 + 600) / 5.0f;
			}
			if (rotationTime > 2)
			{
				projectile.scale -= 1f;
				if (!Main.player[localPlayer].channel || Main.player[localPlayer].CCed || !hasMana)
				{
					projectile.scale -= 20f;
				}
			}
			if (projectile.scale <= 0)
			{
				projectile.Kill();
			}
			var drawPoints = Rotate(points, rotationTime);
			List<Vector2> finalPoints = new List<Vector2>();
			foreach (Vector3 point in drawPoints)
			{
				finalPoints.Add(new Vector2(point.X, point.Y) * projectile.scale + projectile.position);
			}
			this.finalPoints = finalPoints;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.Aqua;
		}
		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
			Rectangle rectangle = projectile.getRect();
			rectangle.Width = (int)(projectile.width * projectile.scale * 1.5);
			rectangle.Height = (int)(projectile.height * projectile.scale * 1.5);
			rectangle.X -= rectangle.Width / 2;
			rectangle.Y -= rectangle.Height / 2;
			//mod.Logger.Info(rectangle);
			hitbox = rectangle;
			//base.ModifyDamageHitbox(ref hitbox);
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			//var sound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Weapons/Ranged/BruhSFX").WithVolume(Math.Min(1, damage * 5 / (float)projectile.damage));
			Main.PlaySound(SoundID.Item1, target.position);
			//base.OnHitNPC(target, damage, knockback, crit);
		}

		// Additional hooks/methods here.
	}
}