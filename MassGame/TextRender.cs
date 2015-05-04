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
using System.Collections.Generic;
using System.IO;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace TOltjenbruns.MassGame {
	public class TextRender {
		
		private static string defaultCharMap = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";

		private Element[] alphabet;
		private string charMap;

		public string CharMap {
			get {
				return this.charMap;
			}
		}		
		public TextRender (string file)
		 : this (file, defaultCharMap) {
		}
		
		public TextRender (string file, string charMap) 
		 : this (file, charMap, false){
		}
		
		public TextRender (string file, string charMap, bool strict) {
			string actualCharMap = "";
			List<Element> elementList = new List<Element> ();
			for (int i = 0; i < charMap.Length; i++) {
				StreamReader sr = null;
				int line = 0;
				try {
					sr = new StreamReader (file + '/' + charMap [i]);
					elementList.Add (Element.Parse (sr, ref line));
					Console.WriteLine ("[TextRender] " + charMap [i] + " parsed successfully,");
					actualCharMap += charMap [i];
				} catch (FileNotFoundException) {
					if (strict) {
						Console.WriteLine ("[TextRender] Character " + charMap [i] + " cannot be found");
						throw;
					}
					else
						Console.WriteLine ("[TextRender] Character " + charMap [i] + " cannot be found, removing from charmap");
				} catch (FormatException e) {
					if (((e.Data.Contains (typeof(Element))) || (e.Data.Contains (typeof(Element)))) && (e.Data [typeof(TextRender)].Equals (line)))
					if (strict) {
						Console.WriteLine ("[TextRender] " + e.Message + " in character " + charMap [i]);
						throw;
					}
					else 
						Console.WriteLine ("[TextRender] " + e.Message + " in character " + charMap [i] + ", removing from charmap");
					else
						throw;
				} finally {
					if (sr != null)
						sr.Close ();
				}
			}
			alphabet = elementList.ToArray();
			this.charMap = actualCharMap;
		}
		
		public void render (string text, Vector3 pos) {
			render (text, pos, 1);
		}
		
		public void render (string text, Vector3 pos, float scale) {
			render(text, pos, new Vector3(scale, scale, scale));
		}
		
		public void render (string text, Vector3 pos, Vector3 scale) {
			render (text, pos, scale, 1);
		}
		
		public void render (string text, Vector3 pos, Vector3 scale, float lineWidth) {
			render (text, pos, new Rgba(255, 255, 255, 255), scale, lineWidth);
		}
		
		public void render (string text, Vector3 pos, Rgba mask) {
			render (text, pos, mask, 1);
		}
		
		public void render (string text, Vector3 pos, Rgba mask, float scale) {
			render(text, pos, mask, new Vector3(scale, scale, scale));
		}
		
		public void render (string text, Vector3 pos, Rgba mask, Vector3 scale) {
			render (text, pos, mask, scale, 1);
		}
		
		public void render (string text, Vector3 pos, Rgba mask, Vector3 scale, float lineWidth) {
			//TODO: implement caching
			float cursor = 0;
			foreach (char c in text) {
				if (charMap.Contains (c.ToString ())) {
					int index = charMap.IndexOf (c);
					Element element = alphabet [index];
					
					Vector3 epos = element.Position;
					Rgba emask = element.ColorMask;
					Vector3 escale = element.Scale;
					float eLineWidth = element.LineWidth;
					
					element.Position = epos + pos + (Vector3.UnitX * cursor);
					element.ColorMask = new Rgba (emask.ToVector4 () * mask.ToVector4 ());
					element.Scale = escale * scale;
					element.LineWidth = eLineWidth * lineWidth;
					
					element.updateTransBuffer ();
					element.updateColorBuffer ();
					element.draw (Game.Graphics);
					
					element.Position = epos;
					element.ColorMask = emask;
					element.Scale = escale;
					element.LineWidth = eLineWidth;
					
					cursor += element.Dimension.X * element.Scale.X * scale.X;
				}
				else if (c == ' ') 
					cursor += 10 * alphabet[0].Scale.X * scale.X;
			}
		}
	}
}

