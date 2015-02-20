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
	public abstract class Particle {
		#region Private Fields
		private static Vector3 gravity = new Vector3(0, -30, 0);
		private static Emitter gEmit = new Emitter(30, 0.5f);

		//new particle objects should always completely buffer the element on first update
		//TODO: move update polling to Element
		private bool updateTransform = true;
		private bool updateColor = true;
		
		private SortedList<Emitter, Vector3> forces;
		#endregion
		
		#region Properties
		private Emitter emitter;
		protected Emitter Emitter {
			get {return emitter;}
		}
		
		private Element element;
		protected Element Element {
			get {return element;}
		}
		
		private byte polarity;
		protected bool polarityUpdate = true;
		public byte Polarity {
			get { return polarity; }
			set { 
				polarityUpdate = true;
				polarity = value;
			}
		}
		
		private HashSet<Particle> particles;
		protected HashSet<Particle> Particles{
			get { return particles; }
		}
		
//		private float volatilily;
//		public float Volatility {
//			get { return volatilily; }
//			set { 
//				volatilily = value;
//				if (
//			}
//		}
		
		private Rgba colorMask;
		public Rgba ColorMask {
			get { return colorMask; }
			set {
				colorMask = value;
				updateColor = true;
			}
		}
		
		private Vector3 position;
		public Vector3 Position {
			get {return position;}
			set {
				updateTransform = true;
				position = value;
			}
		}
		
		private Vector3 velocity;
		public Vector3 Velocity {
			get {return velocity;}	
		}
		
		private Vector3 acceleration;
		public Vector3 Acceleration {
			get {return acceleration;}	
		}
		
		private double rotation;
		public double Rotation {
			get {return rotation;}
			set {
				updateTransform = true;
				rotation = value;
			}
		}
		#endregion
		
		#region Constructors
		public Particle(Polygon poly, Emitter emitter, HashSet<Particle> particles){
			element = new Element(poly);
			this.emitter = emitter;
			this.particles = particles;
			
			forces = new SortedList<Emitter, Vector3>();
			
			position = Vector3.Zero;
			rotation = 0.0;
		}
		#endregion
		
		#region Original Methods
		public void attract(Vector3 pos, Emitter e, float netSize, float delta){
			Vector3 diff = Position + Velocity - pos;
			float power = e.power * delta;
			float diffLength = diff.Length();
			if (diffLength < netSize){
				Vector3 force = Vector3.Zero;
				if (diffLength > power)
					force += diff.Multiply(power/(diffLength*diffLength));
				else 
					force += diff.Multiply(1/power);
				//if (force.Length() > netSize)
				//	force = Vector3.Zero;
				applyForce(-force, e);
			}
		}
		
		public void repel(Vector3 pos, Emitter e, float netSize, float delta){
			Vector3 diff = Position + Velocity - pos;
			float power = e.power * delta;
			float diffLength = diff.Length();
			if (diffLength < netSize){
				Vector3 force = Vector3.Zero;
				if (diffLength > power)
					force += diff.Multiply(power/(diffLength*diffLength));
				else 
					force += diff.Multiply(1/power);
				//if (force.Length() > netSize)
				//	force = Vector3.Zero;
				applyForce(force, e);
			}
		}
		
//		public void attract(Vector3 pos, float power, float netSize){
//			Vector3 diff = pos - Position;
//			Vector3 force = Vector3.Zero;
//			if (diff.LengthSquared() < netSize){
//				if (diff.Length() > power)
//					force += diff.Multiply(power/diff.LengthSquared());
//				else 
//					force += diff.Divide(power);
//				if (force.LengthSquared() > netSize)
//					force = Vector3.Zero;
//			}
//			acceleration += force;
//		}
//		
//		public void repel(Vector3 pos, float power, float netSize){
//			Vector3 diff = pos - Position;
//			Vector3 force = Vector3.Zero;
//			if (diff.LengthSquared() < netSize){
//				if (diff.Length() > power)
//					force += diff.Multiply(power/diff.LengthSquared());
//				else 
//					force += diff.Multiply(power);
//				if (force.LengthSquared() > netSize)
//					force = Vector3.Zero;
//			}
//			acceleration -= force;
//		}
		
		public void applyForce(Vector3 f, Emitter e){
			Vector3 existingForce = Vector3.Zero;
			if (forces.ContainsKey(e))
				forces [e] += f;
			else 
				forces.Add (e, f);
		}
		
		public void render (GraphicsContext graphics){
			element.draw(graphics);
		}
		
		public void dispose(){
			element.dispose();	
		}
		#endregion
		
		#region Abstract Methods
		//TODO: move update flagging to element
		public abstract void transform();
		
		//TODO: move update flagging to element
		public abstract void color();
		
		public virtual void update (float delta){
			//applyForce(gravity.Multiply(delta), gEmit);
			
			int fCount = forces.Count;
			HashSet<Emitter> removeQueue = new HashSet<Emitter>();
			foreach (var pair in forces){
				Vector3 force = pair.Value;
				Emitter e = pair.Key;
				if (force.LengthSquared() < 0.01)
					removeQueue.Add (pair.Key);
				acceleration += force;
				forces[pair.Key] = force.Multiply(e.sustain);
			}
			foreach (Emitter e in removeQueue){
				forces.Remove (e);
			}
			velocity += acceleration;
			acceleration = Velocity.Multiply(-1/2f);
			Position += velocity;
			
			if (position.X < -200) position.X = 200;
			if (position.X > 200) position.X = -200;
			if (position.Y < -200) position.Y = 200;
			if (position.Y > 200) position.Y = -200;
			
			if (updateTransform) {
				updateTransform = false;
				transform ();
				element.updateTransBuffer();
			}
			
			if (updateColor) {
				updateColor = false;
				color ();
				element.updateColorBuffer();
			}
			
		}
		#endregion
	}
}

