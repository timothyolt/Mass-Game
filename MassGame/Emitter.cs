using System;

namespace TOltjenbruns.MassGame {
	public enum EmitterType : byte {
		BIT, //small particles, governed by physics, no ai
		MAG, //large particles, ai or player controlled
		FORCE //forces from non representational objects (Eg, gravity, instantaneous forces)
	}
	
	//Physics object, should know everything about a game object that affects its physics
	public struct Emitter : IComparable{
		private static uint nextId = 0;
		
		public readonly uint id;
		public readonly float power;
		public readonly float sustain;
		public byte polarity;
		public readonly EmitterType etype;
		
		public Emitter(float power, float sustain, byte polarity, EmitterType ptype){
			id = nextId;
			nextId++;
			
			this.power = power;
			this.sustain = sustain;
			this.polarity = polarity;
			this.etype = ptype;
		}

		public int CompareTo (object obj){
			return ((Emitter) obj).id.CompareTo(id);
		}
	}
}

