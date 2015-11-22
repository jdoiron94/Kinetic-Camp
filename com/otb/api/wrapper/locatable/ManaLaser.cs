﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace OutsideTheBox {

    public class ManaLaser : Laser {

        public ManaLaser(Texture2D texture, Vector2 location, SoundEffectInstance effect, int height, int width, bool Activated, bool? defaultValue = null) :
            base(texture, location, effect, height, width, Activated, defaultValue) {

        }

        public override void update(InputManager inputManager) {
            if (isActivated()) {
                inputManager.getPlayerManager().depleteMana(50);
            }
        }
    }
}
