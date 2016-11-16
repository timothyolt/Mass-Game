// Copyright (C) 2015 Timothy A. Oltjenbruns
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
using System;

namespace TOltjenbruns.MassGame {
	public enum EmitterType : byte {
		BIT, //small particles, governed by physics, no ai
		MAG, //large particles, ai or player controlled
		FORCE //forces from non representational objects (Eg, gravity, instantaneous forces)
	}
	
	//Physics object, should know everything about a game object that affects its physics
	public struct Emitter : IComparable {
		private static uint nextId = 0;
		public readonly uint id;
		public readonly float power;
		public readonly float sustain;
		public readonly float field;
		public readonly EmitterType etype;
		
		public Emitter (float power, float sustain, float field, EmitterType ptype) {
			id = nextId;
			nextId++;
			
			this.power = power;
			this.sustain = sustain;
			this.field = field;
			this.etype = ptype;
		}

		public int CompareTo (object obj) {
			return ((Emitter)obj).id.CompareTo (id);
		}
	}
}

