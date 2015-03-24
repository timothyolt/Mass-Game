// /*
//  *	Copyright (C) 2015 Timothy A. Oltjenbruns
//  *
//  *	This program is free software; you can redistribute it and/or modify
//  *	it under the terms of the GNU General Public License as published by
//  *	the Free Software Foundation; either version 2 of the License, or
//  *	(at your option) any later version.
//  *	
//  *	This program is distributed in the hope that it will be useful,
//  *	but WITHOUT ANY WARRANTY; without even the implied warranty of
//  *	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  *	GNU General Public License for more details.
//  *	
//  *	You should have received a copy of the GNU General Public License along
//  *	with this program; if not, write to the Free Software Foundation, Inc.,
//  *	51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
//  */
using System;

namespace TOltjenbruns.MassGame
{
	public class PickCannon : Particle
	{
		#region Private Fields
		private const float power = 1000;
		private const float sustain = 0.5f;
		private const float field = 35;
		#endregion
		
		#region Constructor
		public PickCannon () 
			:base(Player.playerPoly,new Emitter(power,sustain,field,0,EmitterType.FORCE))
		{
			
		}
		#endregion
		
		#region override
		public override void update (float delta)
		{
			base.update (delta);
		}
		
		public override void transform ()
		{
			
		}
		public override void color ()
		{
			
		}
		#endregion
	}
}

