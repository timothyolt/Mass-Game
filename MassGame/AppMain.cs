// Copyright (C) 2015 Timothy A. Oltjenbruns and Steffen Lim
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
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using TOltjenbruns.MassGame.Particle;
using TOltjenbruns.MassGame.Particle.Bit;
using TOltjenbruns.MassGame.Particle.Mag;

namespace TOltjenbruns.MassGame {

	class AppMain {
		
		private static bool running = true;
	
		private static void Main (string[] args) {
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
				foreach (BaseParticle p in Game.RemoveParticles)
					Game.Particles.Remove(p);
				Game.RemoveParticles.Clear();
				Render();
			}
			Dispose();
		}
	
		private static bool Init () {
			
			Game.Graphics = new GraphicsContext();
			Game.Shader = new ShaderProgram("/Application/shaders/Primitive.cgx");
			Game.Rand = new Random();
			Game.Particles = new HashSet<BaseParticle>();
			Game.RemoveParticles = new HashSet<BaseParticle>();
				
			Game.Shader.SetUniformBinding(0, "WorldViewProj");
			Game.Shader.SetAttributeBinding(0, "iPosition");
			Game.Shader.SetAttributeBinding(1, "iColor");
			
			Game.Player = new Player();
			Game.Particles.Add(Game.Player);
			bool blackHolePlaced = false;
			for (int i = 0; i < 200; i++) {
				BaseParticle particle = new ParticleBit();
				particle.Position = new Vector3(
					(float) (Game.Rand.NextDouble() * Game.SCREEN_WIDTH) - Game.SCREEN_WIDTH / 2, 
					(float) (Game.Rand.NextDouble() * Game.SCREEN_HEIGHT) - Game.SCREEN_HEIGHT / 2, 0f);
				Game.Particles.Add(particle);
			}
			for (int i = 0; i < 10; i++)
				switch (Game.Rand.Next(3)) {
				case 0:
					EnemyCannon e = new EnemyCannon();
					e.Position = new Vector3(
						(float) (Game.Rand.NextDouble() * Game.SCREEN_WIDTH) - Game.SCREEN_WIDTH / 2, 
						(float) (Game.Rand.NextDouble() * Game.SCREEN_HEIGHT) - Game.SCREEN_HEIGHT / 2, 0f);
					Game.Particles.Add(e);
					break;
				case 1:
				default:
					if (!blackHolePlaced) {
						blackHolePlaced = true;
						EnemyBlackHole e3 = new EnemyBlackHole();
						e3.Position = new Vector3(
							(float) (Game.Rand.NextDouble() * Game.SCREEN_WIDTH) - Game.SCREEN_WIDTH / 2, 
							(float) (Game.Rand.NextDouble() * Game.SCREEN_HEIGHT) - Game.SCREEN_HEIGHT / 2, 0f);
						Game.Particles.Add(e3);
						break;
					}
					else {
						EnemySpray e2 = new EnemySpray();
						e2.Position = new Vector3(
						(float) (Game.Rand.NextDouble() * Game.SCREEN_WIDTH) - Game.SCREEN_WIDTH / 2, 
						(float) (Game.Rand.NextDouble() * Game.SCREEN_HEIGHT) - Game.SCREEN_HEIGHT / 2, 0f);
						Game.Particles.Add(e2);
						break;
					
					}		
				}
			Game.PickupCannon = new PickupCannon(new Vector3(Game.Rand.Next(-200, 200), Game.Rand.Next(-200, 200), 0));
			Game.PickupBlackHole = new PickupBlackHole(new Vector3(Game.Rand.Next(-200, 200), Game.Rand.Next(-200, 200), 0));
			Game.groundPowerUps.Add(Game.PickupCannon);
			Game.groundPowerUps.Add(Game.PickupBlackHole);
			
			return true;
		}

		private static void Dispose () {
			Game.Dispose();
		}
	
		private static bool Update (float delta) {
			//TODO: ADD STUFF
			Game.GamePadData = GamePad.GetData(0);
			Game.Player.update(delta);
			foreach (BaseParticle p in Game.Particles)
				p.update(delta);
			foreach (BaseParticle p in Game.groundPowerUps)
				p.update(delta);
			return true;
		}
	
		private static bool Render () {
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
			foreach (BaseParticle p in Game.Particles)
				p.render();
			foreach (BaseParticle p in Game.groundPowerUps)
				p.render();
			Game.Player.render();
	
			Game.Graphics.SwapBuffers();
	
			return true;
		}
	}

} 