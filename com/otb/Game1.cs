﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using System.Collections.Generic;

namespace OutsideTheBox {

    /// <summary>
    /// Class which handles the whole game
    /// </summary>

    public class Game1 : Game {

        private readonly GraphicsDeviceManager graphics;

        private SpriteBatch spriteBatch;
        private Player player;
        private Level level;
        private List<Level> levels;
        private PlayerManager playerManager;
        private InputManager inputManager;
        private Menu pauseMenu;
        private Texture2D cursor;
        private Target target;
        private MouseState mouse;
        private Texture2D startMenu;

        private Texture2D pixel;

        private Song factorySong;
        private SoundEffect effect;
        public SoundEffect boltSound;

        private int midX;
        private int midY;
        private int width;
        private int height;

        private int levelIndex;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Returns the center x coordinate of the game
        /// </summary>
        /// <returns>Returns the center x coordinate of the game, with respect to the player</returns>
        public int getMidX() {
            return midX;
        }

        /// <summary>
        /// Returns the center y coordinate of the game
        /// </summary>
        /// <returns>Returns the center y coordinate of the game, with respect to the player</returns>
        public int getMidY() {
            return midY;
        }

        /// <summary>
        /// Returns the width of the game
        /// </summary>
        /// <returns>Returns the width of the game</returns>
        public int getWidth() {
            return width;
        }

        /// <summary>
        /// Returns the height of the game
        /// </summary>
        /// <returns>Returns the height of the game</returns>
        public int getHeight() {
            return height;
        }

        /// <summary>
        /// Returns the mouse state of the game
        /// </summary>
        /// <returns>Returns the mouse state of the game</returns>
        public MouseState getMouse() {
            return mouse;
        }

        /// <summary>
        /// Returns an instance of the current level
        /// </summary>
        /// <returns>Returns an instance of the current level</returns>
        public Level getLevel() {
            return level;
        }

        /// <summary>
        /// Sets the game's level
        /// </summary>
        /// <param name="level">The level to set</param>
        public void setLevel(Level level) {
            this.level = level;
        }

        /// <summary>
        /// Returns the level at the specified index
        /// </summary>
        /// <param name="index">The index to retrieve</param>
        /// <returns>Returns the level at the specified index</returns>
        public Level getLevel(int index) {
            return levels[index];
        }

        /// <summary>
        /// Returns the level list
        /// </summary>
        /// <returns>Returns the list of levels</returns>
        public List<Level> getLevels() {
            return levels;
        }

        /// <summary>
        /// Returns an instance of the player
        /// </summary>
        /// <returns>Returns an instance of the player</returns>
        public Player getPlayer() {
            return player;
        }

        /// <summary>
        /// Returns the input manager for the game
        /// </summary>
        /// <returns>Returns the input manager</returns>
        public InputManager getInputManager() {
            return inputManager;
        }

        /// <summary>
        /// Sets the level index for the game
        /// </summary>
        /// <param name="index">The index to be set</param>
        public void setLevel(int index) {
            levelIndex = index;
        }

        /// <summary>
        /// Returns the level index for the game
        /// </summary>
        /// <returns>Returns the level index</returns>
        public int getLevelIndex() {
            return levelIndex;
        }

        /// <summary>
        /// Adds a projectile to the game from an NPC
        /// </summary>
        /// <param name="projectile">The projectile to be added</param>
        public void addProjectile(Projectile projectile) {
            level.addProjectile(projectile);
        }

        /// <summary>
        /// Draws an outline of the bounds of a specified area
        /// </summary>
        /// <param name="batch">The SpriteBatch to draw with</param>
        /// <param name="area">The area to be drawn</param>
        public void outline(SpriteBatch batch, Rectangle area) {
            batch.Draw(pixel, new Rectangle(area.X, area.Y, area.Width, 1), Color.Green);
            batch.Draw(pixel, new Rectangle(area.X, area.Y, 1, area.Height), Color.Green);
            batch.Draw(pixel, new Rectangle(area.X + area.Width - 1, area.Y, 1, area.Height), Color.Green);
            batch.Draw(pixel, new Rectangle(area.X, area.Y + area.Height - 1, area.Width, 1), Color.Green);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        //
        protected override void Initialize() {
            base.Initialize();
            //graphics.PreferredBackBufferWidth = x;
            //graphics.PreferredBackBufferHeight = y;
            //graphics.ApplyChanges();
            Window.Title = "Outside The Box";
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            base.LoadContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            width = 800;
            height = 480;

            factorySong = Content.Load<Song>("audio/songs/Factory");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(factorySong);

            boltSound = Content.Load<SoundEffect>("audio/Sound Effects/boltSound");
            startMenu = Content.Load<Texture2D>("menus/StartMenu");

            Texture2D playur = Content.Load<Texture2D>("sprites/entities/player/Standing1");
            Texture2D bullet = Content.Load<Texture2D>("sprites/projectiles/BulletOrb");
            Texture2D fireOrb = Content.Load<Texture2D>("sprites/projectiles/FireOrb");
            Texture2D iceOrb = Content.Load<Texture2D>("sprites/projectiles/IceOrb");
            Texture2D confusionOrb = Content.Load<Texture2D>("sprites/projectiles/ConfusionOrb");
            Texture2D lightningOrb= Content.Load<Texture2D>("sprites/projectiles/LightningOrb");
            Texture2D paralysisOrb = Content.Load<Texture2D>("sprites/projectiles/ParalysisOrb");
            Texture2D health = Content.Load<Texture2D>("ui/HealthBarTexture");
            Texture2D back = Content.Load<Texture2D>("ui/BackBarTexture");
            Texture2D mana = Content.Load<Texture2D>("ui/ManaBarTexture");
            midX = (graphics.PreferredBackBufferWidth - playur.Width) / 2;
            midY = (graphics.PreferredBackBufferHeight - playur.Height) / 2;
            player = new Player(playur, Vector2.Zero, Direction.South, 100, 50, 0, 3);
            player.setProjectile(new Projectile(player, bullet, 5, 250, boltSound));
            playerManager = new PlayerManager(player, Content, new DisplayBar(health, new Vector2(20F, 20F), Color.Red, back), new DisplayBar(mana, new Vector2(20F, 50F), Color.Blue, back));
            player.loadTextures(Content);

            Texture2D male1 = Content.Load<Texture2D>("sprites/entities/npcs/NormieMaleStand1");
            Texture2D male2 = Content.Load<Texture2D>("sprites/entities/npcs/NormieMaleStand2");
            Npc npc = new Npc(this, male1, new Vector2(midX + 148F, midY + 135F), Direction.East, new NpcDefinition("Normie", new string[0], new int[0]), 150, 0x5);
            Npc npc2 = new Npc(this, male1, new Vector2(midX + 350F, midY + 100F), Direction.East, new NpcDefinition("Normie2", new string[0], new int[0]), 150, 0x5);
            Npc npc3 = new Npc(this, male2, new Vector2(midX + 240F, midY + 123F), Direction.North, new NpcDefinition("Normie3", new string[0], new int[0]), 150, 0x5);
            npc2.setProjectile(new Projectile(npc2, bullet, 10, 500, boltSound));
            npc3.setProjectile(new Projectile(npc3, bullet, 10, 500, boltSound));
            Npc npc4 = new Npc(this, male2, new Vector2(50F, 50F), Direction.West, new NpcDefinition("Normie4", new string[0], new int[0]), 150, 0x5);
            Npc npc5 = new Npc(this, male2, new Vector2(150F, 130F), Direction.South, new NpcDefinition("Normie5", new string[0], new int[0]), 150, 0x5);
            Npc npc6 = new Npc(this, male2, new Vector2(400F, 200F), Direction.South, new NpcDefinition("Normie6", new string[0], new int[0]), 150, 0x5);
            npc6.setProjectile(new Projectile(npc6, bullet, 10, 500, boltSound));

            Texture2D box = Content.Load<Texture2D>("sprites/objects/CardboardBox");
            GameObject obj = new GameObject(box, new Vector2(midX + 20F, midY + 65F), true);
            GameObject obj2 = new GameObject(box, new Vector2(midX + 20F, midY + 205F), true);

            Texture2D door = Content.Load<Texture2D>("sprites/objects/DoorTexture");
            Door door1 = new Door(door, null, new Vector2(width - 10F, height - 89F), Direction.East, false, true, 10, 64);
            Door door2 = new Door(door, null, new Vector2(0F, height - 89F), Direction.West, false, false, 10, 64);

            Texture2D pressButton = Content.Load<Texture2D>("sprites/objects/PressButton");

            Texture2D bronze = Content.Load<Texture2D>("sprites/objects/BronzeCoinFront");
            Texture2D silver = Content.Load<Texture2D>("sprites/objects/SilverCoinFront");
            Texture2D gold = Content.Load<Texture2D>("sprites/objects/GoldCoinFront");
            Texture2D side1 = Content.Load<Texture2D>("sprites/objects/BronzeCoinSide");
            Texture2D side2 = Content.Load<Texture2D>("sprites/objects/SilverCoinSide");
            Texture2D side3 = Content.Load<Texture2D>("sprites/objects/GoldCoinSide");
            Token token1 = new Token(bronze, side1, new Vector2(midX + 230F, midY + 95F), TokenType.Bronze);
            Token token2 = new Token(silver, side2, new Vector2(midX + 230F, midY + 225F), TokenType.Silver);
            Token token3 = new Token(gold, side3, new Vector2(200F, 200F), TokenType.Gold);
            Token token4 = new Token(gold, side3, new Vector2(200F, 200F), TokenType.Gold);

            Texture2D wall = Content.Load<Texture2D>("sprites/objects/WallTexture");
            Wall wall1 = new Wall(wall, null, new Vector2(120F, 250F), Direction.East, false, false, 120, 20);
            Wall wall2 = new Wall(wall, null, new Vector2(120F, 350F), Direction.East, false, false, 120, 20);
            Wall wall3 = new Wall(wall, null, new Vector2(100F, 250F), Direction.East, false, false, 20, 120);
            Wall wall4 = new Wall(wall, null, new Vector2(650F, 100F), Direction.East, false, false, 120, 20);
            Wall wall5 = new Wall(wall, null, new Vector2(650F, 200F), Direction.East, false, false, 120, 20);
            Wall wall6 = new Wall(wall, null, new Vector2(770F, 100F), Direction.East, false, false, 20, 120);

            Texture2D l1 = Content.Load<Texture2D>("sprites/levels/Level1");
            Texture2D l2 = Content.Load<Texture2D>("sprites/levels/Level1Map");
            Texture2D bubble = Content.Load<Texture2D>("sprites/thoughts/PassBubble1");
            Level level1 = new Level(this, player, l1, new Npc[] { npc, npc2, npc5 }, new GameObject[] { obj2, obj }, new DisplayBar[] { playerManager.getHealthBar(), playerManager.getManaBar() }, new Token[] { token1, token2, token3 }, new Door[] { door1 }, new Wall[0], new ThoughtBubble[0], new PressButton[] { }, 1);
            Level level2 = new Level(this, player, l2, new Npc[] { npc3, npc4, npc6 }, new GameObject[0], new DisplayBar[] { playerManager.getHealthBar(), playerManager.getManaBar() }, new Token[] { token4 }, new Door[] { door2 }, new Wall[] { wall1, wall2, wall3, wall4, wall5, wall6 }, new ThoughtBubble[] { new ThoughtBubble(bubble, Vector2.Zero, npc3, false, false) }, new PressButton[] { }, 2);
            levels = new List<Level>();
            levels.Add(level1);
            levels.Add(level2);
            level = levels[0];
            levelIndex = 0;

            Texture2D button1 = Content.Load<Texture2D>("menus/assets/button_mind_read");
            Texture2D button2 = Content.Load<Texture2D>("menus/assets/button_clairvoyance");
            Texture2D button3 = Content.Load<Texture2D>("menus/assets/button_confusion");
            Texture2D button4 = Content.Load<Texture2D>("menus/assets/button_dash");
            Texture2D button5 = Content.Load<Texture2D>("menus/assets/button_slow_time");
            Texture2D button6 = Content.Load<Texture2D>("menus/assets/button_invisibility");
            Texture2D button7 = Content.Load<Texture2D>("menus/assets/button_fire_bolt");
            Texture2D button8 = Content.Load<Texture2D>("menus/assets/button_ice_bolt");
            Texture2D button9 = Content.Load<Texture2D>("menus/assets/button_lightning_bolt");
            Button[] menuButtons = { new Button(button1, new Vector2(270F, 140F)), new Button(button2, new Vector2(270F, 220F)),
                                       new Button(button3, new Vector2(270F, 310F)), new Button(button4, new Vector2(355F, 140F)),
                                       new Button(button5, new Vector2(355F, 220F)), new Button(button6, new Vector2(355F, 310F)),
                                       new Button(button7, new Vector2(445F, 140F)), new Button(button8, new Vector2(445F, 220F)),
                                       new Button(button9, new Vector2(445F, 310F)) };

            Texture2D pauseScreen = Content.Load<Texture2D>("menus/PausePlaceholderScreen");
            pauseMenu = new Menu(pauseScreen, menuButtons);

            Texture2D targ = Content.Load<Texture2D>("sprites/cursors/TargetingCursor");
            target = new Target(targ);

            Screen[] screens = { new Screen("Menu"), new Screen("Normal", true), new Screen("Telekinesis-Select"), new Screen("Telekinesis-Move"), new Screen("Start") };
            inputManager = new InputManager(this, player, level, pauseMenu, target, playerManager, screens, new MindRead(bubble));
            level.setInputManager(inputManager);
            pauseMenu.setInputManager(inputManager);
            inputManager.setDeathManager(new DeathManager(inputManager));

            cursor = Content.Load<Texture2D>("sprites/cursors/Cursor");

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });
            npc.setPath(new AIPath(npc, this, new int[] { midX - 100, midY - 100, midX + 100, midY + 135 }, new int[0], new Direction[] { Direction.West, Direction.North, Direction.East, Direction.South }));
            npc3.setPath(new AIPath(npc3, this, new int[] { midX - 100, midY - 100, midX + 100, midY + 150 }, new int[0], new Direction[] { Direction.West, Direction.North, Direction.East, Direction.South }));
            npc4.setPath(new AIPath(npc4, this, new int[] { 200, 60 }, new int[0], new Direction[] { Direction.East, Direction.West }));
            npc5.setPath(new AIPath(npc5, this, new int[] { 200, 150 }, new int[0], new Direction[] { Direction.East, Direction.West }));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            base.UnloadContent();
            spriteBatch.Dispose();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
            playerManager.updateHealthCooldown();
            inputManager.update(gameTime);
            if (level.isActive()) {
                level.updateProjectiles();
                level.updateNpcs(gameTime);
            }
            mouse = Mouse.GetState();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            level.draw(spriteBatch);
            if (pauseMenu.isActive()) {
                pauseMenu.draw(spriteBatch);
            }
            if (mouse != null) {
                if (level.getMode() < 1) {
                    spriteBatch.Draw(cursor, new Vector2(mouse.X, mouse.Y), Color.White);
                } else {
                    spriteBatch.Draw(target.getTexture(), new Vector2(mouse.X - (target.getTexture().Width / 2F), mouse.Y - (target.getTexture().Height / 2F)), Color.White);
                }
            }
            if (inputManager.getScreenManager().getActiveScreen().getName() == "Start") {
                spriteBatch.Draw(startMenu, new Vector2(-290F, -100F), Color.White);
            }
            spriteBatch.End();
        }
    }
}
