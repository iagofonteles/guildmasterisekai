using System;
namespace Drafts.Tilemap3D.Generators.Paths {
	[Flags]
	public enum Neighbor { x = 1, z = 2, d = 4, }

	[Serializable]
	public class RuleConfig : RuleSetup {
		public Neighbor conn;
		public Neighbor free;
		public int frame;
		protected override int Conn => (int)conn < 0 ? 7 : (int)conn;
		protected override int Free => (int)free < 0 ? 7 : (int)free;
	}
}
