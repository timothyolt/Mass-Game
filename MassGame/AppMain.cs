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

//TODO: fix enemy inheritance
//TODO: weapon cycle visibiliy
//TODO: game states:
//		intro
//		instruction
//		high score
//		game
//		high score set
//TODO: sound effects and background sound
//TODO: enemy spawning
//TODO: fire upon release for easier aiming (and visual feedback)
//TODO: particle inventory (might cause less derpy physics)
//		still render them, but physics only interacts with the holder until thrown
//TODO: external mesh files
//TODO: options
//		control style
//		difficulty
//TODO: clean up and comment
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace TOltjenbruns.MassGame {
	class AppMain {
		private static bool running = true;
	
		private static void Main (string[] args) {
			Stopwatch s = new Stopwatch ();
			s.Start ();
			Init ();
			while (running) {
				SystemEvents.CheckEvents ();
				float delta = s.ElapsedMilliseconds / 1000f;
				//Console.WriteLine("FPS: " + (int)(1/delta));
				Game.update (delta);
				s.Reset ();
				s.Start ();
			}
			Game.Dispose ();
		}
	
		private static bool Init () {
			//TODO: Test Colors
//			Console.WriteLine(Color.WHITE.ToRgba());
//			Console.WriteLine(Color.RED.ToRgba());
//			Console.WriteLine(Color.GREEN.ToRgba());
//			Console.WriteLine(Color.BLUE.ToRgba());
//			Console.WriteLine(Color.BLACK.ToRgba());
			
			
			Game.Graphics = new GraphicsContext ();
			Game.Shader = new ShaderProgram ("/Application/shaders/Primitive.cgx");
			Game.Rand = new Random ();
				
			Game.Shader.SetUniformBinding (0, "WorldViewProj");
			Game.Shader.SetAttributeBinding (0, "iPosition");
			Game.Shader.SetAttributeBinding (1, "iColor");
			
			Game.GameState = Game.EGameState.DEMO;
			return true;
		}
	}

} 