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
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace TOltjenbruns.MassGame {
	public static class Game {
		//TODO: add dictionary keyed by string (polarity name) with a simple struct holding a color and byte
		
		public enum EGameState {
			INTRO,
			INSTRUCTIONS,
			HISCORE,
			OPTIONS,
			ARENA,
			DEMO,
			ADD_HISCORE,
			CREDITS,
		}
		
		private static EGameState gameState;
		
		#region On GameState
		public static EGameState GameState {
			get { return gameState;}
			set {
				switch (value) {
				case EGameState.INTRO:
					onGameState_INTRO ();
					break;
				case EGameState.INSTRUCTIONS:
					onGameState_INSTRUCTIONS ();
					break;
				case EGameState.HISCORE:
					onGameState_HISCORE ();
					break;
				case EGameState.OPTIONS:
					onGameState_OPTIONS ();
					break;
				case EGameState.ARENA:
					onGameState_ARENA ();
					break;
				case EGameState.DEMO:
					onGameState_DEMO ();
					break;
				case EGameState.ADD_HISCORE:
					onGameState_ADD_HISCORE ();
					break;
				case EGameState.CREDITS:
					onGameState_CREDITS ();
					break;
				}
				gameState = value;
			}
		}
		
		private static void onGameState_INTRO(){
			
		}
		
		private static void onGameState_INSTRUCTIONS () {
			
		}
		
		private static void onGameState_HISCORE(){
			
		}
		
		private static void onGameState_OPTIONS(){
			
		}
		
		private static void onGameState_ARENA(){
			
		}
		
		private static void onGameState_DEMO(){
			Game.Particles = new HashSet<BaseParticle> ();
			Game.RemoveParticles = new HashSet<BaseParticle> ();
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
				switch (1) {//Game.Rand.Next (3)) {
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
		}
		
		private static void onGameState_ADD_HISCORE(){
			
		}
		
		private static void onGameState_CREDITS () {
			
		}
		#endregion
		
		#region Update GameState
		public static void update (float delta) {
			switch (gameState) {
			case EGameState.INTRO:
				updateGameState_INTRO (delta);
				break;
			case EGameState.INSTRUCTIONS:
				updateGameState_INSTRUCTIONS (delta);
				break;
			case EGameState.HISCORE:
				updateGameState_HISCORE (delta);
				break;
			case EGameState.OPTIONS:
				updateGameState_OPTIONS (delta);
				break;
			case EGameState.ARENA:
				updateGameState_ARENA (delta);
				break;
			case EGameState.DEMO:
				updateGameState_DEMO (delta);
				break;
			case EGameState.ADD_HISCORE:
				updateGameState_ADD_HISCORE (delta);
				break;
			case EGameState.CREDITS:
				updateGameState_CREDITS (delta);
				break;
			}
		}
		
		private static void updateGameState_INTRO(float delta){
			
		}
		
		private static void updateGameState_INSTRUCTIONS (float delta) {
			
		}
		
		private static void updateGameState_HISCORE(float delta){
			
		}
		
		private static void updateGameState_OPTIONS(float delta){
			
		}
		
		private static void updateGameState_ARENA (float delta) {
			
		}
		
		private static void updateGameState_DEMO (float delta) {
			Game.GamePadData = GamePad.GetData (0);
				Game.Player.update (delta);
			foreach (BaseParticle p in Game.Particles)
				p.update (delta);
			foreach (BaseParticle p in Game.pickups)
				p.update (delta);
			foreach (BaseParticle p in Game.RemoveParticles)
				Game.Particles.Remove (p);
			Game.RemoveParticles.Clear ();
		}
		
		private static void updateGameState_ADD_HISCORE(float delta){
			
		}
		
		private static void updateGameState_CREDITS (float delta) {
			
		}
		#endregion
		
		#region Render GameState
		public static void render () {
			switch (gameState) {
			case EGameState.INTRO:
				renderGameState_INTRO ();
				break;
			case EGameState.INSTRUCTIONS:
				renderGameState_INSTRUCTIONS ();
				break;
			case EGameState.HISCORE:
				renderGameState_HISCORE ();
				break;
			case EGameState.OPTIONS:
				renderGameState_OPTIONS ();
				break;
			case EGameState.ARENA:
				renderGameState_ARENA ();
				break;
			case EGameState.DEMO:
				renderGameState_DEMO ();
				break;
			case EGameState.ADD_HISCORE:
				renderGameState_ADD_HISCORE ();
				break;
			case EGameState.CREDITS:
				renderGameState_CREDITS ();
				break;
			}
		}
		
		private static void renderGameState_INTRO () {
			
		}
		
		private static void renderGameState_INSTRUCTIONS () {
			
		}
		
		private static void renderGameState_HISCORE () {
			
		}
		
		private static void renderGameState_OPTIONS () {
			
		}
		
		private static void renderGameState_ARENA () {
			
		}
		
		private static void renderGameState_DEMO () {
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
			Game.Graphics.SwapBuffers ();
		}
		
		private static void renderGameState_ADD_HISCORE () {
			
		}
		
		private static void renderGameState_CREDITS () {
			
		}
		#endregion
		
		public enum PolarityState : byte {
			NEUTRAL = 0,
			ENEMY = 1,
			PLAYER = 2,
		}
		
		private static CannonPickup cannonPick = null;

		public static CannonPickup CannonPickup {
			get { return cannonPick;}
			set { cannonPick = value;}
		}
		
		private static BlackHolePickup bWholePick = null;

		public static BlackHolePickup BlackHolePickup {
			get { return bWholePick;}
			set { bWholePick = value;}
		}
		
		private static PlayerMag player = null;

		public static PlayerMag Player {
			get { return player;}
			set {
				if (player == null)
					player = value;
			}
		}
		
		private static HashSet<BaseParticle> removeParticles = null;

		public static HashSet<BaseParticle> RemoveParticles {
			get { return removeParticles;}
			set {
				if (removeParticles == null)
					removeParticles = value;
			}
		}
		
		private static HashSet<BaseParticle> particles = null;

		public static HashSet<BaseParticle> Particles {
			get { return particles;}
			set {
				if (particles == null)
					particles = value;
			}
		}
		
		private static GamePadData gamePadData;

		public static GamePadData GamePadData {
			get { return gamePadData;}
			set { gamePadData = value;}
		}
		
		private static GraphicsContext graphics = null;

		public static GraphicsContext Graphics {
			get { return graphics;}
			set {
				if (graphics == null)
					graphics = value;
			}
		}
		
		private static ShaderProgram shader = null;

		public static ShaderProgram Shader {
			get { return shader;}
			set {
				if (shader == null)
					shader = value;
			}
		}
		
		private static Random random = null;

		public static Random Rand {
			get { return random;}
			set {
				if (random == null)
					random = value;
			}
		}
		
		public const float SCREEN_WIDTH = 400;
		public const float SCREEN_HEIGHT = 400;
		
		//TODO: make all other diffs use this handy little thing
		public static Vector3 LoopDiff (this Vector3 vFrom, Vector3 vTo) {
			Vector3 diff = vTo - vFrom;
			if (diff.X > SCREEN_WIDTH / 2) 
				diff.X -= SCREEN_WIDTH;
			if (diff.X < -SCREEN_WIDTH / 2)
				diff.X += SCREEN_WIDTH;
			if (diff.Y > SCREEN_HEIGHT / 2) 
				diff.Y -= SCREEN_HEIGHT;
			if (diff.Y < -SCREEN_HEIGHT / 2)
				diff.Y += SCREEN_HEIGHT;
			return diff;
		}
		
		public static List<BaseParticle> obtainedPowerUps = new List<BaseParticle> ();
		public static List<BaseParticle> pickups = new List<BaseParticle> ();
		
		public static void Dispose () {
			shader.Dispose ();
			graphics.Dispose ();
			player.dispose ();
			foreach (BaseParticle p in Game.Particles)
				p.dispose ();
			player = null;
			removeParticles = null;
			particles = null;
			graphics = null;
			shader = null;
			random = null;
		}
	}
}

