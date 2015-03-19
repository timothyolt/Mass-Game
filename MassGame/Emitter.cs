using System;

namespace TOltjenbruns.MassGame {
	public enum ParticleType : byte {
		BIT,
		MAG,
		FORCE
	}
	
	//Physics object, should know everything about a game object that affects its physics
	public struct Emitter : IComparable{
		private static uint nextId = 0;
		
		public readonly uint id;
		public readonly float power;
		public readonly float sustain;
		public byte polarity;
		public readonly ParticleType ptype;
		
		public Emitter(float power, float sustain, byte polarity, ParticleType ptype){
			id = nextId;
			nextId++;
			
			this.power = power;
			this.sustain = sustain;
			this.polarity = polarity;
			this.ptype = ptype;
		}

		public int CompareTo (object obj){
			return ((Emitter) obj).id.CompareTo(id);
		}
	}
}

