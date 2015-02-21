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

namespace Polarity{
	public class Element{
		//TODO: move update flagging to this class and out of player
		private const bool DEBUG = false;
		
		//TODO: YES I know, I probably broke encapsulation with my arrays. These were not meant to be changed by a client
		public Polygon[] polygons {get; private set;}
		public int[] vPolyIndex {get; private set;}
		public Primitive[] primitives {get; private set;}
		public VertexBuffer vertexBuffer {get; private set;}
		
		public double rotation {get; set;}
		public Rgba colorMaster {get; set;}
		
		public Vector3 position {get; set;}
		public Vector2 scale {get; set;}
		//TODO: Center does NOT work as intended
		public Vector2 center {get; set;}
		public float lineWidth {get; set;}
		
		public Element (Polygon polygon)
			: this(new Polygon[] {polygon}){}
		
		public Element (Polygon[] polygons){
			this.polygons = polygons;
			
			vPolyIndex = new int[polygons.Length];
			primitives = new Primitive[polygons.Length];
				
			createBuffer();
			
			rotation = 0.0;
			colorMaster = new Rgba(255, 255, 255, 255);
			
			position = Vector3.Zero;
			scale = Vector2.One;
			center = Vector2.Zero;
			lineWidth = 1;
			
			//updateTransBuffer();
			//updateColorBuffer();
		}
			
		private void createBuffer(){
		
			int bufferLength = 0;
			for (int i = 0; i < polygons.Length; i++){
				vPolyIndex[i] = bufferLength;
				primitives[i] = new Primitive(
					polygons[i].drawMode, bufferLength, polygons[i].vertexCount, 0);
				bufferLength += polygons[i].vertexCount;
			}
			
			//TODO: Check all poly's for indicies and generate sets for unindexed poly's
			if (polygons[0].indicies != null){
				int indexCount = 0;
				int[] iPolyIndex = new int[polygons.Length];
				for (int i = 0; i < polygons.Length; i++){
					iPolyIndex[i] = indexCount;
					primitives[i].Count = (ushort) polygons[i].indicies.Length;
					indexCount += polygons[i].indicies.Length;
				}
				ushort[] indicies = new ushort[indexCount];
				for (int poly = 0; poly < polygons.Length; poly++)
					for (int i = 0; i < polygons[poly].indicies.Length; i++)
						indicies[iPolyIndex[poly] + i] = polygons[poly].indicies[i];
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
			Vector3[] aVerts = (Vector3[]) polygons[pIndex].verticies.Clone();
			
			for (int vIndex = 0; vIndex < aVerts.Length; vIndex++){
				aVerts[vIndex] = calcRotation (polygons[pIndex], aVerts[vIndex]);
				aVerts[vIndex] = calcScale (polygons[pIndex], aVerts[vIndex]);
				aVerts[vIndex] += polygons[pIndex].position + position;
			}
			
			if (DEBUG) foreach (Vector3 v in aVerts)
			 Console.WriteLine("Vertex:" + v + ";");
			
			vertexBuffer.SetVertices(
				0, expandVerticies(aVerts), vPolyIndex[pIndex], 0, polygons[pIndex].vertexCount);
		}
		
		protected Vector3 calcRotation(Polygon poly, Vector3 vertex){
			float xPoly;
			float yPoly;
			
			if (poly.rotation != 0){
				double angle = Math.Atan2(
					vertex.Y - poly.center.Y, 
					vertex.X - poly.center.X);
				angle += poly.rotation;
				
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
		
		protected Vector3 calcScale(Polygon poly, Vector3 vertex){
			float xPoly;
			float yPoly;
			
			if (poly.scale != Vector2.One){
				xPoly = poly.scale.X * (vertex.X - poly.center.X);
				yPoly = poly.scale.Y * (vertex.Y - poly.center.Y);
			}
			else {
				xPoly = vertex.X;
				yPoly = vertex.Y;
			}
			
			if (scale != Vector2.One){
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
			Rgba[] aColors = (Rgba[]) polygons[pIndex].colors.Clone();
			for (int cIndex = 0; cIndex < polygons[pIndex].vertexCount; cIndex++){
				Vector4 pColor = polygons[pIndex].colorMaster.ToVector4();
				Vector4 eColor = colorMaster.ToVector4();
				aColors[cIndex] = new Rgba(aColors[cIndex].ToVector4() * pColor * eColor);
			}
			if (DEBUG) foreach (Rgba c in aColors)
			  	Console.WriteLine("Color:" + c.ToVector4() + ";");
			vertexBuffer.SetVertices(1, expandColors(aColors), vPolyIndex[pIndex], 0, polygons[pIndex].vertexCount);
		}
		
		public void draw(GraphicsContext graphics){
			graphics.SetVertexBuffer(0, vertexBuffer);
			graphics.SetLineWidth(lineWidth);
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