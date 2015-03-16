/*
 *	Copyright (C) 2015 Timothy A. Oltjenbruns
 *
 *	This program is free software; you can redistribute it and/or modify
 *	it under the terms of the GNU General Public License as published by
 *	the Free Software Foundation; either version 2 of the License, or
 *	(at your option) any later version.
 *	
 *	This program is distributed in the hope that it will be useful,
 *	but WITHOUT ANY WARRANTY; without even the implied warranty of
 *	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *	GNU General Public License for more details.
 *	
 *	You should have received a copy of the GNU General Public License along
 *	with this program; if not, write to the Free Software Foundation, Inc.,
 *	51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace TOltjenbruns.MassGame {
	public class Enemy {
		
		#region Protected Fields
		
		protected byte polarity;
		
		protected Vector3 velocity;
		protected Vector3 target;
		protected Vector3 avoidVector;
		
		protected Player player;
		protected HashSet<Particle> particles;
		protected HashSet<Enemy> enemies;
		#endregion
		
		#region Private Fields
		private readonly Emitter emitter;
		private const float power = 1000;
		private const float sustain = 0.99f;
		private const float field = 50;
		
		private readonly Emitter gunEmitter;
		private const float gunPower = 1000;
		private const float gunSustain = 0.75f;
		private const float gunField = 50;
		
		private Element element;
		//new player objects should always completely buffer the element on first update
		//TODO: move update polling to Element
		private bool updateTransform = true;
		private bool updateColor = true;
		
		private float gunCooldown = 5;
		
		private float maxSpd;
		
		#endregion
		
		#region Properties
		private Vector3 position;
		public Vector3 Position {
			get {return position;}
			set {
				updateTransform = true;
				position = value;
			}
		}
		
		private double rotation;
		public double Rotation {
			get {return rotation;}
			set {
				updateTransform = true;
				rotation = value;
			}
		}
		
		private float health;
		public float Health {
			get {return health;}
		}
		#endregion
		
		#region Constructors
		public Enemy (Player player, HashSet<Particle> particles, HashSet<Enemy> enemies)
			: this (player, particles, enemies, new Rgba(255, 0, 0, 255)){
		}
		
		public Enemy(Player player, HashSet<Particle> particles, HashSet<Enemy> enemies, Rgba colorMask){
			this.enemies = enemies;
			this.particles = particles;
			this.player = player;
			
			element = new Element(Player.playerPoly);
			element.LineWidth = 4;
			element.ColorMask = colorMask;
			
			emitter = new Emitter(power, sustain);
			gunEmitter = new Emitter(gunPower, gunSustain);
			
			polarity = 1;
			health = 20;
			position = Vector3.Zero;
			rotation = 0.0;
		}
		#endregion
		
		#region Original Methods
		public virtual void preUpdate (float delta){
			maxSpd = 100*delta;
			gunCooldown -= delta;
			
			float targetDist = 0;
			AvoidPlusTarget(position, player, enemies, ref target, ref avoidVector, ref targetDist);
		}
		
		public virtual void update (float delta){
			preUpdate(delta);
			Move(delta);
			Fire(delta);
			
//			Vector3 diff = player.Position - position;
//			float distance = diff.Length();
//			if (distance > 150){
//				Vector3 velocity = diff.Normalize();
//				velocity = velocity.Multiply(120 * delta);
//				Position += velocity;
//			}
//			else if (distance < 50){
//				Vector3 velocity = diff.Normalize();
//				velocity = velocity.Multiply(-120 * delta);
//				Position += velocity;
//			}
			
//			if (gunCooldown <= 0){
//				gunCooldown	= 5;
//				Vector3 aim = diff.Normalize();
//				aim = aim.Multiply(gunPower * delta);
//				foreach (Particle p in particles)
//					if ((p.Position - position).Length() < gunField){
//						p.Polarity = 1;
//						p.applyForce(aim, gunEmitter);
//					}
//			}
//			else 
//				foreach (Particle p in particles)
//					switch(p.Polarity){
//						case 0:
//							p.attract (position, emitter, field, delta);
//							break;
//						case 1:
//							p.repel (position, emitter, field, delta);
//							break;
//						case 2:
//							Vector3 partDiff = p.Position - position;
//							if (partDiff.Length() < 20){
//								takeDamage (1);
//								p.Polarity = 0;
//							}
//							break;
//					}
			
			Position += velocity;
			loopScreen();
			if (updateTransform) {
				updateTransform = false;
				//TODO: Fix element center
				element.Position = position.Multiply(0.01f).Add(new Vector3(-0.125f, -0.125f, 0));
				element.Rotation = rotation;
				element.updateTransBuffer();
			}
			if (updateColor) {
				updateColor = false;
				element.updateColorBuffer();
			}
		}
		
		public virtual void Move (float delta){
			Vector3 acceleration = target.Multiply(1f/500) + avoidVector.Multiply(1f/50);
			acceleration *= delta;
			
			if (acceleration.LengthSquared() > delta*delta) {
				acceleration = acceleration.Normalize();
				acceleration.Multiply(delta);
			}
			//Console.WriteLine(avoidVector);
			velocity += acceleration;
			
			//limits the velocity
			float maxSpdSq = maxSpd*maxSpd;
			if (velocity.LengthSquared() > maxSpdSq) {
				velocity = velocity.Normalize();
				velocity *= maxSpd;
			}
		}
		
		public virtual void Fire (float delta){
			gunCooldown -= delta;
			if (gunCooldown <= 0){
				gunCooldown	= 5;
				Vector3 aim = target.Normalize();
				aim = aim.Multiply(gunPower * delta);
				float gunFieldSq = gunField*gunField;
				foreach (Particle p in particles)
					if ((p.Position - position).LengthSquared() < gunFieldSq){
						p.Polarity = polarity;
						p.applyForce(aim, gunEmitter);
					}
			}
			else 
				foreach (Particle p in particles)
					switch(p.Polarity){
						case 0:
							p.attract (position, emitter, field, delta);
							break;
						case 1:
							p.repel (position, emitter, field, delta);
							break;
						default:
							Vector3 partDiff = p.Position - position;
							if (partDiff.LengthSquared() < 400){
								takeDamage (1);
								p.Polarity = 0;
							}
							break;
					}
		}
		
		public virtual void takeDamage(float damage){
			//TODO: Subtract HP
			//TODO: Change Color
			//TODO: Check if dead
		}
		
		public void render (GraphicsContext graphics){
			element.draw(graphics);
		}
		
		public void dispose(){
			element.dispose();	
		}
		#endregion
		
		#region AI Methods
		public void AvoidPlusTarget (Vector3 pos, Player player, HashSet<Enemy> enemies,
		                             ref Vector3 target, ref Vector3 avoidVector, ref float targetDist){
			//not sure if we want to avoid the oppposite polar particles as well
			Vector3 _avoidVector = Vector3.Zero;
			Vector3 playerOffsetPos = Offset(player.Position,pos);
			float closest = playerOffsetPos.LengthSquared();
			Vector3 _target = playerOffsetPos;
			int neighborCount = 0;
			int threshold = 10000;// 100 pixels squared
			//closest is the distance to player here \/
			if (closest < threshold) {
				neighborCount++;
				Vector3 norm = playerOffsetPos.Normalize();
				_avoidVector -= norm.Multiply(FMath.Sqrt(threshold-closest));
			}
			foreach(Enemy e in enemies){
				if (this != e) {
					Vector3 offset = Offset(e.position,pos);
					float dist = offset.LengthSquared();
					if (dist < closest) {
						closest = dist;
						_target = offset;
					}
					if (dist < threshold ) {
						neighborCount++;
						Vector3 norm_ = offset.Normalize();
						_avoidVector -= norm_.Multiply(FMath.Sqrt(threshold-dist));
					}
				}
			}
			target = _target;
			avoidVector = _avoidVector;
			targetDist = FMath.Sqrt(closest);
		}
		
		/// <summary>
		/// Returns the Vector3 offsetPoint relative to perspectivePoint.
		/// </summary>
		/// <param name='offsetPoint'>
		/// Offset point, the target.
		/// </param>
		/// <param name='perspectivePoint'>
		/// Perspective point, the center of the parspective.
		/// </param>
		public Vector3 Offset (Vector3 offsetPoint, Vector3 perspectivePoint){
			Vector3 diff = offsetPoint-perspectivePoint;
			//assuming a 400x400 loop screen.
			if (diff.X > 200) 
				diff.X -= 400;
			if (diff.X < -200)
				diff.X += 400;
			if (diff.Y > 200) 
				diff.Y -= 400;
			if (diff.Y < -200)
				diff.Y += 400;
			return diff;
		}
		
		public void loopScreen ()
		{
			if (position.X > 200) {
				position.X -= 400;
				updateTransform = true;
			}
			if (position.X <= -200) {
				position.X += 400;
				updateTransform = true;
			}
			if (position.Y > 200) {
				position.Y -= 400;
				updateTransform = true;
			}
			if (position.Y <= -200) {
				position.Y += 400;
				updateTransform = true;
			}
		}
		#endregion
	}
}

