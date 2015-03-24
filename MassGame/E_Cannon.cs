 /*Copyright (C) 2015 Timothy A. Oltjenbruns and Steffen Lim
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

namespace TOltjenbruns.MassGame{
	public class E_Cannon : Enemy
	{
		#region Private Fields
		
		private const float targetPower = 4f;
		private const float targetSustain = 0f;
		private const float targetField = 200;
		private Emitter targetEmitter;
		
		private const float seekPower = 1000f;
		private const float seekSustain = 0.2f;
		private const float seekField = 35;
		private Emitter seekEmitter;
		
		private E_Cannon neighbor = null;
		#endregion
		
		#region Constructor
		public E_Cannon ()
			: this (new Rgba(255, 0, 255, 255)){
		}
		
		public E_Cannon (Rgba colorMask)
			: base (colorMask)
		{
			targetEmitter = new Emitter(targetPower,targetSustain,targetField,Polarity,EmitterType.FORCE);
			seekEmitter = new Emitter(seekPower,seekSustain,seekField,Polarity,EmitterType.FORCE);
			foreach(Particle p in Game.Particles)
				//Lazy code, last one in array is the neighbor
				if (p is E_Cannon)
					neighbor = (E_Cannon) p;
//			groupEmitter = new Emitter(500,0.4f,0,EmitterType.FORCE);
		}
		#endregion
		
		#region Original Methods
		public override void preUpdate (float delta)
		{
			base.preUpdate (delta);
			if (neighbor != null){
				Vector3 aim = Position.LoopDiff(neighbor.Position);
				aim = aim.Normalize().Multiply(targetEmitter.power);
				applyForce(aim, targetEmitter);
			}
			else 
				foreach(Particle p in Game.Particles)
					if(p is CubeParticle)
						attract(p.Position,seekEmitter,delta,true);
//			attract(Game.Player.Position,targetEmitter,200,delta);
//			foreach(Particle p in Game.Particles){
//				if(p!=this && p is E_Cannon){
//					attract(p.Position,groupEmitter,100,delta,false);
//				}
//			}
		}
		protected override void Move (float delta)
		{
			base.Move(delta);
		}
		#endregion
		
	}
}

