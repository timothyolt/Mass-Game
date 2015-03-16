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
using Sce.PlayStation.Core;

namespace TOltjenbruns.MassGame {
	public enum Color : ulong {
		TRANS = 0x00000000,
		BLACK = 0x000000FF,
		WHITE = 0xFFFFFFFF,
		RED = 0xFF0000FF,
		GREEN = 0x00FF00FF,
		BLUE = 0x0000FFFF
	}
	
	public static class ColorRgba {
		public static Rgba ToRgba(this Color c){
			ulong i = (ulong) c;
			Console.WriteLine("{0:X}", i);
			return new Rgba(
				(int)(i & 0xFF000000)/0x1000000, 
				(int)(i & 0x00FF0000)/0x10000,
				(int)(i & 0x0000FF00)/0x100,
				(int)(i & 0x000000FF)
			);
		}
	}
}

