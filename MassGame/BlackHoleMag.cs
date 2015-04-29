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
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace TOltjenbruns.MassGame {
	public class BlackHoleMag : BaseMag {
//		private static readonly float[] verticies = {
//			0, 5, 0,			//0
//			0, 15, 0,			//1
//			2.1651f, 6.25f, 0,	//2
//			2.1651f, 13.75f, 0,	//3
//			4.3301f, 7.5f, 0,		//4
//			4.3301f, 12.5f, 0,	//5
//			8.6603f, 0, 0,		//6
//			8.6603f, 2.5f, 0,		//7
//			8.6603f, 5, 0, 		//8
//			8.6603f, 10, 0,		//9
//			8.6603f, 15, 0,		//10
//			8.6603f, 17.5f, 0,	//11
//			8.6603f, 20, 0,		//12
//			12.9904f, 7.5f, 0,	//13
//			12.9904f, 12.5f, 0,	//14
//			15.1554f, 6.25f, 0,	//15
//			15.1554f, 13.75f, 0,	//16
//			17.3205f, 5, 0,		//17
//			17.3205f, 15, 0,		//18
//		};
//		
//		private static readonly float[] colors = {
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//			1, 1, 1, 1,
//		};
//		
//		private static readonly ushort[] indicies = {
//			2, 0, 3, 1, 11, 12, 16, 18, 15, 17, 7, 6, 2, //Outer edge
//			4, 5, 2, 5, 10, 14, 16, 14, 13, 8, 7, 8, 4, //Inner edge
//			9, 11, 9, 15, //Center
//		};
//		
//		public static readonly Polygon blackHolePoly = new Polygon (
//        Element.condenseVerticies (verticies), 
//        Element.condenseColors (colors),
//        indicies,
//        DrawMode.LineStrip,
//        3.14 / 6,
//        new Rgba (255, 255, 255, 255),
//        Vector3.Zero,
//        new Vector3 (0.02f, 0.02f, 0.02f),
//		Vector3.Zero);
		public static readonly Polygon blackHolePoly = Polygon.Parse("Application/polygons/BlackHoleMag.poly");
		
		private const float power = 1000;
		private const float sustain = 0.5f;
		private const float field = 35;
		private const float gunPower = 1000;
		private const float gunSustain = 0.75f;
		private const float gunField = 50;
		
		#region Constructor
		public BlackHoleMag (byte polarity)
			: this (polarity, new Rgba(0, 0, 0, 255)) {
		}
		
		public BlackHoleMag (byte polarity, Rgba colorMask)
			: base (blackHolePoly, polarity, colorMask) {
		}
		#endregion
		
		#region Original Methods
		protected override void Move (float delta) {
			foreach (BaseParticle p in Game.Particles) {
				if (p is BitParticle)//Quick and dirty way to avoid all particles (brownian motion)
					attract (p.Position, Emitter, delta, true);
			}
		}

		protected override void Fire (float delta) {
			gunCooldown = (float)(8 + (Game.Rand.NextDouble () * 7));
			Target = Position.LoopDiff (Game.Player.Position);
			Vector3 aim = Target.Normalize ();
			aim = aim.Multiply (gunPower * delta);
			foreach (BaseParticle p in Game.Particles) {
				if (
					p != this && 
					p.EmitterType == EmitterType.BIT && 
					Position.LoopDiff (p.Position).Length () <= gunField
				) {
					((BitParticle)p).fireBlackHole ();
					p.Polarity = Polarity;
					p.clearForces ();
				}
			}
		}
		
		#endregion
		
	}
}

