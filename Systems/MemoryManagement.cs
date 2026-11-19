using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epicosity.Systems.MemoryManagement
{
	/// <summary>
	/// 
	/// </summary>
	internal class LoadManagerAttribute : Attribute
	{
		public ManageType type;
		public ManageNetmode netMode;
		public enum ManageType
		{
			Load,
			Unload
		}
		[Flags]
		public enum ManageNetmode
		{
			Client = 1,
			Server = 2,
		}
		public LoadManagerAttribute(ManageType type, ManageNetmode netMode = ManageNetmode.Client | ManageNetmode.Server)
		{
			this.type = type;
			this.netMode = netMode;
		}
	}
}
