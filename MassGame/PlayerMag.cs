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
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace TOltjenbruns.MassGame {
	public class PlayerMag : BaseParticle {
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

        180 / 256f, 180 / 256f, 180 / 256f, 1.0f,
        180 / 256f, 180 / 256f, 180 / 256f, 1.0f,
        180 / 256f, 180 / 256f, 180 / 256f, 1.0f,
        180 / 256f, 180 / 256f, 180 / 256f, 1.0f,
        180 / 256f, 180 / 256f, 180 / 256f, 1.0f,
        180 / 256f, 180 / 256f, 180 / 256f, 1.0f,

        256 / 256f, 256 / 256f, 256 / 256f, 1.0f,

        130 / 256f, 130 / 256f, 130 / 256f, 1.0f,
        130 / 256f, 130 / 256f, 130 / 256f, 1.0f,
        130 / 256f, 130 / 256f, 130 / 256f, 1.0f,
        75 / 256f,  75 / 256f,  75 / 256f, 1.0f,
        };
		private static readonly ushort[] indicies = {
        9, 10, 8, 10, 7, //Low Center
        0, 1, 3, 5, 4, 2, //Outline
        0, 6, 3, 6, 4 //Top Center
        };
		public static readonly Polygon playerPoly = new Polygon (
        Element.condenseVerticies (verticies), 
        Element.condenseColors (colors),
        indicies,
        DrawMode.LineStrip,
        3.14 / 6,
        new Rgba (255, 255, 255, 255),
        Vector3.Zero,
        new Vector3 (0.25f, 0.25f, 0.25f),
			Vector3.Zero);
        #endregion
		
		#region Private Fields
		private const float power = 1000;
		private const float sustain = 0.75f;
		private const float field = 35;
		private const float gunPower = 2000;
		private const float gunSustain = 0.65f;
		private const float gunField = 35;
		private readonly Emitter gunEmitter;
		private const float compactPower = 2000;
		private const float compactSustain = 0.65f;
		private const float compactField = 35;
		private readonly Emitter compactEmitter;
		private const float sprayCooldownReset = 0.3f;
		private float sprayCooldown = 0;
		private const float chargeTimeReset = 0.25f;
		private float chargeTime = 0;
		private int fireType = 0;
		#endregion
		
		#region Properties
//		private Vector3 Position;
//		public Vector3 Position {
//			get {return Position;}
//			set {
//				updateTransform = true;
//				Position = value;
//			}
//		}
		
		private float healthMax;
		private float health;

		public float Health {
			get { return health;}
		}
		#endregion
		
		#region Constructors
		public PlayerMag (byte polarity)
			: this (polarity, new Rgba(0, 255, 0, 255)) {
		}
		
		public PlayerMag (byte polarity, Rgba colorMask) 
			: base (playerPoly, polarity, new Emitter(power, sustain, field, EmitterType.MAG)) {
			
			//element = new Element(playerPoly);
			Element.LineWidth = 4;
			Element.ColorMask = colorMask;
			
			gunEmitter = new Emitter (gunPower, gunSustain, gunField, EmitterType.FORCE);
			
			health = 300;
			healthMax = 300;
			Position = Vector3.Zero;
		}
		#endregion
		
		#region Original Methods
		public override void update (float delta) {
			GamePadData gamePad = Game.GamePadData;
			move (delta, gamePad);
			if (sprayCooldown <= 0)
				fire (delta, gamePad);
			else
				sprayCooldown -= delta;
			polarize (delta);
			pickUpPower ();
			base.update (delta);
			Console.WriteLine (health);
		}
		
		private void move (float delta, GamePadData gamePad) {
			Vector3 velocity = Vector3.Zero;
			if ((gamePad.Buttons & GamePadButtons.Up) != 0)
				velocity.Y += 1;
			if ((gamePad.Buttons & GamePadButtons.Down) != 0)
				velocity.Y -= 1;
			if ((gamePad.Buttons & GamePadButtons.Right) != 0)
				velocity.X += 1;
			if ((gamePad.Buttons & GamePadButtons.Left) != 0)
				velocity.X -= 1;
			if (velocity != Vector3.Zero) {
				velocity = velocity.Normalize ();
				velocity = velocity.Multiply (120 * delta);
				Position += velocity;
			}
		}
		
		private void fire (float delta, GamePadData gamePad) {
			Vector3 aim = Vector3.Zero;
			
			if ((gamePad.Buttons & GamePadButtons.L) != 0)
			if (Game.obtainedPowerUps.Contains (Game.CannonPickup))
				fireType = 1;
			if ((gamePad.Buttons & GamePadButtons.R) != 0)
			if (Game.obtainedPowerUps.Contains (Game.BlackHolePickup))
				fireType = 2;
			if ((gamePad.Buttons & GamePadButtons.Triangle) != 0)
				aim.Y += 1;
			if ((gamePad.Buttons & GamePadButtons.Cross) != 0)
				aim.Y -= 1;
			if ((gamePad.Buttons & GamePadButtons.Circle) != 0)
				aim.X += 1;
			if ((gamePad.Buttons & GamePadButtons.Square) != 0)
				aim.X -= 1;
			if (aim != Vector3.Zero) {
				aim = aim.Normalize ();
				aim = aim.Multiply (gunPower * delta);
				foreach (BaseParticle p in Game.Particles)
					if (p != this && 
						p.EmitterType == EmitterType.BIT && 
						Position.LoopDiff (p.Position).Length () <= gunField) {
						switch (fireType) {
						case 1:
							((BitParticle)p).fireCannon ();
							break;
						case 2:	
							((BitParticle)p).fireBlackHole ();
							break;
						default:
							((BitParticle)p).fireSpray ();
							break;
						}
						p.Polarity = Polarity;
						p.clearForces ();
						p.applyForce (aim, gunEmitter);
					}
				sprayCooldown = sprayCooldownReset;
				fireType = 0;
			}
		}
		
		private void polarize (float delta) {
			foreach (BaseParticle p in Game.Particles) {
				if (p != this && p.EmitterType.Equals (EmitterType.BIT)) {
					p.attract (Position, Emitter, Polarity, delta);
					if (p.Polarity != 0 && p.Polarity != Polarity) {
						Vector3 partDiff = Position.LoopDiff (p.Position);
						if (partDiff.LengthSquared () < 400) {
							takeDamage (1);
							p.Polarity = 0;
						}
					}
				}
			}
		}
		
		public void pickUpPower () {
			for (int i = 0; i < Game.pickups.Count; i++)
				if (Game.pickups [i].Position.LoopDiff (Position).LengthSquared () < field * field) {
					Game.obtainedPowerUps.Add (Game.pickups [i]);
					Game.pickups.RemoveAt (i);
				}
		}
		
		public virtual void takeDamage (float damage) {
			health -= damage;
			bool r = ColorMask.R > 0;
			bool g = ColorMask.G > 0;
			bool b = ColorMask.B > 0;
			int fade = (int)(256 * (health / healthMax));
			ColorMask = new Rgba (
				r ? fade : 0, 
				g ? fade : 0, 
				b ? fade : 0, 255);
			if (health <= 0)
				Game.RemoveParticles.Add (this);
		}
		#endregion
		
		#region Override Methods
		public override void transform () {
			//TODO: Fix element center
			Element.Position = Position.Multiply (0.01f).Add (new Vector3 (-0.125f, -0.125f, 0));
			Element.Rotation = Rotation;
		}

		public override void color () {
			
		}
		#endregion
	}
}

