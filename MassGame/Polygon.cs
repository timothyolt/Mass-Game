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
using System.Collections.ObjectModel;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace TOltjenbruns.MassGame {
	public struct Polygon{
		#region Readonly Properties
		private Vector3[] verticies;
		public ReadOnlyCollection<Vector3> Verticies {
			get {return Array.AsReadOnly<Vector3>(verticies);}
		}
		public int vertexCount {
			get {return verticies.Length;}
		}
		
		private Rgba[] colors;
		public ReadOnlyCollection<Rgba> Colors {
			get {return Array.AsReadOnly<Rgba>(colors);}
		}
		
		private ushort[] indicies;
		public ReadOnlyCollection<ushort> Indicies {
			get {return Array.AsReadOnly<ushort>(indicies);}
		}
		public int indexCount {
			get {return indicies.Length;}
		}
		
		public readonly DrawMode DrawMode;
		#endregion
		
		#region Properties
		private double rotation;
		private bool rotUpdate;
		public double Rotation {
			get {return rotation;}
			set {
				rotUpdate = true;
				rotation = value;
			}
		}
		
		private Rgba colorMask;
		private bool cMaskUpdate;
		public Rgba ColorMask {
			get {return colorMask;}
			set {
				cMaskUpdate = true;
				colorMask = value;
			}
		}
		
		private Vector3 position;
		private bool posUpdate;
		public Vector3 Position {
			get {return position;}
			set {
				posUpdate = true;
				position = value;
			}
		}
		
		private Vector3 scale;
		private bool scaleUpdate;
		public Vector3 Scale {
			get {return scale;}
			set {
				scaleUpdate = true;
				scale = value;
			}
		}
		
		private Vector3 center;
		private bool cenUpdate;
		public Vector3 Center {
			get {return center;}
			set {
				cenUpdate = true;
				center = value;
			}
		}
		#endregion
		
		#region Constructors
		public Polygon (Vector3[] verticies, Rgba[] colors, ushort[] indicies, DrawMode drawMode,
		                double rotation, Rgba colorMaster,
		                Vector3 position, Vector3 scale, Vector3 center)
			: this(verticies, colors, indicies, drawMode, rotation, colorMaster){
			this.position = position;
			this.scale = scale;
			this.center = center;
		}
		
		public Polygon (Vector3[] verticies, Rgba[] colors, ushort[] indicies, DrawMode drawMode,
		                double rotation, Rgba colorMaster)
			: this(verticies, colors, indicies, drawMode){
			this.rotation = rotation;
			this.colorMask = colorMaster;
		}
		
		public Polygon (Vector3[] verticies, Rgba[] colors, ushort[] indicies, DrawMode drawMode)
			: this(verticies, colors, drawMode){
			this.indicies = indicies;	
		}
		
		public Polygon (Vector3[] verticies, Rgba[] colors, DrawMode drawMode,
		                double rotation, Rgba colorMaster,
		                Vector3 position, Vector3 scale, Vector3 center)
			: this(verticies, colors, drawMode, rotation, colorMaster){
			this.position = position;
			this.scale = scale;
			this.center = center;
		}
		
		public Polygon (Vector3[] verticies, Rgba[] colors, DrawMode drawMode,
		                double rotation, Rgba colorMaster)
			: this(verticies, colors, drawMode){
			this.rotation = rotation;
			this.colorMask = colorMaster;
		}
		
		public Polygon (Vector3[] verticies, Rgba[] colors, DrawMode drawMode)
			: this(){
			this.verticies = verticies;
			this.colors = colors;
			this.DrawMode = drawMode;
		
			rotation = 0.0;
			colorMask = new Rgba(255, 255, 255, 255);
			position = Vector3.Zero;
			scale = Vector3.One;
			center = Vector3.Zero;
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
		#endregion
		
	}
}