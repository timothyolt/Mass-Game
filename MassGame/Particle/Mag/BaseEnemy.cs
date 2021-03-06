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
using Sce.PlayStation.Core.Graphics;
using TOltjenbruns.MassGame.Particle.Bit;

namespace TOltjenbruns.MassGame.Particle.Mag {

	public class BaseEnemy : BaseParticle {
		
		#region Private Fields
		private const float power = 1000;
		private const float sustain = 0.5f;
		private const float field = 35;
		private readonly Emitter gunEmitter;
		private const float gunPower = 1000;
		private const float gunSustain = 0.75f;
		private const float gunField = 50;
		
		//new player objects should always completely buffer the element on first update
		//TODO: move update polling to Element
		
		private float gunCooldown = (float) (3 + (Game.Rand.NextDouble() * 4));
		private Vector3 target;
		#endregion
		
		#region Properties
		private float healthMax;
		private float health;

		public float Health {
			get { return health;}
		}
		#endregion
		
		#region Constructors
		public BaseEnemy ()
			: this (new Rgba(255, 0, 0, 255)) {
		}
		
		public BaseEnemy (Rgba colorMask) 
			: base (Player.playerPoly, new Emitter(power, sustain, field, 1, EmitterType.MAG)) {
			ColorMask = colorMask;
			gunEmitter = new Emitter(gunPower, gunSustain, gunField, 1, EmitterType.FORCE);
			Element.LineWidth = 4;
			health = 10;
			healthMax = 10;
		}
		#endregion
		
		#region Original Methods
		public virtual void preUpdate (float delta) {
			gunCooldown -= delta;
		}
		
		protected virtual void Move (float delta) {
			foreach (BaseParticle p in Game.Particles) {
				if (p.EmitterType == EmitterType.MAG) {
					//Vector3 diff = Position.LoopDiff(p.Position);
					//diff = diff.Normalize();
					//diff /= diff.LengthSquared();
					//diff *= delta;
					//p.attract(Position, Emitter, field, delta);
				}
			}
		}
		
		protected virtual void Fire (float delta) {
			if (Polarity != 3)
				gunCooldown = (float) (3 + (Game.Rand.NextDouble() * 4));
			else
				gunCooldown = 5;
			target = Position.LoopDiff(Game.Player.Position);
			gunCooldown = (float) (3 + (Game.Rand.NextDouble() * 4));
			Vector3 aim = target.Normalize();
			aim = aim.Multiply(gunPower * delta);
			//TODO: we are doing a lot of duplicate distance tests, lets make a HashSet<Particle,DistDiffPair>
			//and a private subclass DistDiffPair which holds a public vector3 (the diff) and a public float (distance)
			//Putting this in particle and making a method to populate the set each tick would be a good idea
			//Dont worry about clearing it, there can only be one data stored under a key (a particle in this case)
			//Last update's distances will be overriten when the HashSet is repopulated
//				foreach (Particle p in Game.Particles)
//					if (
//				    	(p.EmitterType.Equals(EmitterType.MAG)) && 
//						(p.Position - Position).LengthSquared() < gunFieldSq
//					){
//						p.Polarity = Polarity;
//						p.applyForce(aim, gunEmitter);
//					}'
			foreach (BaseParticle p in Game.Particles) {
				if (
					p != this && 
					p.EmitterType == EmitterType.BIT && 
					Position.LoopDiff(p.Position).Length() <= gunField
				) {
					if (this is EnemyCannon)
						((ParticleBit) p).fireCannon();
					else if (this is EnemyBlackHole)
							((ParticleBit) p).fireBlackHole();
						else
							((ParticleBit) p).fireSpray();
					p.Polarity = Polarity;
					p.clearForces();
					p.applyForce(aim, gunEmitter);
				}
			
			}
		}
		
		protected virtual void polarize (float delta) {
			target = Position.LoopDiff(Game.Player.Position);
			foreach (BaseParticle p in Game.Particles) {
				if (p != this && p.EmitterType.Equals(EmitterType.BIT)) {
					p.attract(Position, Emitter, delta);
					if (p.Polarity != 0 && p.Polarity != Polarity) {
						Vector3 partDiff = Position.LoopDiff(p.Position);
						if (partDiff.LengthSquared() < 400) {
							takeDamage(1);
							p.Polarity = 0;
						}
					}
				}
			}
		}
		
		public virtual void takeDamage (float damage) {
			health -= damage;
			bool r = ColorMask.R > 0;
			bool g = ColorMask.G > 0;
			bool b = ColorMask.B > 0;
			int fade = (int) (256 * (health / healthMax));
			ColorMask = new Rgba(
				r ? fade : 0, 
				g ? fade : 0, 
				b ? fade : 0, 255);
			if (health <= 0)
				Game.RemoveParticles.Add(this);
		}
		#endregion
		
		#region Override Methods
		public override void update (float delta) {
			preUpdate(delta);
			Move(delta);
			if (gunCooldown <= 0)
				Fire(delta);
			polarize(delta);
			base.update(delta);
		}
		
		public override void transform () {
			Element.Position = Position.Multiply(0.01f).Add(new Vector3(-0.0625f, -0.0625f, 0));
			Element.Rotation = Rotation;
		}
		
		public override void color () {
			Element.ColorMask = ColorMask;
		}
		#endregion
	}
}

