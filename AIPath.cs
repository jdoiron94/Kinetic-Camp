﻿using Microsoft.Xna.Framework;

namespace KineticCamp {

    public class AIPath {

        /*
         * Class which contains information to create a path for an artificially intelligent
         * npc to follow a static path through the game.
         */

        // TODO: Support delays between moving directions and animations

        private int state;
        private byte ticks;

        private readonly int[] path;
        private readonly int[] delays;

        private const byte SKIPPED_FRAMES = 0x4;

        private readonly Npc npc;
        private readonly Player player;
        private readonly CollisionManager collisionManager;
        private readonly Direction[] directions;
        
        public AIPath(Npc npc, Game1 game, int[] path, int[] delays, Direction[] directions) {
            this.npc = npc;
            player = game.getPlayer();
            collisionManager = game.getInputManager().getCollisionManager();
            this.path = path;
            this.delays = delays;
            this.directions = directions;
            state = 0;
            ticks = 0;
        }

        /// <summary>
        /// Returns an instance of the npc
        /// </summary>
        /// <returns>Returns an instance of the npc</returns>
        public Npc getNpc() {
            return npc;
        }

        /// <summary>
        /// Returns the path followed by the npc
        /// </summary>
        /// <returns>Returns the integer array path followed by the npc</returns>
        public int[] getPath() {
            return path;
        }

        /// <summary>
        /// Returns the delays followed by the npc
        /// </summary>
        /// <returns>Returns the integer array of delays between directions, followed by the npc</returns>
        public int[] getDelays() {
            return delays;
        }

        /// <summary>
        /// Returns the directions followed by the npc
        /// </summary>
        /// <returns>Returns the direction array followed by the npc</returns>
        public Direction[] getDirections() {
            return directions;
        }

        /// <summary>
        /// Returns the number of frames skipped between movements
        /// </summary>
        /// <returns>Returns 4</returns>
        public byte getSkippedFrames() {
            return SKIPPED_FRAMES;
        }

        /// <summary>
        /// Updates the npc's direction and movement, if it has been sufficient time between interactions
        /// </summary>
        public void update() {
            npc.setDirection(directions[state]);
            Vector2 destination;
            Vector2 prevDest = npc.getDestination();
            switch (npc.getDirection()) {
                case Direction.NORTH:
                    if (npc.getLocation().Y > path[state]) {
                        if (ticks >= SKIPPED_FRAMES) {
                            destination = new Vector2(npc.getLocation().X, npc.getLocation().Y - npc.getVelocity());
                            npc.setDestination(destination);
                            if (collisionManager.isValid(npc)) {
                                npc.deriveY(-npc.getVelocity());
                            }
                            ticks = 0;
                        } else {
                            ticks++;
                        }
                    }
                    state = npc.getLocation().Y <= path[state] ? (state + 1) % path.Length : state;
                    break;
                case Direction.SOUTH:
                    if (npc.getLocation().Y < path[state]) {
                        if (ticks >= SKIPPED_FRAMES) {
                            destination = new Vector2(npc.getLocation().X, npc.getLocation().Y + npc.getVelocity());
                            npc.setDestination(destination);
                            if (collisionManager.isValid(npc)) {
                                npc.deriveY(npc.getVelocity());
                            }
                            ticks = 0;
                        } else {
                            ticks++;
                        }
                    }
                    state = npc.getLocation().Y >= path[state] ? (state + 1) % path.Length : state;
                    break;
                case Direction.WEST:
                    if (npc.getLocation().X > path[state]) {
                        if (ticks >= SKIPPED_FRAMES) {
                            destination = new Vector2(npc.getLocation().X - npc.getVelocity(), npc.getLocation().Y);
                            npc.setDestination(destination);
                            if (collisionManager.isValid(npc)) {
                                npc.deriveX(-npc.getVelocity());
                            }
                            ticks = 0;
                        } else {
                            ticks++;
                        }
                    }
                    state = npc.getLocation().X <= path[state] ? (state + 1) % path.Length : state;
                    break;
                case Direction.EAST:
                    if (npc.getLocation().X < path[state]) {
                        if (ticks >= SKIPPED_FRAMES) {
                            destination = new Vector2(npc.getLocation().X + npc.getVelocity(), npc.getLocation().Y);
                            npc.setDestination(destination);
                            if (collisionManager.isValid(npc)) {
                                npc.deriveX(npc.getVelocity());
                            }
                            ticks = 0;
                        } else {
                            ticks++;
                        }
                    }
                    state = npc.getLocation().X >= path[state] ? (state + 1) % path.Length : state;
                    break;
            }
        }
    }
}