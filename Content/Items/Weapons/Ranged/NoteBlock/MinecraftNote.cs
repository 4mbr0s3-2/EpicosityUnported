using Epicosity.Systems.MemoryManagement;
using Epicosity.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Epicosity.Projectiles.Weapons.Ranged
{
	public class MinecraftNote : ModProjectile
	{
		SoundEffectInstance sound; // This is here to prevent the sound from cutting off
		public enum Material
		{
			Wood,
			Sand,
			Glass,
			Stone,
			Gold,
			Clay,
			Ice,
			Wool,
			Bone,
			Iron,
			SoulSand,
			Pumpkin,
			Emerald,
			HayBale,
			Glowstone
		}
		public static Color[] colors = new Color[]
		{
		};
		[LoadManager(LoadManagerAttribute.ManageType.Load, LoadManagerAttribute.ManageNetmode.Client)]
		public static void LoadColors()
		{
			colors = new Color[] 
			{
				new Color(0x50, 0xd5, 0x00),
				new Color(0x8a, 0xd5, 0x00),
				new Color(0xd5, 0xd5, 0x00),
				new Color(0xd5, 0x8a, 0x00),
				new Color(0xd5, 0x50, 0x00),
				new Color(0xd5, 0x25, 0x00),
				new Color(0xcc, 0x00, 0x00),
				new Color(0xd5, 0x00, 0x25),
				new Color(0xd5, 0x00, 0x50),
				new Color(0xd5, 0x00, 0x85),
				new Color(0xd5, 0x00, 0xd5),
				new Color(0x8a, 0x00, 0xd5),
				new Color(0x50, 0x00, 0xd5),
				new Color(0x30, 0x00, 0xfe),
				new Color(0x00, 0x00, 0xf5),
				new Color(0x00, 0x30, 0xfe),
				new Color(0x00, 0x60, 0xff),
				new Color(0x00, 0x93, 0xea),
				new Color(0x00, 0xd5, 0xd5),
				new Color(0x00, 0xd5, 0x8a),
				new Color(0x00, 0xd5, 0x50),
				new Color(0x00, 0xd5, 0x25),
				new Color(0x00, 0xcc, 0x00),
				new Color(0x29, 0xea, 0x00),
				new Color(0x29, 0xea, 0x00),
				new Color(0x50, 0xd5, 0x00)
			};
		}
		bool initialized = false;
		public float NoteValue
		{
			get
			{
				return projectile.ai[0];
			}
			set
			{
				projectile.ai[0] = value;
			}
		}
		public override void SetDefaults()
		{
			projectile.width = 16;
			projectile.height = 20;
			projectile.friendly = true;
			projectile.penetrate = 1;
		}
		public override void AI()
		{
			// Play sound
			if (!initialized)
			{
				SoundEffectInstance soundEffectInstance = ModContent.GetSound($"{typeof(Epicosity).Name}/Sounds/Item/Weapons/Ranged/MinecraftNoteHarp").CreateInstance();
				SoundUtil.SetSoundInstancePanAndVolume(soundEffectInstance, (int)projectile.position.X, (int)projectile.position.Y);
				soundEffectInstance.Pitch = (float)((NoteValue - 12) / 12);
				sound = soundEffectInstance;
				//Main.NewText(soundEffectInstance.Pitch);
				Main.PlaySoundInstance(sound);
				initialized = true;
			}
		}
		public override void Kill(int timeLeft)
		{
			if (Terraria.Main.netMode != NetmodeID.Server)
			{
				for (int d = 0; d < 5; d++)
				{
					Terraria.Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Gold, 0, 0, 1, colors[(int)NoteValue]);
				}
			}
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return (Color?)colors[(int)NoteValue];
		}
	}
}
