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
using System.IO;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;

namespace TOltjenbruns.MassGame{
	public class Element{
		//TODO: move update flagging to this class and out of player
		private const bool DEBUG = false;
		
		#region Serializer Methods
		public static Element Parse (string file) {
			StreamReader sr = null;
			Element e = null;
			int line = 0;
			try {
				sr = new StreamReader (file);
				e = Parse (sr, ref line);
			} catch (FileNotFoundException) {
				Console.WriteLine ("Element file " + file + " cannot be found");
				throw;
			} finally {
				if (sr != null)
					sr.Close ();
			}
			return e;
		}
		
		public static Element Parse (StreamReader sr, ref int line) {
			Vector3 transform = Vector3.Zero;
			double rotation = 0;
			Rgba mask = new Rgba (255, 255, 255, 255);
			Vector3 scale = new Vector3 (1, 1, 1);
			Vector3 origin = Vector3.Zero;
			Vector2 dimensions = Vector2.Zero;
			float lineWidth = 1;
			List<Polygon> polygons = new List<Polygon> ();
			char type = ' ';
			while (!sr.EndOfStream) {
				char prime = (char)sr.Read ();
				if (!prime.Equals (' '))
					type = prime;
				sr.Read (); //Read Out Whitespace
				string unparsed = sr.ReadLine ();
				line++;
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
					case 'd':
						dimensions = new Vector2 (float.Parse (data [0]), float.Parse (data [1]));
						break;
					case 'w':
						lineWidth = float.Parse (data [0]);
						break;
					case 'p':
						polygons.Add (Polygon.Parse (sr, ref line));
						break;
					}
				} catch (FormatException e) {
					if ((e.Data.Contains (typeof(Polygon))) && (!e.Data [typeof(TextRender)].Equals (line))) {
						Exception e2 = new FormatException ("Unable to parse line " + line + " \"" + unparsed + "\" " + e.Message, e);
						e2.Data.Add (typeof(Element), line);
						throw e2;
					}
					else
						throw;
				} catch (IndexOutOfRangeException e) {
					if ((e.Data.Contains (typeof(Polygon))) && (!e.Data [typeof(TextRender)].Equals (line))) {
						Exception e2 = new FormatException ("Missing parameter on line " + line + " " + e.Message, e);
						e2.Data.Add (typeof(Element), line);
						throw e2;
					}
					else
						throw;
				}
			}
			Element el = new Element (polygons.ToArray () [0]);
			el.Position = transform;
			el.Rotation = rotation;
			el.ColorMask = mask;
			el.Scale = scale;
			el.Center = origin;
			el.LineWidth = lineWidth;
			el.Dimension = dimensions;
			return el;
		}
		#endregion
		
		#region Properties
		public Polygon[] polygons {get; private set;}
		public int[] vPolyIndex {get; private set;}
		public Primitive[] primitives {get; private set;}
		public VertexBuffer vertexBuffer {get; private set;}
		
		private double rotation;
		private double rotUpdate = 0;
		public double Rotation {
			get {return rotation;}
			set {rotUpdate = value;}
		}
		
		private Rgba colorMask;
		private Rgba cMaskUpdate = Color.WHITE.ToRgba();
		public Rgba ColorMask {
			get {return colorMask;}
			set {
				cMaskUpdate = value;
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
			get { return center;}
			set {
				cenUpdate = true;
				center = value;
			}
		}
		
		private Vector2 dimension;
		public Vector2 Dimension {
			get { return dimension;}
			set {dimension = value;}
		}
		
		public float LineWidth {get; set;}
		#endregion
		
		//TODO: Center does NOT work as intended
		
		#region Constructors
		public Element (Polygon polygon)
			: this(new Polygon[] {polygon}){}
		
		public Element (Polygon[] polygons){
			this.polygons = polygons;
			
			vPolyIndex = new int[polygons.Length];
			primitives = new Primitive[polygons.Length];
				
			createBuffer();
			
			rotation = 0.0;
			colorMask = new Rgba(255, 255, 255, 255);
			
			position = Vector3.Zero;
			scale = Vector3.One;
			center = Vector3.Zero;
			LineWidth = 1;
			
			//updateTransBuffer();
			//updateColorBuffer();
		}
		#endregion
			
		#region Original Methods
		private void createBuffer () {
			int bufferLength = 0;
			for (int i = 0; i < polygons.Length; i++) {
				vPolyIndex [i] = bufferLength;
				primitives [i] = new Primitive (
					polygons [i].DrawMode, bufferLength, polygons [i].vertexCount, 0);
				bufferLength += polygons [i].vertexCount;
			}
			
			//TODO: Check all poly's for indicies and generate sets for unindexed poly's
			if (polygons [0].Indicies != null) {
				int indexCount = 0;
				int[] iPolyIndex = new int[polygons.Length];
				for (int i = 0; i < polygons.Length; i++) {
					iPolyIndex [i] = indexCount;
					primitives [i].Count = (ushort)polygons [i].indexCount;
					indexCount += polygons [i].indexCount;
				}
				ushort[] indicies = new ushort[indexCount];
				for (int poly = 0; poly < polygons.Length; poly++)
					for (int i = 0; i < polygons[poly].indexCount; i++)
						indicies [iPolyIndex [poly] + i] = polygons [poly].Indicies [i];
				//handle graphicssystemexceptions caused by using with a bad graphics driver or using before the system is initialized
				vertexBuffer = new VertexBuffer (
				bufferLength, indexCount, VertexFormat.Float3, VertexFormat.Float4);
				vertexBuffer.SetIndices (indicies);
			}
			else
				vertexBuffer = new VertexBuffer (
				bufferLength, VertexFormat.Float3, VertexFormat.Float4);		
		}
		
		public void updateTransBuffer(){
			for (int i=0; i < polygons.Length; i++)
				updateTransBuffer (i);
		}
	
		public void updateTransBuffer(int pIndex){
			Vector3[] aVerts = new Vector3[polygons[pIndex].vertexCount];
			polygons[pIndex].Verticies.CopyTo(aVerts, 0);
			
			for (int vIndex = 0; vIndex < aVerts.Length; vIndex++){
				aVerts[vIndex] = calcRotation (polygons[pIndex], aVerts[vIndex]);
				aVerts[vIndex] = calcScale (polygons[pIndex], aVerts[vIndex]);
				//put position first
				aVerts[vIndex] += polygons[pIndex].Position + position;
			}
			
			if (DEBUG) foreach (Vector3 v in aVerts)
			 Console.WriteLine("Vertex:" + v + ";");
			
			vertexBuffer.SetVertices(
				0, expandVerticies(aVerts), vPolyIndex[pIndex], 0, polygons[pIndex].vertexCount);
		}
		
		//Will assume position has already been updated
		//uses center and position vectors to find center of rotation/scale
		private Vector3 calcRotation(Polygon poly, Vector3 vertex){
			float xPoly;
			float yPoly;
			
			if (poly.Rotation != 0){
				double angle = Math.Atan2(
					vertex.Y - poly.Center.Y, 
					vertex.X - poly.Center.X);
				angle += poly.Rotation;
				
				float magnitude = vertex.Length();
				xPoly = (float) (magnitude * Math.Cos(angle));
				yPoly = (float) (magnitude * Math.Sin(angle));
			}
			else {
				xPoly = vertex.X;
				yPoly = vertex.Y;
			}
			
			if (rotation != 0){
				double angle = Math.Atan2(
					yPoly - center.Y, 
					xPoly - center.X);
				angle += rotation;
				
				float magnitude = (float) Math.Sqrt(
					(xPoly * xPoly) +
					(yPoly * yPoly));
				return new Vector3(
					(float) (magnitude * Math.Cos(angle)),
					(float) (magnitude * Math.Sin(angle)),
					0);
			}
			else return new Vector3(xPoly, yPoly, 0);
		}
		//Will assume position has already been updated
		//uses center and position vectors to find center of rotation/scale
		private Vector3 calcScale(Polygon poly, Vector3 vertex){
			float xPoly;
			float yPoly;
			
			if (poly.Scale != Vector3.One){
				xPoly = poly.Scale.X * (vertex.X - poly.Center.X);
				yPoly = poly.Scale.Y * (vertex.Y - poly.Center.Y);
			}
			else {
				xPoly = vertex.X;
				yPoly = vertex.Y;
			}
			
			if (scale != Vector3.One){
				return new Vector3(
					scale.X * (xPoly - center.X),
					scale.Y * (yPoly - center.Y),
					0);
			}
			else return new Vector3(xPoly, yPoly, 0);
		}
		
		public void updateColorBuffer(){
			for (int i=0; i < polygons.Length; i++)
				updateColorBuffer (i);
		}
		
		public void updateColorBuffer(int pIndex){
			Rgba[] aColors = new Rgba[polygons[pIndex].vertexCount];
			polygons[pIndex].Colors.CopyTo(aColors, 0);
			for (int cIndex = 0; cIndex < polygons[pIndex].vertexCount; cIndex++){
				Vector4 pColor = polygons[pIndex].ColorMask.ToVector4();
				Vector4 eColor = colorMask.ToVector4();
				aColors[cIndex] = new Rgba(aColors[cIndex].ToVector4() * pColor * eColor);
			}
			if (DEBUG) foreach (Rgba c in aColors)
			  	Console.WriteLine("Color:" + c.ToVector4() + ";");
			vertexBuffer.SetVertices(1, expandColors(aColors), vPolyIndex[pIndex], 0, polygons[pIndex].vertexCount);
		}
		
		public void draw(GraphicsContext graphics){
			graphics.SetVertexBuffer(0, vertexBuffer);
			graphics.SetLineWidth(LineWidth);
			graphics.DrawArrays(primitives, 0, primitives.Length);
		}
		
		public void dispose(){
	 		vertexBuffer.Dispose();
		}
		#endregion
		
		#region Static Methods
		public static float[] expandVerticies(Vector3[] verticies){
			float[] output  = new float[verticies.Length * 3];
			for (int i = 0; i < verticies.Length; i++){
				output[(i * 3)] = verticies[i].X;
				output[(i * 3) + 1] = verticies[i].Y;
				output[(i * 3) + 2] = verticies[i].Z;
			}
			return output;
		}
		
		public static Vector3[] condenseVerticies(float[] verticies){
			if (verticies.Length % 3 != 0)
				throw new Exception("Invalid vertex component count");
			
			Vector3[] output = new Vector3[verticies.Length / 3];
			for (int i = 0; i < output.Length; i++){
				output[i] = new Vector3(
					verticies[(i * 3)],
					verticies[(i * 3) + 1],
					verticies[(i * 3) + 2]);
			}
			return output;
		}
		
		public static float[] expandColors(Rgba[] colors){
			float[] output  = new float[colors.Length * 4];
			for (int i = 0; i < colors.Length; i++){
				Vector4 colorVector = colors[i].ToVector4();
				output[(i * 4)] = colorVector.X;
				output[(i * 4) + 1] = colorVector.Y;
				output[(i * 4) + 2] = colorVector.Z;
				output[(i * 4) + 3] = colorVector.W;
			}
			return output;
		}
		
		public static Rgba[] condenseColors(float[] colors){
			if (colors.Length % 4 != 0)
				throw new Exception("Invalid color component count");
			
			Rgba[] output = new Rgba[colors.Length / 4];
			for (int i = 0; i < output.Length; i++){
				output[i] = new Rgba(new Vector4(
					colors[(i * 4)],
					colors[(i * 4) + 1],
					colors[(i * 4) + 2],
					colors[(i * 4) + 3]));
			}
			return output;
		}
		#endregion
	}
}