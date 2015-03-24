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
using System.Diagnostics;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;

namespace TOltjenbruns.MassGame {
	class AppMain {
	    private static bool running = true;
	
	    private static void Main(string[] args) {
			Stopwatch s = new Stopwatch();
			s.Start();
	        Init();
	        while (running) {
	            SystemEvents.CheckEvents();
				float delta = s.ElapsedMilliseconds / 1000f;
				//Console.WriteLine("FPS: " + (int)(1/delta));
				s.Reset();
				s.Start();
	            Update(delta);
	            Render();
	        }
	        Dispose();
	    }
	
	    private static bool Init() {
			//TODO: Test Colors
//			Console.WriteLine(Color.WHITE.ToRgba());
//			Console.WriteLine(Color.RED.ToRgba());
//			Console.WriteLine(Color.GREEN.ToRgba());
//			Console.WriteLine(Color.BLUE.ToRgba());
//			Console.WriteLine(Color.BLACK.ToRgba());
			
			
	        Game.Graphics = new GraphicsContext();
	        Game.Shader = new ShaderProgram("/Application/shaders/Primitive.cgx");
			Game.Rand = new Random();
			Game.Particles = new HashSet<Particle>();
				
	        Game.Shader.SetUniformBinding(0, "WorldViewProj");
	        Game.Shader.SetAttributeBinding(0, "iPosition");
	        Game.Shader.SetAttributeBinding(1, "iColor");
			
			Game.Player = new Player();
			Game.Particles.Add(Game.Player);
			for (int i = 0; i < 100; i++){
				Particle particle = new CubeParticle();
				particle.Position = new Vector3(
					(float)(Game.Rand.NextDouble() * Game.SCREEN_WIDTH) - Game.SCREEN_WIDTH/2, 
					(float)(Game.Rand.NextDouble() * Game.SCREEN_HEIGHT) - Game.SCREEN_HEIGHT/2, 0f);
				Game.Particles.Add (particle);
			}
			for (int i = 0; i < 5; i++){
				switch(0){/*/Game.Rand.Next(3)){*/
				case 0:
					E_Cannon e = new E_Cannon();
					e.Position = new Vector3(
						(float)(Game.Rand.NextDouble() * Game.SCREEN_WIDTH) - Game.SCREEN_WIDTH/2, 
						(float)(Game.Rand.NextDouble() * Game.SCREEN_HEIGHT) - Game.SCREEN_HEIGHT/2, 0f);
					Game.Particles.Add(e);
					break;
				case 1:
					E_BlackWhole e3 = new E_BlackWhole();
					e3.Position = new Vector3(
						(float)(Game.Rand.NextDouble() * Game.SCREEN_WIDTH) - Game.SCREEN_WIDTH/2, 
						(float)(Game.Rand.NextDouble() * Game.SCREEN_HEIGHT) - Game.SCREEN_HEIGHT/2, 0f);
					Game.Particles.Add(e3);
					break;
				default:
					E_Spray e2 = new E_Spray();
					e2.Position = new Vector3(
						(float)(Game.Rand.NextDouble() * Game.SCREEN_WIDTH) - Game.SCREEN_WIDTH/2, 
						(float)(Game.Rand.NextDouble() * Game.SCREEN_HEIGHT) - Game.SCREEN_HEIGHT/2, 0f);
					Game.Particles.Add(e2);
					break;
				}
			}
			
	        return true;
	    }

	    private static void Dispose() {
			Game.Dispose();
	    }
	
	    private static bool Update(float delta) {
			//TODO: ADD STUFF
			Game.GamePadData = GamePad.GetData(0);
			Game.Player.update(delta);
			foreach (Particle p in Game.Particles) p.update(delta);
	        return true;
	    }
	
	    private static bool Render() {
		    float aspect = Game.Graphics.Screen.AspectRatio;
	        float fov = FMath.Radians(45.0f);
			//TODO: convert to orthographic camera maybe?
			//Matrix4 proj = Matrix4.Ortho(0f, aspect, 1f, 0f, 1f, -1f);;
	        Matrix4 proj = Matrix4.Perspective(fov, aspect, 1.0f, 1000000.0f);
	        Matrix4 view = Matrix4.LookAt(/*new Vector3(0.0f, 0.0f, 5.0f),//*/new Vector3(0.0f, -2.5f, 3.0f),
	                                    /*new Vector3(0.0f, 0.0f, 0.0f),//*/new Vector3(0.0f, -0.50f, 0.0f),
	                                    Vector3.UnitY);
			//Matrix4 worldViewProj = proj;	
			Matrix4 worldViewProj = proj * view;	
			
	        Game.Shader.SetUniformValue(0, ref worldViewProj);
	
	        //graphics.SetViewport(0, 0, graphics.Screen.Height, graphics.Screen.Height);
	        Game.Graphics.SetViewport(0, 0, Game.Graphics.Screen.Width, Game.Graphics.Screen.Height);
	        Game.Graphics.SetClearColor(0.2f, 0.2f, 0.2f, 1.0f);
	        Game.Graphics.Clear();
	
	        Game.Graphics.SetShaderProgram(Game.Shader);
			foreach (Particle p in Game.Particles) p.render();
			Game.Player.render();
	
	        Game.Graphics.SwapBuffers();
	
	        return true;
	    }
	}

} 