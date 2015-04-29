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
	public class CannonMag : BaseMag {
		public static readonly Polygon cannonPoly = Polygon.Parse("/Application/polygons/CannonMag.poly");
		
		#region Private Fields
		
		private const float power = 1000;
		private const float sustain = 0.5f;
		private const float field = 35;
		private const float gunPower = 1000;
		private const float gunSustain = 0.75f;
		private const float gunField = 50;
		private readonly Emitter gunEmitter;
		private const float targetPower = 250f;
		private const float targetSustain = 0.01f;
		private const float targetField = 200;
		private Emitter targetEmitter;
		private const float seekPower = 10000000000f;
		private const float seekSustain = 0.8f;
		private const float seekField = 40;
		private Emitter seekEmitter;
		#endregion
		
		#region Constructor
		public CannonMag (byte polarity)
			: this (polarity, new Rgba(255, 0, 255, 255)) {
		}
		
		public CannonMag (byte polarity, Rgba colorMask)
			: base (cannonPoly, polarity, colorMask) {
			Polarity = (byte)Game.PolarityState.ENEMY;
			gunEmitter = new Emitter (gunPower, gunSustain, gunField, EmitterType.FORCE);
			targetEmitter = new Emitter (targetPower, targetSustain, targetField, EmitterType.FORCE);
			seekEmitter = new Emitter (seekPower, seekSustain, seekField, EmitterType.FORCE);
		}
		#endregion
		
		#region Original Methods

		protected override void Move (float delta) {
			foreach (BaseParticle p in Game.Particles)
				if ((p.Polarity != (byte)Game.PolarityState.NEUTRAL) || p is BitParticle)
					attract (p.Position, targetEmitter, delta, false);
				else 
					attract (p.Position, seekEmitter, delta, true);
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
					((BitParticle)p).fireCannon ();
					p.Polarity = Polarity;
					p.clearForces ();
					p.applyForce (aim, gunEmitter);
				}
			}
		}
		#endregion
		
	}
}

