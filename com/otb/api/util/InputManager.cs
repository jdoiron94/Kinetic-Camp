using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace OutsideTheBox {

    /// <summary>
    /// Class which handles all input given to the game
    /// </summary>

    public class InputManager {

        private readonly Game1 game;
        private readonly Player player;
        private readonly Menu pauseMenu;
        private readonly PlayerManager playerManager;
        private readonly CollisionManager collisionManager;
        private readonly ScreenManager screenManager;
        private readonly Target target;

        private Level level;
        private MindRead mindRead;
        private GameObject selectedObject;
        private DeathManager deathManager;
        private KeyboardState lastKeyState;
        private KeyboardState currentKeyState;

        private ButtonState lastState;
        private ButtonState state;

        private readonly int velocity;
        private readonly int midX;
        private readonly int midY;

        private int ticks;
        private bool stagnant;
        private bool moving;
        private bool powerReveal;

        private const byte WAIT = 0x4;

        public InputManager(Game1 game, Player player, Level level, Menu pauseMenu, Target target, PlayerManager playerManager, Screen[] screens, MindRead mindRead) {
            this.game = game;
            this.player = player;
            this.level = level;
            this.pauseMenu = pauseMenu;
            this.target = target;
            this.playerManager = playerManager;
            this.mindRead = mindRead;
            powerReveal = false;
            collisionManager = new CollisionManager(player, level);
            screenManager = new ScreenManager(screens[4], screens);
            selectedObject = null;
            velocity = player.getVelocity();
            midX = game.getMidX();
            midY = game.getMidY();
            lastKeyState = new KeyboardState();
            currentKeyState = new KeyboardState();
            ticks = 0;
            stagnant = false;
            moving = false;
        }

        /// <summary>
        /// Returns an instance of the game
        /// </summary>
        /// <returns>Returns an instance of the game</returns>
        public Game1 getGame() {
            return game;
        }

        /// <summary>
        /// Returns an instance of the player
        /// </summary>
        /// <returns>Returns an instance of the player</returns>
        public Player getPlayer() {
            return player;
        }

        /// <summary>
        /// Returns an instance of the level
        /// </summary>
        /// <returns>Returns an instance of the level</returns>
        public Level getLevel() {
            return level;
        }

        /// <summary>
        /// Returns the menu
        /// </summary>
        /// <returns>Returns the pause menu</returns>
        public Menu getMenu() {
            return pauseMenu;
        }

        /// <summary>
        /// Returns an instance of the player manager
        /// </summary>
        /// <returns>Returns an instance of the player manager</returns>
        public PlayerManager getPlayerManager() {
            return playerManager;
        }

        /// <summary>
        /// Returns the collision manager
        /// </summary>
        /// <returns>Returns the collision manager</returns>
        public CollisionManager getCollisionManager() {
            return collisionManager;
        }

        /// <summary>
        /// Returns the screen manager
        /// </summary>
        /// <returns>Returns the screen manager</returns>
        public ScreenManager getScreenManager() {
            return screenManager;
        }

        /// <summary>
        /// Returns the power reveal bool
        /// </summary>
        /// <returns>Returns the power reveal bool</returns>
        public bool getPowerReveal() {
            return powerReveal;
        }

        /// <summary>
        /// Sets the power reveal bool
        /// </summary>
        /// <param name="reveal">Sets the power reveal bool</param>
        public void setPowerReveal(bool reveal) {
            powerReveal = reveal;
        }

        /// <summary>
        /// Sets the death manager
        /// </summary>
        /// <param name="deathManager">Sets the death manager</param>
        public void setDeathManager(DeathManager deathManager) {
            this.deathManager = deathManager;
        }

        /// <summary>
        /// Controls updating of the game based on the current screen state and mouse/keyboard input
        /// </summary>
        /// <param name="time">The GameTime to update with respect to</param>
        public void update(GameTime time) {
            lastKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            Screen active = screenManager.getActiveScreen();
            if (currentKeyState.IsKeyDown(Keys.Escape)) {
                game.Exit();
            }
            if (playerManager.getHealth() <= 0) {
                Console.WriteLine("reset game");
                deathManager.resetGame();
            }
            if (collisionManager.playerSpotted(level)) {
                player.setLocation(new Vector2(0F, 0F));
                player.deriveHealth(10);
            }
            if (lastKeyState.IsKeyDown(Keys.F1) && currentKeyState.IsKeyUp(Keys.F1)) {
                foreach (Level l in game.getLevels()) {
                    l.toggleDebug();
                }
            }
            if (active.getName() == "Start") {
                if (lastKeyState.IsKeyDown(Keys.Space) && currentKeyState.IsKeyUp(Keys.Space)) {
                    level.setMode(0);
                    screenManager.setActiveScreen(1);
                }
            } else if (active.getName() == "Normal") {
                if (playerManager.getHealthCooldown() == 35) {
                    playerManager.regenerateHealth();
                    playerManager.regenerateMana();
                }
                foreach (ThoughtBubble tb in level.getThoughts()) {
                    tb.reveal(mindRead.isActivated());
                    tb.updateLocation();
                }
                GameObject gCollision = collisionManager.getObjectCollision(player);
                if (gCollision != null && gCollision is Token) {
                    Token t = (Token) gCollision;
                    t.setCollected(true);
                    playerManager.incrementExperience(t.getExp());
                    level.removeToken(t);
                } else if (gCollision != null && gCollision is Door) {
                    Door d = (Door) gCollision;
                    int index = (game.getLevelIndex()) + (d.getNext() ? 1 : -1);
                    level.setActive(false);
                    game.setLevel(index);
                    level = game.getLevel(index);
                    deathManager = new DeathManager(this);
                    setDeathManager(deathManager);
                    collisionManager.getLevel().setActive(false);
                    level.setActive(true);
                    level.setInputManager(this);
                    collisionManager.setLevel(level);
                    game.setLevel(level);
                    if (d.getDirection() == Direction.East) {
                        player.setLocation(new Vector2(40F, d.getLocation().Y));
                    } else if (d.getDirection() == Direction.West) {
                        player.setLocation(new Vector2(696F, d.getLocation().Y));
                    } else if (d.getDirection() == Direction.North) {
                        player.setLocation(new Vector2(d.getLocation().X, 376F)); // untested
                    } else {
                        player.setLocation(new Vector2(d.getLocation().X, 104F)); // untested
                    }
                }
                if (lastKeyState.IsKeyDown(Keys.H) && currentKeyState.IsKeyUp(Keys.H)) {
                    if (mindRead.isCooldown()) {
                        mindRead.activatePower(true);
                        playerManager.depleteMana(mindRead.getManaCost());
                    }
                }
                mindRead.behavior(time);
                SlowTime slowmo = (SlowTime) playerManager.getPowers()[0];
                if (lastKeyState.IsKeyDown(Keys.L) && currentKeyState.IsKeyUp(Keys.L)) {
                    if (slowmo.isUnlocked() && !slowmo.isActivated()) {
                        if (slowmo.isCooldown()) {
                            slowmo.activatePower(true);
                            playerManager.depleteMana(slowmo.getManaCost());
                        }
                    }
                }
                slowmo.doStuff(level);
                Dash dash = (Dash) playerManager.getPowers()[1];
                if (lastKeyState.IsKeyDown(Keys.K) && currentKeyState.IsKeyUp(Keys.K)) {
                    if (dash.isUnlocked()) {
                        if (dash.isCooldown() && !dash.isActivated()) {
                            dash.activatePower(true);
                            playerManager.depleteMana(dash.getManaCost());
                        }
                    }
                }
                dash.doStuff(level);
                Confuse confuse = (Confuse) playerManager.getPowers()[2];
                if (lastKeyState.IsKeyDown(Keys.C) && currentKeyState.IsKeyUp(Keys.C)) {
                    if (confuse.isUnlocked()) {
                        if (confuse.isCooldown() && !confuse.isActivated()) {
                            confuse.activatePower(true);
                            playerManager.depleteMana(confuse.getManaCost());
                        }
                    }
                }
                confuse.doStuff(level);
                if (currentKeyState.IsKeyDown(Keys.W)) {
                    player.setDirection(Direction.North);
                    player.updateMovement();
                    player.setDestination(new Vector2(player.getLocation().X, player.getLocation().Y - velocity));
                    if (player.getDestination().Y >= 0 && collisionManager.isValid(player)) {
                        player.deriveY(-velocity);
                    }
                } else if (lastKeyState.IsKeyDown(Keys.W) && currentKeyState.IsKeyUp(Keys.W)) {
                    stagnant = true;
                } else if (currentKeyState.IsKeyDown(Keys.S)) {
                    player.setDirection(Direction.South);
                    player.updateMovement();
                    player.setDestination(new Vector2(player.getLocation().X, player.getLocation().Y + velocity));
                    if (player.getDestination().Y <= midY * 2 && collisionManager.isValid(player)) {
                        player.deriveY(velocity);
                    }
                } else if (lastKeyState.IsKeyDown(Keys.S) && currentKeyState.IsKeyUp(Keys.S)) {
                    stagnant = true;
                } else if (currentKeyState.IsKeyDown(Keys.A)) {
                    player.setDirection(Direction.West);
                    player.updateMovement();
                    player.setDestination(new Vector2(player.getLocation().X - velocity, player.getLocation().Y));
                    if (player.getDestination().X >= 0 && collisionManager.isValid(player)) {
                        player.deriveX(-velocity);
                    }
                } else if (lastKeyState.IsKeyDown(Keys.A) && currentKeyState.IsKeyUp(Keys.A)) {
                    stagnant = true;
                } else if (currentKeyState.IsKeyDown(Keys.D)) {
                    player.setDirection(Direction.East);
                    player.updateMovement();
                    player.setDestination(new Vector2(player.getLocation().X + velocity, player.getLocation().Y));
                    if (player.getDestination().X <= midX * 2 && collisionManager.isValid(player)) {
                        player.deriveX(velocity);
                    }
                } else if (lastKeyState.IsKeyDown(Keys.D) && currentKeyState.IsKeyUp(Keys.D)) {
                    stagnant = true;
                } else {
                    player.setDestination(player.getLocation());
                }
                if (currentKeyState.IsKeyDown(Keys.Space)) {
                    double ms = time.TotalGameTime.TotalMilliseconds;
                    if ((player.getLastFired() == -1 || ms - player.getLastFired() >= player.getProjectile().getCooldown()) && playerManager.getMana() >= 5) {
                        level.addProjectile(player.createProjectile(ms));
                        playerManager.depleteMana(5);
                    }
                }
                if (stagnant) {
                    if (ticks >= WAIT) {
                        player.updateStill();
                        ticks = 0;
                        stagnant = false;
                    } else {
                        ticks++;
                    }
                }
                if (lastKeyState.IsKeyDown(Keys.X) && currentKeyState.IsKeyUp(Keys.X)) {
                    level.setMode(1);
                    screenManager.setActiveScreen(2);
                    target.setActive(true);
                } else if (currentKeyState.IsKeyDown(Keys.P)) {
                    playerManager.damagePlayer(2);
                } else if (lastKeyState.IsKeyDown(Keys.M) && currentKeyState.IsKeyUp(Keys.M)) {
                    screenManager.setActiveScreen(0);
                    level.setActive(false);
                    pauseMenu.setActive(true);
                }
            } else if (active.getName() == "Telekinesis-Select") {
                lastState = state;
                state = Mouse.GetState().LeftButton;
                playerManager.setManaDrainRate(5);
                if (lastState == ButtonState.Pressed && state == ButtonState.Released) {
                    foreach (GameObject obj in level.getObjects()) {
                        if (obj.isLiftable()) {
                            if (obj.getBounds().Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y))) {
                                obj.setSelected(true);
                                selectedObject = obj;
                                level.setMode(2);
                                screenManager.setActiveScreen(3);
                            }
                        }
                    }
                } else if (lastKeyState.IsKeyDown(Keys.X) && currentKeyState.IsKeyUp(Keys.X)) {
                    level.setMode(0);
                    screenManager.setActiveScreen(1);
                }
            } else if (active.getName() == "Telekinesis-Move") {
                playerManager.updateManaDrainRate();
                if (playerManager.getManaDrainRate() == 5) {
                    playerManager.depleteMana(1);
                }
                if (currentKeyState.IsKeyDown(Keys.W)) {
                    selectedObject.setDirection(Direction.North);
                    selectedObject.setDestination(new Vector2(selectedObject.getLocation().X, selectedObject.getLocation().Y - velocity));
                    if (selectedObject.getDestination().Y > 0 && collisionManager.isValid(selectedObject)) {
                        selectedObject.deriveY(-velocity);
                        if (playerManager.getManaDrainRate() == 5) {
                            playerManager.depleteMana(2);
                        }
                    }
                } else if (currentKeyState.IsKeyDown(Keys.S)) {
                    selectedObject.setDirection(Direction.South);
                    selectedObject.setDestination(new Vector2(selectedObject.getLocation().X, selectedObject.getLocation().Y + velocity));
                    if (selectedObject.getDestination().Y < midY * 2 && collisionManager.isValid(selectedObject)) {
                        selectedObject.deriveY(velocity);
                        if (playerManager.getManaDrainRate() == 5) {
                            playerManager.depleteMana(2);
                        }
                    }
                } else if (currentKeyState.IsKeyDown(Keys.A)) {
                    selectedObject.setDirection(Direction.West);
                    selectedObject.setDestination(new Vector2(selectedObject.getLocation().X - velocity, selectedObject.getLocation().Y));
                    if (selectedObject.getDestination().X > 0 && collisionManager.isValid(selectedObject)) {
                        selectedObject.deriveX(-velocity);
                        if (playerManager.getManaDrainRate() == 5) {
                            playerManager.depleteMana(2);
                        }
                    }
                } else if (currentKeyState.IsKeyDown(Keys.D)) {
                    selectedObject.setDirection(Direction.East);
                    selectedObject.setDestination(new Vector2(selectedObject.getLocation().X + velocity, selectedObject.getLocation().Y));
                    if (selectedObject.getDestination().X < midX * 2 && collisionManager.isValid(selectedObject)) {
                        selectedObject.deriveX(velocity);
                        if (playerManager.getManaDrainRate() == 5) {
                            playerManager.depleteMana(2);
                        }
                    }
                } else {
                    selectedObject.setDestination(selectedObject.getLocation());
                }
                if ((moving || (lastKeyState.IsKeyDown(Keys.Space) && currentKeyState.IsKeyDown(Keys.Space))) && playerManager.getMana() > 0) {
                    moving = true;
                    if (selectedObject.getDirection() == Direction.North) {
                        selectedObject.setDestination(new Vector2(selectedObject.getLocation().X, selectedObject.getLocation().Y - velocity));
                        if (selectedObject.getDestination().Y > 0 && collisionManager.isValid(selectedObject)) {
                            selectedObject.deriveY(-velocity);
                        } else {
                            moving = false;
                        }
                    } else if (selectedObject.getDirection() == Direction.South) {
                        if (selectedObject.getDestination().Y < midY * 2 && collisionManager.isValid(selectedObject)) {
                            selectedObject.deriveY(velocity);
                        } else {
                            moving = false;
                        }
                    } else if (selectedObject.getDirection() == Direction.West) {
                        if (selectedObject.getDestination().X > 0 && collisionManager.isValid(selectedObject)) {
                            selectedObject.deriveX(-velocity);
                        } else {
                            moving = false;
                        }
                    } else {
                        if (selectedObject.getDestination().X < midX * 2 && collisionManager.isValid(selectedObject)) {
                            selectedObject.deriveX(velocity);
                        } else {
                            moving = false;
                        }
                    }
                    if (playerManager.getManaDrainRate() == 5) {
                        playerManager.depleteMana(1);
                    }
                    if (!moving) {
                        selectedObject.setSelected(false);
                        selectedObject = null;
                        level.setMode(0);
                        screenManager.setActiveScreen(1);
                    }
                } else if ((lastKeyState.IsKeyDown(Keys.X) && currentKeyState.IsKeyUp(Keys.X)) || playerManager.getMana() == 0) {
                    selectedObject.setSelected(false);
                    selectedObject = null;
                    level.setMode(0);
                    screenManager.setActiveScreen(1);
                }
            } else if (active.getName() == "Menu") {
                lastState = state;
                state = Mouse.GetState().LeftButton;
                if (lastState == ButtonState.Pressed && state == ButtonState.Released) {
                    pauseMenu.reactToMouseClick();
                } else if (lastKeyState.IsKeyDown(Keys.M) && currentKeyState.IsKeyUp(Keys.M)) {
                    screenManager.setActiveScreen(1);
                    pauseMenu.setActive(false);
                    level.setActive(true);
                }
            }
        }
    }
}