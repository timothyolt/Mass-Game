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
	public class Player {
        #region PlayerPolygon
        private static readonly float[] verticies = {
        0.333f, 0.000f, 0f, //Vericies
        0.666f, 0.000f, 0f,
        0.166f, 0.289f, 0f,
        0.833f, 0.289f, 0f,
        0.333f, 0.577f, 0f,
        0.666f, 0.577f, 0f,

        0.500f, 0.289f, 0f, //Top Center

        0.666f, 0.000f, 0f, //Low Center
        0.166f, 0.289f, 0f,
        0.666f, 0.577f, 0f,
        0.500f, 0.289f, 0f,
        };
        private static readonly float[] colors = {

        180/256f, 180/256f, 180/256f, 1.0f,
        180/256f, 180/256f, 180/256f, 1.0f,
        180/256f, 180/256f, 180/256f, 1.0f,
        180/256f, 180/256f, 180/256f, 1.0f,
        180/256f, 180/256f, 180/256f, 1.0f,
        180/256f, 180/256f, 180/256f, 1.0f,

        256/256f, 256/256f, 256/256f, 1.0f,

        130/256f, 130/256f, 130/256f, 1.0f,
        130/256f, 130/256f, 130/256f, 1.0f,
        130/256f, 130/256f, 130/256f, 1.0f,
        75/256f,  75/256f,  75/256f, 1.0f,
        };
        private static readonly ushort[] indicies = {
        9, 10, 8, 10, 7, //Low Center
        0, 1, 3, 5, 4, 2, //Outline
        0, 6, 3, 6, 4 //Top Center
        };

        public static readonly Polygon playerPoly = new Polygon(
        Element.condenseVerticies(verticies), 
        Element.condenseColors(colors),
        indicies,
        DrawMode.LineStrip,
        3.14/6,
        new Rgba(255, 255, 255, 255),
        Vector3.Zero,
        new Vector3(0.25f, 0.25f, 0.25f),
			Vector3.Zero);
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
		public Player (HashSet<Particle> particles)
			: this (particles, new Rgba(0, 255, 0, 255)){
		}
		
		public Player(HashSet<Particle> particles, Rgba colorMask){
			this.particles = particles;
			
			element = new Element(playerPoly);
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
		public void update (float delta, GamePadData gamePad){
			Vector3 velocity = Vector3.Zero;
			if ((gamePad.Buttons & GamePadButtons.Up) != 0)
				velocity.Y += 1;
			if ((gamePad.Buttons & GamePadButtons.Down) != 0)
				velocity.Y -= 1;
			if ((gamePad.Buttons & GamePadButtons.Right) != 0)
				velocity.X += 1;
			if ((gamePad.Buttons & GamePadButtons.Left) != 0)
				velocity.X -= 1;
			if (velocity != Vector3.Zero){
				velocity = velocity.Normalize();
				velocity = velocity.Multiply(120 * delta);
				Position += velocity;
				loopScreen();
			}
			
			Vector3 aim = Vector3.Zero;
			if ((gamePad.Buttons & GamePadButtons.Triangle) != 0)
				aim.Y += 1;
			if ((gamePad.Buttons & GamePadButtons.Cross) != 0)
				aim.Y -= 1;
			if ((gamePad.Buttons & GamePadButtons.Circle) != 0)
				aim.X += 1;
			if ((gamePad.Buttons & GamePadButtons.Square) != 0)
				aim.X -= 1;
			if (aim != Vector3.Zero){
				aim = aim.Normalize();
				aim = aim.Multiply(gunPower * delta);
				foreach (Particle p in particles)
					if ((p.Position - position).Length() < gunField){
						p.Polarity = 2;
						p.applyForce(aim, gunEmitter);
					}
			}
			else 
				foreach (Particle p in particles)
					switch(p.Polarity){
						case 0:
							p.attract (position, emitter, field, delta);
							break;
						case 2:
							p.repel (position, emitter, field, delta);
							break;
						default:
							Vector3 diff = p.Position - position;
							if (diff.Length() < 20){
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
		
		public void render (){
			element.draw(Game.Graphics);
		}
		
		public void dispose(){
			element.dispose();	
		}
		#endregion
		
		#region additional meathods
		public void loopScreen ()
		{
			if (position.X > 200) {
				position.X -= 400;
			}
			if (position.X <= -200) {
				position.X += 400;
			}
			if (position.Y > 200) {
				position.Y -= 400;
			}
			if (position.Y <= -200) {
				position.Y += 400;
			}
		}
		#endregion
	}
}

