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

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Polarity{
	public struct Polygon{
		public Vector3[] verticies {get; private set;}
		public Rgba[] colors {get; private set;}
		public ushort[] indicies {get; private set;}
		public DrawMode drawMode {get; private set;}
		public int vertexCount {get {
			//if (indicies == null) return verticies.Length;
			//else return indicies.Length;}}
			return verticies.Length;}}
		
		public double rotation {get; set;}
		public Rgba colorMaster {get; set;}
		
		public Vector3 position {get; set;}
		public Vector2 scale {get; set;}
		public Vector2 center {get; set;}
		
		public Polygon (Vector3[] verticies, Rgba[] colors, ushort[] indicies, DrawMode drawMode,
		                double rotation, Rgba colorMaster,
		                Vector3 position, Vector2 scale, Vector2 center)
			: this(verticies, colors, indicies, drawMode, rotation, colorMaster){
			this.position = position;
			this.scale = scale;
			this.center = center;
		}
		
		public Polygon (Vector3[] verticies, Rgba[] colors, ushort[] indicies, DrawMode drawMode,
		                double rotation, Rgba colorMaster)
			: this(verticies, colors, indicies, drawMode){
			this.rotation = rotation;
			this.colorMaster = colorMaster;
		}
		
		public Polygon (Vector3[] verticies, Rgba[] colors, ushort[] indicies, DrawMode drawMode)
			: this(verticies, colors, drawMode){
			this.indicies = indicies;	
		}
		
		public Polygon (Vector3[] verticies, Rgba[] colors, DrawMode drawMode,
		                double rotation, Rgba colorMaster,
		                Vector3 position, Vector2 scale, Vector2 center)
			: this(verticies, colors, drawMode, rotation, colorMaster){
			this.position = position;
			this.scale = scale;
			this.center = center;
		}
		
		public Polygon (Vector3[] verticies, Rgba[] colors, DrawMode drawMode,
		                double rotation, Rgba colorMaster)
			: this(verticies, colors, drawMode){
			this.rotation = rotation;
			this.colorMaster = colorMaster;
		}
		
		public Polygon (Vector3[] verticies, Rgba[] colors, DrawMode drawMode)
			: this(){
			this.verticies = verticies;
			this.colors = colors;
			this.drawMode = drawMode;
		
			rotation = 0.0;
			colorMaster = new Rgba(255, 255, 255, 255);
			position = Vector3.Zero;
			scale = Vector2.One;
			center = Vector2.Zero;
		}
		
		public Polygon (Vector3[] verticies, Rgba[] colors)
			: this(verticies, colors, DrawMode.LineStrip) {}
		
		private static Rgba[] DefaultColors(int size){
			Rgba[] colors = new Rgba[size];
			for (int i=0; i < size; i++)
				colors[i] = new Rgba(255, 255, 255, 255);
			return colors;
		}
		
		public Polygon (Vector3[] verticies)
			: this(verticies, DefaultColors(verticies.Length)){}
		
	}
}