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
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using TOltjenbruns.MassGame.Particle;
using TOltjenbruns.MassGame.Particle.Bit;
using TOltjenbruns.MassGame.Particle.Mag;

namespace TOltjenbruns.MassGame {

	public static class Game {
		//TODO: add dictionary keyed by string (polarity name) with a simple struct holding a color and byte
		
		private static PickupCannon pickupCannon = null;

		public static PickupCannon PickupCannon {
			get { return pickupCannon;}
			set { pickupCannon = value;}
		}
		
		private static PickupBlackHole pickupBlackHole = null;

		public static PickupBlackHole PickupBlackHole {
			get { return pickupBlackHole;}
			set { pickupBlackHole = value;}
		}
		
		private static Player player = null;

		public static Player Player {
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
		
		public static List<BaseParticle> obtainedPowerUps = new List<BaseParticle>();
		public static List<BaseParticle> groundPowerUps = new List<BaseParticle>();
		
		public static void Dispose () {
			shader.Dispose();
			graphics.Dispose();
			player.dispose();
			foreach (BaseParticle p in Game.Particles)
				p.dispose();
			player = null;
			removeParticles = null;
			particles = null;
			graphics = null;
			shader = null;
			random = null;
		}
	}
}

