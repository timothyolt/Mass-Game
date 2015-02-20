using System;

namespace TOltjenbruns.MassGame {
	public struct Emitter : IComparable{
		private static uint nextId = 0;
		
		public readonly uint id;
		public readonly float power;
		public readonly float sustain;
		
		public Emitter(float power, float sustain){
			id = nextId;
			nextId++;
			
			this.power = power;
			this.sustain = sustain;
		}

		public int CompareTo (object obj){
			return ((Emitter) obj).id.CompareTo(id);
		}
	}
}

