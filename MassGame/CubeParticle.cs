using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;

namespace TOltjenbruns.MassGame{
	public class CubeParticle : Particle {
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
		
		private const float epower = 250;
		private const float esustain = 0.8f;
		private const float efield = 20;
		private Emitter explodeEmitter;
		
		private const float polarityFadeReset = 5;
		private const float polarityFadeCannon = 4.8f;
		private float polarityFade = 0;
		#endregion
		
		public override byte Polarity{
			get {return base.Polarity;}
			set {
				sprayEmitter.polarity = value;
				cannonEmitter.polarity = value;
				explodeEmitter.polarity = value;
				base.Polarity = value;
			}
		}
		
		#region Constructors
		
		public CubeParticle ()
			: base(Player.playerPoly, new Emitter(power, sustain, field, 0, EmitterType.BIT)) {
			sprayEmitter = Emitter;
			cannonEmitter = new Emitter(cpower, csustain, cfield, 0, EmitterType.BIT);
			explodeEmitter = new Emitter(epower, esustain, efield, 0, EmitterType.BIT);
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
				default:
					polarityFade = polarityFadeReset;
					break;
				}
			}
			
			if (polarityFade > 0){
				if (cannonState && polarityFade < polarityFadeCannon){
					//Console.WriteLine(Polarity);
					Emitter = explodeEmitter;
				}
				int fade = (int)((polarityFade/5.0f) * 256.0f);
				switch (Polarity){
					case 1:
						Element.ColorMask = new Rgba(256, 256-fade, 0, 255);
						Element.updateColorBuffer();
						break;
					case 2:
						Element.ColorMask = new Rgba(255-fade, 255, 0, 255);
						Element.updateColorBuffer();
						break;
					case 3:
						Element.ColorMask = new Rgba(255, 255-fade, fade, 255);
						Element.updateColorBuffer();
						break;
					case 4:
						Element.ColorMask = new Rgba(255-fade, 255-fade, 0, 255);
						Element.updateColorBuffer();
						break;
				}
				polarityFade -= delta;
				if (polarityFade <= 0){	
					Emitter = sprayEmitter;
					polarityFade = 0;
					Polarity = 0;
					Element.ColorMask = new Rgba(255, 255, 0, 255);
					Element.updateColorBuffer();
				}
			}
		}
		
		private void polarize(float delta){
			foreach (Particle p in Game.Particles)
//<<<<<<< HEAD
//				if (p is CubeParticle) {
//					if (p != this){
//						//if(cooldown > 4 && cooldown < 5 && Polarity == 3){
//							p.attract (Position,cannonEmitter,3,delta);
//						//}else
//							//p.attract (Position, Emitter, field, delta);
//					}
//				}
//				
//			//TODO: Fix element center
//			//Rotation = Math.Atan2(velocity.Y, velocity.X);
//			base.update (delta);
//=======
				if (p != this && p.Polarity == Polarity && (!p.EmitterType.Equals(EmitterType.MAG)))
					p.attract (Position, Emitter, delta);
		}
		
		public void fireCannon(){
			cannonState = true;
			Emitter = cannonEmitter;
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

