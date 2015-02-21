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
using Sce.PlayStation.Core.Imaging;

namespace TOltjenbruns.MassGame{
	public class Element{
		//TODO: move update flagging to this class and out of player
		private const bool DEBUG = false;
		
		public Polygon[] polygons {get; private set;}
		public int[] vPolyIndex {get; private set;}
		public Primitive[] primitives {get; private set;}
		public VertexBuffer vertexBuffer {get; private set;}
		
		
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
		
		public float LineWidth {get; set;}
		
		//TODO: Center does NOT work as intended
		
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
			
		private void createBuffer(){
			int bufferLength = 0;
			for (int i = 0; i < polygons.Length; i++){
				vPolyIndex[i] = bufferLength;
				primitives[i] = new Primitive(
					polygons[i].DrawMode, bufferLength, polygons[i].vertexCount, 0);
				bufferLength += polygons[i].vertexCount;
			}
			
			//TODO: Check all poly's for indicies and generate sets for unindexed poly's
			if (polygons[0].Indicies != null){
				int indexCount = 0;
				int[] iPolyIndex = new int[polygons.Length];
				for (int i = 0; i < polygons.Length; i++){
					iPolyIndex[i] = indexCount;
					primitives[i].Count = (ushort) polygons[i].indexCount;
					indexCount += polygons[i].indexCount;
				}
				ushort[] indicies = new ushort[indexCount];
				for (int poly = 0; poly < polygons.Length; poly++)
					for (int i = 0; i < polygons[poly].indexCount; i++)
						indicies[iPolyIndex[poly] + i] = polygons[poly].Indicies[i];
				vertexBuffer = new VertexBuffer(
				bufferLength, indexCount, VertexFormat.Float3, VertexFormat.Float4);
				vertexBuffer.SetIndices(indicies);
			}
			else vertexBuffer = new VertexBuffer(
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
	}
}