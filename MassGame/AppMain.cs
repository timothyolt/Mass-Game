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
	    private static GraphicsContext graphics;
	    private static ShaderProgram shader;
		private static Random r;
		
		private static Player player;
		private static HashSet<Particle> particles;
		private static HashSet<Enemy> enemies;
	
	    static void Main(string[] args) {
			Stopwatch s = new Stopwatch();
			s.Start();
	        Init();
	        while (running) {
	            SystemEvents.CheckEvents();
				float delta = s.ElapsedMilliseconds / 1000f;
				Console.WriteLine("FPS: " + (int)(1/delta));
				s.Reset();
				s.Start();
	            Update(delta);
	            Render();
	        }
	        Dispose();
	    }
	
	    static bool Init() {
	        graphics = new GraphicsContext();
	        shader = new ShaderProgram("/Application/shaders/Primitive.cgx");
			r = new Random();
			particles = new HashSet<Particle>();
			enemies = new HashSet<Enemy>();
				
	        shader.SetUniformBinding(0, "WorldViewProj");
	        shader.SetAttributeBinding(0, "iPosition");
	        shader.SetAttributeBinding(1, "iColor");
			
			player = new Player(particles);
			for (int i = 0; i < 100; i++){
				Particle particle = new CubeParticle(Player.playerPoly, player, particles);
				particle.Position = new Vector3((float)(r.NextDouble() * 400) - 200, (float)(r.NextDouble() * 400) - 200, 0f);
				particles.Add (particle);
			}
			for (int i = 0; i < 3; i++){
				Enemy e = new Enemy(player, particles);
				e.Position = new Vector3((float)(r.NextDouble() * 400) - 200, (float)(r.NextDouble() * 400) - 200, 0f);
				enemies.Add(e);
			}
			
	        return true;
	    }

	    static void Dispose() {
	        shader.Dispose();
	        graphics.Dispose();
			player.dispose();
			foreach (Particle p in particles) p.dispose();
			foreach (Enemy e in enemies) e.dispose();
	    }
	
	    static bool Update(float delta) {
			//TODO: ADD STUFF
			GamePadData gamePadData = GamePad.GetData(0);
			player.update(delta, gamePadData);
			foreach (Particle p in particles) p.update(delta);
			foreach (Enemy e in enemies) e.update(delta);
	        return true;
	    }
	
	    static bool Render() {
		    float aspect = graphics.Screen.AspectRatio;
	        float fov = FMath.Radians(45.0f);
			//TODO: convert to orthographic camera maybe?
			//Matrix4 proj = Matrix4.Ortho(0f, aspect, 1f, 0f, 1f, -1f);;
	        Matrix4 proj = Matrix4.Perspective(fov, aspect, 1.0f, 1000000.0f);
	        Matrix4 view = Matrix4.LookAt(new Vector3(0.0f, -2.5f, 3.0f),
	                                    new Vector3(0.0f, -0.5f, 0.0f),
	                                    Vector3.UnitY);
			//Matrix4 worldViewProj = proj;	
			Matrix4 worldViewProj = proj * view;	
			
	        shader.SetUniformValue(0, ref worldViewProj);
	
	        //graphics.SetViewport(0, 0, graphics.Screen.Height, graphics.Screen.Height);
	        graphics.SetViewport(0, 0, graphics.Screen.Width, graphics.Screen.Height);
	        graphics.SetClearColor(0.2f, 0.2f, 0.2f, 1.0f);
	        graphics.Clear();
	
	        graphics.SetShaderProgram(shader);
			foreach (Particle p in particles) p.render(graphics);
			foreach (Enemy e in enemies) e.render(graphics);
			player.render(graphics);
	
	        graphics.SwapBuffers();
	
	        return true;
	    }
	}

} 