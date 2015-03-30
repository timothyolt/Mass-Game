// Copyright (C) 2015 Timothy A. Oltjenbruns and Steffen Lim
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
using TOltjenbruns.MassGame.Particle.Bit;

namespace TOltjenbruns.MassGame.Particle.Mag {

	public class EnemyBlackHole : BaseEnemy {
		#region Private Fields
		private Emitter avoidEmitter;
		#endregion
		
		#region Constructors
		public EnemyBlackHole ()
			: this (new Rgba(0, 0, 0, 255)) {
		}
		
		public EnemyBlackHole (Rgba colorMask)
			: base (colorMask) {
		}
		#endregion
		
		#region Override Methods
		public override void preUpdate (float delta) {
			base.preUpdate(delta);
			foreach (BaseParticle p in Game.Particles) {
				if (p is ParticleBit)
					//Quick and dirty way to avoid all particles (brownian motion)
					attract(p.Position, Emitter, delta, true);
			}
		}
		
		#endregion
		
	}
}

