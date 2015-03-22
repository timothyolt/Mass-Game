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
	public class Enemy : Particle{
		
		#region Private Fields
		private static Random rand = new Random();
		private readonly Emitter boidEmitter;
		private const float power = 1000;
		private const float sustain = 0.99f;
		private const float field = 50;
		
		private readonly Emitter gunEmitter;
		private const float gunPower = 1000;
		private const float gunSustain = 0.75f;
		private const float gunField = 50;
		
		//new player objects should always completely buffer the element on first update
		//TODO: move update polling to Element
		private bool updateTransform = true;
		private bool updateColor = true;
		
		private float gunCooldown = (float)(3+(rand.NextDouble()*4));
		private Vector3 target;
		#endregion
		
		#region Properties
		//Speed property
		
		//target property
		
		//
		
		private float health;
		public float Health {
			get {return health;}
		}
		#endregion
		
		#region Constructors
		public Enemy ()
			: this (new Rgba(255, 0, 0, 255)){
		}
		
		public Enemy(Rgba colorMask) 
			: base (Player.playerPoly, new Emitter(power, sustain, 1, EmitterType.MAG)){
			ColorMask = colorMask;
			gunEmitter = new Emitter(gunPower, gunSustain, 1, EmitterType.FORCE);
			boidEmitter = new Emitter(500, 0.734f, 1, EmitterType.MAG);
			Element.LineWidth = 4;
			health = 20;
		}
		#endregion
		
		#region Original Methods
		public virtual void preUpdate (float delta){
			gunCooldown -= delta;
		}
		
		public override void update (float delta){
			preUpdate(delta);
			Move(delta);
			Fire(delta);
			
			base.update(delta);
		}
		
		public override void transform (){
			Element.Position = Position.Multiply(0.01f).Add(new Vector3(-0.0625f,-0.0625f,0));
			Element.Rotation = Rotation;
		}
		
		public override void color () {
			Element.ColorMask = ColorMask;
		}
		
		public virtual void Move (float delta){
			foreach (Particle p in Game.Particles){
				if (p.EmitterType == EmitterType.MAG) {
					attract(p.Position, boidEmitter, field, delta);
				}
			}
		}
		
		public virtual void Fire (float delta){
			target = Position.LoopDiff(Game.Player.Position);
			if (gunCooldown <= 0){
				gunCooldown	= (float)(3+(rand.NextDouble()*4));
				Vector3 aim = target.Normalize();
				aim = aim.Multiply(gunPower * delta);
				float gunFieldSq = gunField*gunField;
				//TODO: we are doing a lot of duplicate distance tests, lets make a HashSet<Particle,DistDiffPair>
				//and a private subclass DistDiffPair which holds a public vector3 (the diff) and a public float (distance)
				//Putting this in particle and making a method to populate the set each tick would be a good idea
				//Dont worry about clearing it, there can only be one data stored under a key (a particle in this case)
				//Last update's distances will be overriten when the HashSet is repopulated
				foreach (Particle p in Game.Particles)
					if (
				    	(!p.EmitterType.Equals(EmitterType.MAG)) && 
						(p.Position - Position).LengthSquared() < gunFieldSq
					){
						p.Polarity = Polarity;
						p.applyForce(aim, gunEmitter);
					}
			}
			//else
			//This loop is for attracting particles and taking damage, shouldn't that happen regardless of firing
			foreach (Particle p in Game.Particles){
				//to limit the force effects only to BIT types
				if(p.EmitterType == EmitterType.BIT){
					p.attract (Position, Emitter, field, delta);
					
					switch(p.Polarity){
						case 0:
						case 1:
							break;
						default:
							Vector3 partDiff = Position.LoopDiff(p.Position);
							if (partDiff.LengthSquared() < 400){
								takeDamage (1);
								p.Polarity = 0;
							}
							break;
					}
				}
			}
		}
		
		public virtual void takeDamage(float damage){
			//TODO: Subtract HP
			//TODO: Change Color
			//TODO: Check if dead
		}
		#endregion
		
		#region AI Methods
		/*
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
		*/
		#endregion
	}
}

