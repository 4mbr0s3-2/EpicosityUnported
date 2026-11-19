using Epicosity.Network;
using System;
using System.IO;

namespace Epicosity.Tiles
{
	internal class HealthChargerSyncHandler : PacketHandler
	{
		public HealthChargerSyncHandler(byte handlerType) : base(handlerType)
		{
		}

		public override void HandlePacket(BinaryReader reader, int fromWho)
		{
		}
	}
}
