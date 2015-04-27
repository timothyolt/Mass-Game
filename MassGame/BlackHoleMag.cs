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
	public class BlackHoleMag : BaseMag {
		private const float power = 1000;
		private const float sustain = 0.5f;
		private const float field = 35;
		private const float gunPower = 1000;
		private const float gunSustain = 0.75f;
		private const float gunField = 50;
		
		#region Constructor
		public BlackHoleMag (byte polarity)
			: this (polarity, new Rgba(0, 0, 0, 255)) {
		}
		
		public BlackHoleMag (byte polarity, Rgba colorMask)
			: base (polarity, colorMask) {
		}
		#endregion
		
		#region Original Methods
		protected override void Move (float delta) {
			foreach (BaseParticle p in Game.Particles) {
				if (p is BitParticle)//Quick and dirty way to avoid all particles (brownian motion)
					attract (p.Position, Emitter, delta, true);
			}
		}

		protected override void Fire (float delta) {
			gunCooldown = (float)(8 + (Game.Rand.NextDouble () * 7));
			Target = Position.LoopDiff (Game.Player.Position);
			Vector3 aim = Target.Normalize ();
			aim = aim.Multiply (gunPower * delta);
			foreach (BaseParticle p in Game.Particles) {
				if (
					p != this && 
					p.EmitterType == EmitterType.BIT && 
					Position.LoopDiff (p.Position).Length () <= gunField
				) {
					((BitParticle)p).fireBlackHole ();
					p.Polarity = Polarity;
					p.clearForces ();
				}
			}
		}
		
		#endregion
		
	}
}

