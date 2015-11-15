﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OutsideTheBox {

    /// <summary>
    /// Class which handles door object functionality
    /// </summary>

    public class Door : GameObject {

        private bool next;
        private bool unlocked;

        private Texture2D open;
        private Texture2D closed;

        private Door door; 

        public Door(Texture2D[] texture, Projectile projectile, Vector2 location, Direction direction, bool liftable, bool next, int width, int height, bool unlocked, Door door) :
            base(texture[0], projectile, location, direction, liftable, width, height) {
            this.next = next;
            this.unlocked = unlocked;
            this.door = door; 
            open = texture[0];
            closed = texture[1];
        }

        /// <summary>
        /// Sets the door's next bool
        /// </summary>
        /// <param name="next">Bool telling us if door leads to a new level</param>
        public void setNext(bool next) {
            this.next = next;
        }

        /// <summary>
        /// Returns whether or not there is a next level
        /// </summary>
        /// <returns>Returns true if the door leads to a new level; otherwise, false</returns>
        public bool getNext() {
            return next;
        }


        public Door getDoor()
        {
            return door; 
        }

        public void setDoor(Door door)
        {
            this.door = door;
        }

        /// <summary>
        /// Returns whether or not the door is unlocked
        /// </summary>
        /// <returns>Returns true if the door is unlocked; otherwise, false</returns>
        public bool isUnlocked() {
            return unlocked;
        }

        /// <summary>
        /// Handles unlocking of the door
        /// </summary>
        /// <param name="value">Bool to determine the door's unlocked property</param>
        public void unlockDoor(bool value) {
            unlocked = value;
        }

        public void draw(SpriteBatch batch) {
            batch.Draw(unlocked ? open : closed, getLocation(), Color.White);
        }
    }
}
