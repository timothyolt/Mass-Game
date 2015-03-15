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
		
		private Player player;
		private HashSet<Particle> particles;
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
		public Enemy (Player player, HashSet<Particle> particles)
			: this (player, particles, new Rgba(255, 0, 0, 255)){
		}
		
		public Enemy(Player player, HashSet<Particle> particles, Rgba colorMask){
			this.particles = particles;
			this.player = player;
			
			element = new Element(Player.playerPoly);
			element.LineWidth = 4;
			element.ColorMask = colorMask;
			
			emitter = new Emitter(power, sustain);
			gunEmitter = new Emitter(gunPower, gunSustain);
			
			health = 20;
			position = Vector3.Zero;
			rotation = 0.0;
		}
		#endregion
		
		#region Original Methods
		public void update (float delta){
			Vector3 diff = player.Position - position;
			float distance = diff.Length();
			if (distance > 150){
				Vector3 velocity = diff.Normalize();
				velocity = velocity.Multiply(120 * delta);
				Position += velocity;
			}
			else if (distance < 50){
				Vector3 velocity = diff.Normalize();
				velocity = velocity.Multiply(-120 * delta);
				Position += velocity;
			}
			
			gunCooldown -= delta;
			if (gunCooldown <= 0){
				gunCooldown	= 5;
				Vector3 aim = diff.Normalize();
				aim = aim.Multiply(gunPower * delta);
				foreach (Particle p in particles)
					if ((p.Position - position).Length() < gunField){
						p.Polarity = 1;
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
						case 2:
							Vector3 partDiff = p.Position - position;
							if (partDiff.Length() < 20){
								takeDamage (1);
								p.Polarity = 0;
							}
							break;
					}
			
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
		
		public void takeDamage(float damage){
			
		}
		
		public void render (GraphicsContext graphics){
			element.draw(graphics);
		}
		
		public void dispose(){
			element.dispose();	
		}
		#endregion
	}
}

