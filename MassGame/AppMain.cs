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
		private static TextRender tr;
		
		private static Element uiArrowUp;
		private static Element uiArrowDown;
		private static Element uiArrowLeft;
		private static Element uiArrowRight;
		private static Element uiArrowName;
		
		private static string name = "AAA";
		private static int nameLetterIndex = 0;
		private static int nameIndex = 0;
		
		private static bool running = true;
		
		public enum EGameState {
			INTRO,
			GAME,
			HS_ADD,
			HISCORE,
		}
		
		private static List<HiscoreEntry> hiscores;
		
		private static EGameState gameState;

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
			hiscores = new List<HiscoreEntry> ();
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
			for (int i = 0; i < 1; i++)
				switch (5) {//Game.Rand.Next (3)) {
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
			
			gameState = EGameState.HISCORE;
			
			tr = new TextRender ("Application/polygons/font");
			
			Polygon arrow = Polygon.Parse ("Application/polygons/Arrow.poly");
			uiArrowUp = new Element (arrow);
			uiArrowUp.Position = new Vector3 (-0.2f, 1.6f, 0);
			uiArrowUp.Scale = new Vector3 (1, -1, 1);
			uiArrowUp.updateTransBuffer ();
			uiArrowUp.updateColorBuffer ();
			uiArrowDown = new Element (arrow);
			uiArrowDown.Position = new Vector3 (-0.2f, -1.9f, 0);
			uiArrowDown.updateTransBuffer ();
			uiArrowDown.updateColorBuffer ();
			uiArrowLeft = new Element (arrow);
			uiArrowRight = new Element (arrow);
			uiArrowRight.Position = new Vector3 (0.4f, -1.9f, 0);
			uiArrowRight.Rotation = 3.14/2;
			uiArrowRight.updateColorBuffer ();
			uiArrowRight.updateTransBuffer ();
			uiArrowName = new Element (arrow);
			uiArrowName.Position = new Vector3 (-0.6f, 0.5f, 0);
			uiArrowName.Scale = new Vector3 (1, -1, 1);
			uiArrowName.updateTransBuffer ();
			uiArrowName.updateColorBuffer ();
			return true;
		}

		private static void Dispose () {
			StreamWriter sw = new StreamWriter ("Documents/hiscore");
			foreach (HiscoreEntry entry in hiscores)
				sw.WriteLine (entry.ToString ());
			sw.Close();
			Game.Dispose ();
		}
	
		private static bool Update (float delta) {
			GamePadData gamePad = GamePad.GetData (0);
			Game.GamePadData = gamePad;
			switch (gameState) {
			case EGameState.INTRO:
				if ((gamePad.Buttons & GamePadButtons.Up) != 0)
					gameState = EGameState.GAME;
				if ((gamePad.Buttons & GamePadButtons.Down) != 0)
					running = false;
				//if ((gamePad.Buttons & GamePadButtons.Left) != 0)
				//	gameState = EGameState.HISCORE;
				//if ((gamePad.Buttons & GamePadButtons.Up) != 0)
				//	gameState = EGameState.INSTRUCTIONS;
				break;
			case EGameState.HISCORE:
				if ((gamePad.Buttons & GamePadButtons.Right) != 0)
					gameState = EGameState.INTRO;
				break;
			case EGameState.HS_ADD:
				if ((gamePad.Buttons & GamePadButtons.Down) != 0) {
					gameState = EGameState.HISCORE;
					hiscores.Add (new HiscoreEntry (name, (int) Game.Player.Health));
					hiscores.Sort ();
					if (hiscores.Count > 5)
						hiscores.RemoveRange (5, hiscores.Count - 5);
				}
				if ((gamePad.ButtonsUp & GamePadButtons.Right) != 0) {
					nameIndex += 1;
					if (nameIndex > 2)
						nameIndex = 0;
					nameLetterIndex = tr.CharMap.IndexOf (name [nameIndex]);
					uiArrowName.Position = new Vector3 (-0.6f + (nameIndex * 0.4f), 0.5f, 0);
					uiArrowName.updateTransBuffer ();
				}
				if ((gamePad.ButtonsUp & GamePadButtons.Left) != 0) {
					nameIndex -= 1;
					if (nameIndex < 0)
						nameIndex = 2;
					nameLetterIndex = tr.CharMap.IndexOf (name [nameIndex]);
					uiArrowName.Position = new Vector3 (-0.6f + (nameIndex * 0.4f), 0.5f, 0);
					uiArrowName.updateTransBuffer ();
				}
				if ((gamePad.ButtonsUp & GamePadButtons.Up) != 0) {
					nameLetterIndex += 1;
					if (nameLetterIndex > tr.CharMap.Length - 1)
						nameLetterIndex = 0;
					switch (nameIndex) {
					case 0:
						name = "" + tr.CharMap [nameLetterIndex] + name [1] + name [2];
						break;
					case 1:
						name = "" + name [0] + tr.CharMap [nameLetterIndex] + name [2];
						break;
					case 2:
						name = "" + name [0] + name [1] + tr.CharMap [nameLetterIndex];
						break;
					}
				}
				break;
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
				if (Game.Player.Health > hiscores [4].score)
					gameState = EGameState.HS_ADD;
				else
					gameState = EGameState.HISCORE;
				break;
			}
			return true;
		}
	
		private static bool Render () {
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
			int t = (int)(DateTime.Now.Millisecond / 1000f * 512);
			switch (gameState) {
			case EGameState.INTRO:
				tr.render ("MASS", new Vector3 (-.8f, 0, 0), new Rgba (
					(int)Math.Abs (((t) % 512f) - 256), 
					(int)Math.Abs (((t + 128) % 512f) - 256), 
					(int)Math.Abs (((t + 256) % 512f) - 256), 255), 2);
				//tr.render ("MASS", new Vector3 (-.8f, 0, 0), new Rgba (255 * (t % 500) / 500, 255 * ((t + 250) % 1000) / 1000, 255 * ((t + 1000) % 2000) / 2000, 255), 2);
				tr.render ("INITIATE SIMULATION", new Vector3 (-1.7f, 1, 0));
				tr.render ("TERMINATE SESSION", new Vector3 (-.8f, -1.3f, 0), new Rgba (255, 255, 255, 255), 0.5f);
				uiArrowUp.draw (Game.Graphics);
				uiArrowDown.draw (Game.Graphics);
				break;
			case EGameState.GAME:
				foreach (BaseParticle p in Game.Particles)
					p.render ();
				foreach (BaseParticle p in Game.pickups)
					p.render ();
				Game.Player.render ();
				break;
			case EGameState.HS_ADD:
				tr.render (name, new Vector3 (-.55f, 0, 0), new Rgba (
					(int)Math.Abs (((t) % 512f) - 256), 
					(int)Math.Abs (((t + 128) % 512f) - 256), 
					(int)Math.Abs (((t + 256) % 512f) - 256), 255), 2);
				tr.render ("ENTER NAME", new Vector3 (-1f, 1, 0));
				tr.render ("CONFIRM", new Vector3 (-.35f, -1.3f, 0), new Rgba (255, 255, 255, 255), 0.5f);
				uiArrowName.draw (Game.Graphics);
				uiArrowDown.draw (Game.Graphics);
				break;
			case EGameState.HISCORE:
				tr.render ("LEGENDARY MAGS", new Vector3 (-2.8f, 1.6f, 0), new Rgba (255, 255, 255, 255), 2f);
				tr.render ("REINITIALIZE", new Vector3 (-.8f, -1.3f, 0), new Rgba (255, 255, 255, 255));
				uiArrowRight.draw(Game.Graphics);
				for (int i = 0; i < hiscores.Count; i++)
					tr.render (hiscores [i].ToString (), new Vector3 (-1f, 0.8f - (i * 0.4f), 0), new Rgba (
						(int)Math.Abs (((t + (i * 32)) % 512f) - 256), 
						(int)Math.Abs (((t + (i * 64) + 128) % 512f) - 256), 
						(int)Math.Abs (((t + (i * 128) + 256) % 512f) - 256), 255), 1.5f);
				break;
			}
			
			Game.Graphics.SwapBuffers ();
			return true;
		}
	}

} 