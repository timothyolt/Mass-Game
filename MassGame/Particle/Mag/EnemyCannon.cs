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

	public class EnemyCannon : BaseEnemy {
		#region Private Fields
		private const float targetPower = 16f;
		private const float targetSustain = 0.5f;
		private const float targetField = 200;
		private Emitter targetEmitter;
		private const float seekPower = 500f;
		private const float seekSustain = 0.2f;
		private const float seekField = 10;
		private Emitter seekEmitter;
		private EnemyCannon neighbor = null;
		#endregion
		
		#region Constructors
		public EnemyCannon ()
			: this (new Rgba(255, 0, 255, 255)) {
		}
		
		public EnemyCannon (Rgba colorMask)
			: base (colorMask) {
			Polarity = 3;
			targetEmitter = new Emitter(targetPower, targetSustain, targetField, Polarity, EmitterType.FORCE);
			seekEmitter = new Emitter(seekPower, seekSustain, seekField, Polarity, EmitterType.FORCE);
			foreach (BaseParticle p in Game.Particles)
				//Lazy code, last one in array is the neighbor
				if (p is EnemyCannon)
					neighbor = (EnemyCannon) p;
		}
		#endregion
		
		#region Override Methods
		public override void preUpdate (float delta) {
			base.preUpdate(delta);
			if (neighbor != null) {
				Vector3 aim = Position.LoopDiff(neighbor.Position);
				aim = aim.Normalize().Multiply(targetEmitter.power);
				applyForce(aim, targetEmitter);
			}
			else 
				foreach (BaseParticle p in Game.Particles)
					if (p is ParticleBit)
						attract(p.Position, seekEmitter, delta, true);
		}

		protected override void Move (float delta) {
			
		}
		
		public override void applyVelocity (float delta) {
			if (neighbor == null)
				if (Velocity.LengthSquared() > 64)
					Velocity = Velocity.Normalize().Multiply(8);
				else if (Velocity.LengthSquared() > 16)
						Velocity = Velocity.Normalize().Multiply(2);
			base.applyVelocity(delta);
		}
		#endregion
		
	}
}

