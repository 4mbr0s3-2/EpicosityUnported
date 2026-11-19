using Microsoft.Xna.Framework.Audio;
using Terraria.ModLoader;

namespace Epicosity.Sounds.Item.Weapons.Ranged
{
	public class MinecraftNoteHarp : ModSound
	{
		public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
		{
			soundInstance = sound.CreateInstance();
			soundInstance.Volume = volume;
			soundInstance.Pan = pan;
			return soundInstance;
		}
	}
}
