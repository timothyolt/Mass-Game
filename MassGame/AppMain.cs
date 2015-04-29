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
//TODO: clean up
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.UI;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace TOltjenbruns.MassGame {
	class AppMain {
		private static bool running = true;
		
		public enum EGameState {
			INTRO,
			GAME,
			HS_ADD,
			HISCORE,
		}
		
		private static List<HiscoreEntry> hiscores;
		
		private static EGameState gameState;
		
		private static TextRender tr;

		public static EGameState GameState {
			get {
				return gameState;
			}
			set {
				gameState = value;
			}
		}	
	
		private static void Main (string[] args) {
			Stopwatch s = new Stopwatch ();
			s.Start ();
			Init ();
			while (running) {
				SystemEvents.CheckEvents ();
				float delta = s.ElapsedMilliseconds / 1000f;
				//Console.WriteLine("FPS: " + (int)(1/delta));
				s.Reset ();
				s.Start ();
				Update (delta);
				foreach (BaseParticle p in Game.RemoveParticles)
					Game.Particles.Remove (p);
				Game.RemoveParticles.Clear ();
				Render ();
			}
			Dispose ();
		}
	
		private static bool Init () {
			//TODO: Test Colors
//			Console.WriteLine(Color.WHITE.ToRgba());
//			Console.WriteLine(Color.RED.ToRgba());
//			Console.WriteLine(Color.GREEN.ToRgba());
//			Console.WriteLine(Color.BLUE.ToRgba());
//			Console.WriteLine(Color.BLACK.ToRgba());
			hiscores = new List<HiscoreEntry>();
			StreamReader sr;
			try {
				sr = new StreamReader ("/Documents/hiscore");
			} catch (FileNotFoundException) {
				StreamWriter sw = new StreamWriter ("Documents/hiscore");
				for (int i = 0; i < 5; i++)
					sw.WriteLine ("AAA " + (300 - (i * 100)));
				sw.Close ();
				sr = new StreamReader ("/Documents/hiscore");
			}
			while (!sr.EndOfStream) {
				string[] split = sr.ReadLine ().Split (' ');
				hiscores.Add (new HiscoreEntry (split [0], int.Parse (split [1])));
			}
			hiscores.Sort ();
			
			Game.Graphics = new GraphicsContext ();
			Game.Shader = new ShaderProgram ("/Application/shaders/Primitive.cgx");
			Game.Rand = new Random ();
			Game.Particles = new HashSet<BaseParticle> ();
			Game.RemoveParticles = new HashSet<BaseParticle> ();
				
			Game.Shader.SetUniformBinding (0, "WorldViewProj");
			Game.Shader.SetAttributeBinding (0, "iPosition");
			Game.Shader.SetAttributeBinding (1, "iColor");
			
			Game.Player = new PlayerMag ((byte)Game.PolarityState.PLAYER);
			Game.Particles.Add (Game.Player);
			bool blackHolePlaced = false;
			for (int i = 0; i < 200; i++) {
				BaseParticle particle = new BitParticle ((byte)Game.PolarityState.NEUTRAL);
				particle.Position = new Vector3 (
					(float)(Game.Rand.NextDouble () * Game.SCREEN_WIDTH) - Game.SCREEN_WIDTH / 2, 
					(float)(Game.Rand.NextDouble () * Game.SCREEN_HEIGHT) - Game.SCREEN_HEIGHT / 2, 0f);
				Game.Particles.Add (particle);
			}
			for (int i = 0; i < 2; i++)
				switch (0) {//Game.Rand.Next (3)) {
				case 0:
					CannonMag e = new CannonMag ((byte)Game.PolarityState.ENEMY);
					e.Position = new Vector3 (
						(float)(Game.Rand.NextDouble () * Game.SCREEN_WIDTH) - Game.SCREEN_WIDTH / 2, 
						(float)(Game.Rand.NextDouble () * Game.SCREEN_HEIGHT) - Game.SCREEN_HEIGHT / 2, 0f);
					Game.Particles.Add (e);
					break;
				case 1:
					if (!blackHolePlaced) {
						blackHolePlaced = true;
						BlackHoleMag e3 = new BlackHoleMag ((byte)Game.PolarityState.ENEMY);
						e3.Position = new Vector3 (
							(float)(Game.Rand.NextDouble () * Game.SCREEN_WIDTH) - Game.SCREEN_WIDTH / 2, 
							(float)(Game.Rand.NextDouble () * Game.SCREEN_HEIGHT) - Game.SCREEN_HEIGHT / 2, 0f);
						Game.Particles.Add (e3);
						break;
					}
					goto default;
				default:
					SprayEnemy e2 = new SprayEnemy (1);
					e2.Position = new Vector3 (
						(float)(Game.Rand.NextDouble () * Game.SCREEN_WIDTH) - Game.SCREEN_WIDTH / 2, 
						(float)(Game.Rand.NextDouble () * Game.SCREEN_HEIGHT) - Game.SCREEN_HEIGHT / 2, 0f);
					Game.Particles.Add (e2);
					break;
				}
			Game.CannonPickup = new CannonPickup ((byte)Game.PolarityState.NEUTRAL, new Vector3 (Game.Rand.Next (-200, 200), Game.Rand.Next (-200, 200), 0));
			Game.BlackHolePickup = new BlackHolePickup ((byte)Game.PolarityState.NEUTRAL, new Vector3 (Game.Rand.Next (-200, 200), Game.Rand.Next (-200, 200), 0));
			Game.pickups.Add (Game.CannonPickup);
			Game.pickups.Add (Game.BlackHolePickup);
			
			gameState = EGameState.GAME;
			return true;
		}

		private static void Dispose () {
			StreamWriter sw = new StreamWriter ("Documents/hiscore");
			for (int i = 0; (i < 5) && (i < hiscores.Count); i++)
				sw.WriteLine(hiscores[i]);
			Game.Dispose ();
		}
	
		private static bool Update (float delta) {
			Game.GamePadData = GamePad.GetData (0);
			switch (gameState) {
			case EGameState.GAME:
				Game.Player.update (delta);
				foreach (BaseParticle p in Game.Particles)
					p.update (delta);
				foreach (BaseParticle p in Game.pickups)
					p.update (delta);
				int enemycount = 0;
				foreach (BaseParticle p in Game.Particles)
					if (p is BaseMag)
						enemycount++;
				if (enemycount == 0)
					if (Game.Player.Health > hiscores[4].score)
						gameState = EGameState.HS_ADD;
					else
						gameState = EGameState.HISCORE;
				break;
			case EGameState.HS_ADD:
				break;
			}
			return true;
		}
	
		private static bool Render () {
			switch (gameState) {
			case EGameState.GAME:
				float aspect = Game.Graphics.Screen.AspectRatio;
				float fov = FMath.Radians (45.0f);
				//TODO: convert to orthographic camera maybe?
				//Matrix4 proj = Matrix4.Ortho(0f, aspect, 1f, 0f, 1f, -1f);;
				Matrix4 proj = Matrix4.Perspective (fov, aspect, 1.0f, 1000000.0f);
				Matrix4 view = Matrix4.LookAt (/*new Vector3(0.0f, 0.0f, 5.0f),//*/new Vector3 (0.0f, -2.5f, 3.0f),
				/*new Vector3(0.0f, 0.0f, 0.0f),//*/new Vector3 (0.0f, -0.50f, 0.0f),
		                                    Vector3.UnitY);
				//Matrix4 worldViewProj = proj;	
				Matrix4 worldViewProj = proj * view;	
				
				Game.Shader.SetUniformValue (0, ref worldViewProj);
		
				//graphics.SetViewport(0, 0, graphics.Screen.Height, graphics.Screen.Height);
				Game.Graphics.SetViewport (0, 0, Game.Graphics.Screen.Width, Game.Graphics.Screen.Height);
				Game.Graphics.SetClearColor (0.2f, 0.2f, 0.2f, 1.0f);
				Game.Graphics.Clear ();
		
				Game.Graphics.SetShaderProgram (Game.Shader);
				foreach (BaseParticle p in Game.Particles)
					p.render ();
				foreach (BaseParticle p in Game.pickups)
					p.render ();
				Game.Player.render ();
				break;
			}
			
			Game.Graphics.SwapBuffers ();
			return true;
		}
	}

} 