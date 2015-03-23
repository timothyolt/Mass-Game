 /*Copyright (C) 2015 Timothy A. Oltjenbruns
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
		private Emitter groupEmitter;
		private Emitter targetEmitter;
		#endregion
		
		#region Constructor
		public E_Cannon ()
			: this (new Rgba(255, 0, 255, 255)){
		}
		
		public E_Cannon (Rgba colorMask)
			: base (colorMask)
		{
			//TODO: initialize health
			Polarity = 3;
//			GunCooldown	= 5;
//			groupEmitter = new Emitter(500,0.4f,0,EmitterType.FORCE);
//			targetEmitter = new Emitter(300,0.4f,0,EmitterType.FORCE);
		}
		#endregion
		
		#region Original Methods
		public override void preUpdate (float delta)
		{
//			base.preUpdate (delta);
//			attract(Game.Player.Position,targetEmitter,200,delta);
//			foreach(Particle p in Game.Particles){
//				if(p!=this && p is E_Cannon){
//					attract(p.Position,groupEmitter,100,delta,false);
//				}
//			}
		}
		protected override void Move (float delta)
		{
//			base.Move (delta);
//			if(GunCooldown < 1){
//				EmitterSustain = 0.01f;
//			}else{
//				EmitterSustain = 0.99f;
//			}
		}
		
		#endregion
		
	}
}

