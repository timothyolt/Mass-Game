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
using Sce.PlayStation.Core;

namespace TOltjenbruns.MassGame {
	public class SprayEnemy : BaseMag {
		#region Private Fields
		
		private const float targetPower = 0.1f;
		private const float targetSustain = 0.7f;
		private const float targetField = 200;
		private Emitter targetEmitter;
		#endregion
		
		#region Constructor
		public SprayEnemy ()
			: this (new Rgba(255, 0, 0, 255)) {
		}
		
		public SprayEnemy (Rgba colorMask)
			: base (colorMask) {
			//TODO: initialize health
			Polarity = 1;
			targetEmitter = new Emitter (targetPower, targetSustain, targetField, 2, EmitterType.FORCE);
			
			//TODO: empty
		}
		#endregion
		
		#region Original Methods
		public override void preUpdate (float delta) {
			base.preUpdate (delta);			
			Vector3 aim = Position.LoopDiff (Game.Player.Position);
			aim = aim.Normalize ().Multiply (targetEmitter.power);
			applyForce (aim, targetEmitter);
		}
		
		#endregion
		
	}
}

