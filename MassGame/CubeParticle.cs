using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;

namespace TOltjenbruns.MassGame{
	public class CubeParticle : Particle {
		#region Private Fields
		private const float power = 60;
		private const float sustain = 0.90f;
		private const float field = 5;
		
		private float cooldown = 0;
		private Player player;
		#endregion
		
		#region Constructors
		public CubeParticle ()
			: base(Player.playerPoly, new Emitter(power, sustain, 0, ParticleType.BIT)) {
			Element.LineWidth = 2;
			Element.Scale = new Vector3(0.5f, 0.5f, 0.5f);
			Element.ColorMask = new Rgba(255, 255, 0, 255);
			Element.updateColorBuffer();
		}
		#endregion
		
		#region Override Methods
		public override void update (float delta){
			//TODO: move attract call to player
			//attract (player.Position, pEmit, pField, delta);
			if (polarityUpdate){
				polarityUpdate = false;
				switch (Polarity){
				case 0:
					Element.ColorMask = new Rgba(255, 255, 0, 255);
					Element.updateColorBuffer();
					break;
				case 1:
					cooldown = 5;
//					Element.ColorMask = new Rgba(255, 0, 0, 255);
//					Element.updateColorBuffer();
					break;
				case 2:
					cooldown = 5;
//					Element.ColorMask = new Rgba(0, 255, 0, 255);
//					Element.updateColorBuffer();
					break;
				}
			}
			if (cooldown > 0){
				int fade = (int)((cooldown/5.0f) * 256.0f);
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
				cooldown -= delta;
				if (cooldown <= 0){
					cooldown = 0;
					Polarity = 0;
					Element.ColorMask = new Rgba(255, 255, 0, 255);
					Element.updateColorBuffer();
				}
			}
			foreach (Particle p in Game.Particles)
				if (p != this) p.attract (Position, Emitter, field, delta);
			//TODO: Fix element center
			//Rotation = Math.Atan2(velocity.Y, velocity.X);
			base.update (delta);
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

