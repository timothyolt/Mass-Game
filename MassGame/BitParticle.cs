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
using Sce.PlayStation.Core;

namespace TOltjenbruns.MassGame {
	public class BitParticle : BaseParticle {
		#region Private Fields
		private const float power = 60;
		private const float sustain = 0.75f;
		private const float field = 5;
		private Emitter sprayEmitter;
		private const float cpower = 60;
		private const float csustain = 0.5f;
		private const float cfield = 1;
		private Emitter cannonEmitter;
		private bool cannonState = false;
		private const float bpower = 250;
		private const float bsustain = 0.8f;
		private const float bfield = 100;
		private Emitter blackEmitter;
		private bool blackHoleState = false;
		private const float epower = 500;
		private const float esustain = 0.8f;
		private const float efield = 15;
		private Emitter explodeEmitter;
		private const float polarityFadeReset = 5;
		private const float polarityFadeCannon = 4.8f;
		private const float polarityFadeBlack = 2f;
		private float polarityFade = 0;
		#endregion
		
		#region Constructors
		
		public BitParticle (byte polarity)
			: base(PlayerMag.playerPoly, polarity, new Emitter(power, sustain, field, EmitterType.BIT)) {
			sprayEmitter = Emitter;
			cannonEmitter = new Emitter (cpower, csustain, cfield, EmitterType.BIT);
			explodeEmitter = new Emitter (epower, esustain, efield, EmitterType.BIT);
			blackEmitter = new Emitter (bpower, bsustain, bfield, EmitterType.BIT);
			Element.LineWidth = 2;
			Element.Scale = new Vector3 (0.5f, 0.5f, 0.5f);
			Element.ColorMask = new Rgba (255, 255, 0, 255);
			Element.updateColorBuffer ();
		}
		#endregion
		
		#region Original Methods
		private void fadePolarity (float delta) {
			if (
					(cannonState && polarityFade < polarityFadeCannon) ||
					(blackHoleState && polarityFade < polarityFadeBlack)
				) {
				cannonState = false;
				blackHoleState = false;
				Emitter = explodeEmitter;
			}
			if (polarityFade > 0) {
				int fade = (int)((polarityFade / 5.0f) * 256.0f);
				polarityFade -= delta;
				switch (Polarity) {
				case 1:
					Element.ColorMask = new Rgba (25, 25 - fade, 0, 255);
					Element.updateColorBuffer ();
					break;
				case 2:
					Element.ColorMask = new Rgba (255 - fade, 255, 0, 255);
					Element.updateColorBuffer ();
					break;
				case 3:
					Element.ColorMask = new Rgba (255, 255 - fade, fade, 255);
					Element.updateColorBuffer ();
					break;
				case 4:
					Element.ColorMask = new Rgba (255 - fade, 255 - fade, 0, 255);
					Element.updateColorBuffer ();
					break;
				}
				if (polarityFade <= 0) {
					Emitter = sprayEmitter;
					polarityFade = 0;
					Polarity = 0;
					Element.ColorMask = new Rgba (255, 255, 0, 255);
					Element.updateColorBuffer ();
				}
			}
		}
		
		private void polarize (float delta) {
			foreach (BaseParticle p in Game.Particles) {
				if (p != this && (!p.EmitterType.Equals (EmitterType.MAG)))
				if (blackHoleState) {
					p.attract (Position, Emitter, delta, false);
					if ((Position.LoopDiff (p.Position).Length () < Emitter.field / 2))// && (p.Polarity != Polarity))
						p.Polarity = Polarity;
				}
				else if (p.Polarity == Polarity)
					p.attract (Position, Emitter, Polarity, delta);
			}
		}
		
		public void fireCannon () {
			cannonState = true;
			Emitter = cannonEmitter;
			Element.ColorMask = new Rgba (255, 0, 255, 255);
			Element.updateColorBuffer ();
			polarityFade = polarityFadeReset;
		}
		
		public void fireSpray () {
			polarityFade = polarityFadeReset;
		}
		
		public void fireBlackHole () {
			blackHoleState = true;
			Emitter = blackEmitter;
			Element.ColorMask = new Rgba (0, 0, 0, 255);
			Element.updateColorBuffer ();
			polarityFade = polarityFadeReset;
		}
		#endregion
		
		#region Override Methods
		public override void update (float delta) {
			fadePolarity (delta);
			polarize (delta);
			base.update (delta);
		}
		
		protected override void updatePolarity (byte polarity) {
			switch (polarity) {
			case 0:
				Element.ColorMask = new Rgba (255, 255, 0, 255);
				Element.updateColorBuffer ();
				break;
			case 1:
				if ((!cannonState) && (!blackHoleState)) {
					Element.ColorMask = new Rgba (255, 0, 0, 255);
					Element.updateColorBuffer ();
					if (polarityFade == 0)
						polarityFade = polarityFadeReset;
				}
				break;
			case 2:
				Element.ColorMask = new Rgba (0, 255, 0, 255);
				Element.updateColorBuffer ();
				if (polarityFade == 0)
					polarityFade = polarityFadeReset;
				break;
			}
		}
		
		public override void transform () {
			//TODO: Fix element center
			Element.Position = Position.Multiply (0.01f).Add (new Vector3 (-0.0625f, -0.0625f, 0));
			Element.Rotation = Rotation;
		}
		
		public override void color () {
			//Do Nothing
		}
		#endregion
	}
}

