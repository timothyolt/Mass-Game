using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;

namespace TOltjenbruns.MassGame{
	public class CubeParticle : Particle {
		#region Private Fields
		private const float power = 60;
		private const float sustain = 0.5f;
		private const float field = 3;
		private Emitter sprayEmitter;
		
		private const float cpower = 60;
		private const float csustain = 0.75f;
		private const float cfield = 8;
		private Emitter cannonEmitter;
		
		private const float polarityFadeReset = 5;
		private float polarityFade = 0;
		#endregion
		
		#region Constructors
		public CubeParticle ()
			: base(Player.playerPoly, sprayEmitter) {
			Element.LineWidth = 2;
			Element.Scale = new Vector3(0.5f, 0.5f, 0.5f);
			Element.ColorMask = new Rgba(255, 255, 0, 255);
			Element.updateColorBuffer();
		}
		#endregion
		
		#region Override Methods
		public override void update (float delta){
			fadePolarity(delta);
			polarize(delta);
			base.update (delta);
		}
		
		private void fadePolarity(float delta){
			if (polarityUpdate){
				polarityUpdate = false;
				switch (Polarity){
				case 0:
					Element.ColorMask = new Rgba(255, 255, 0, 255);
					Element.updateColorBuffer();
					break;
				case 1:
					polarityFade = polarityFadeReset;
//					Element.ColorMask = new Rgba(255, 0, 0, 255);
//					Element.updateColorBuffer();
					break;
				case 2:
					polarityFade = polarityFadeReset;
//					Element.ColorMask = new Rgba(0, 255, 0, 255);
//					Element.updateColorBuffer();
					break;
				}
			}
			if (polarityFade > 0){
				int fade = (int)((polarityFade/5.0f) * 256.0f);
				switch (Polarity){
					case 1:
						Element.ColorMask = new Rgba(256, 256-fade, 0, 255);
						Element.updateColorBuffer();
						break;
					case 2:
						Element.ColorMask = new Rgba(256-fade, 255, 0, 255);
						Element.updateColorBuffer();
						break;
				}
				polarityFade -= delta;
				if (polarityFade <= 0){
					polarityFade = 0;
					Polarity = 0;
					Element.ColorMask = new Rgba(255, 255, 0, 255);
					Element.updateColorBuffer();
				}
			}
		}
		
		private void polarize(float delta){
			foreach (Particle p in Game.Particles)
				if (p != this && p.Polarity == Polarity && (!p.EmitterType.Equals(EmitterType.MAG)))
					p.attract (Position, Emitter, field, delta);
		}
		
		public override void transform (){
			//TODO: Fix element center
			Element.Position = Position.Multiply(0.01f).Add(new Vector3(-0.0625f,-0.0625f,0));
			Element.Rotation = Rotation;
		}
		
		public override void color (){
			//Do Nothing
		}
		#endregion
	}
}

