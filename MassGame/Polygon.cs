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
using System.Collections.ObjectModel;
using System.IO;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace TOltjenbruns.MassGame {
	public struct Polygon{
		#region Serializer Methods
		public static Polygon Parse (string file) {
			StreamReader sr = null;
			Polygon p;
			int line = 0;
			try {
				sr = new StreamReader (file);
				p = Polygon.Parse (sr, ref line);
			} catch (FileNotFoundException) {
				Console.WriteLine ("Polygon file " + file + " cannot be found");
				throw;
			} finally {
				if (sr != null)
					sr.Close ();
			}
			return p;
		}
		
		public static Polygon Parse (StreamReader sr, ref int line) {
			Vector3 transform = Vector3.Zero;
			double rotation = 0;
			Rgba mask = new Rgba (255, 255, 255, 255);
			Vector3 scale = new Vector3 (1, 1, 1);
			Vector3 origin = Vector3.Zero;
			DrawMode graphics = DrawMode.LineStrip;
			List<Vector3> verticies = new List<Vector3> ();
			List<Rgba> colors = new List<Rgba> ();
			List<ushort> indicies = new List<ushort> ();
			char type = ' ';
			while (!("trmsodwp".Contains (((char) sr.Peek ()).ToString()) || sr.EndOfStream)) {
				sr.Read (); //Read Out Whitespace
				char prime = (char)sr.Read ();
				if (!prime.Equals (' '))
					type = prime;
				sr.Read (); //Read Out Whitespace
				string unparsed = sr.ReadLine ();
				line++;
				if (unparsed == null)
					break;
				string[] data = unparsed.Split (' ');
				try {
					switch (type) {
					case 't':
						transform = new Vector3 (float.Parse (data [0]), float.Parse (data [1]), float.Parse (data [2]));
						break;
					case 'r':
						rotation = double.Parse (data [0]);
						break;
					case 'm':
						mask = new Rgba (new Vector4 (float.Parse (data [0]), float.Parse (data [1]), float.Parse (data [2]), float.Parse (data [3])));
						break;
					case 's':
						scale = new Vector3 (float.Parse (data [0]), float.Parse (data [1]), float.Parse (data [2]));
						break;
					case 'o':
						origin = new Vector3 (float.Parse (data [0]), float.Parse (data [1]), float.Parse (data [2]));
						break;
					case 'g':
						graphics = (DrawMode)Enum.Parse (typeof(DrawMode), data [0]);
						break;
					case 'v':
						verticies.Add (new Vector3 (float.Parse (data [0]), float.Parse (data [1]), float.Parse (data [2])));
						break;
					case 'c':
						colors.Add (new Rgba (new Vector4 (float.Parse (data [0]), float.Parse (data [1]), float.Parse (data [2]), float.Parse (data [3]))));
						break;
					case 'i':
						foreach (string s in data)
							indicies.Add (ushort.Parse (s));
						break;
					}
				} catch (FormatException e) {
					Exception e2 = new FormatException ("Unable to parse line " + line + " \"" + unparsed + "\" " + e.Message, e);
					e2.Data.Add (typeof(Polygon), line);
					throw e2;
				} catch (IndexOutOfRangeException e) {
					Exception e2 = new FormatException ("Missing parameter on line " + line + " " + e.Message, e);
					e2.Data.Add (typeof(Polygon), line);
					throw e2;
				}
			}
			Polygon p = new Polygon (verticies.ToArray (), colors.ToArray (), indicies.ToArray (), graphics, rotation, mask, transform, scale, origin);
			//Console.WriteLine(p.Equals())
			return p;
		}
		#endregion
		
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