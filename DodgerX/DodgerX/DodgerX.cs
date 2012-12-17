using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using bcHighScores;

namespace DodgerX
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DodgerX : Microsoft.Xna.Framework.Game
    {
        
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public SpriteFont DefaultFont;
        public SpriteFont DefaultFont_Embiggened;
        public SpriteFont StatusFont;
        public SpriteFont MessageTitleFont;
        public static AudioEngine audioEngine;
        public static SoundBank soundBank;
        public static WaveBank waveBank;
        //public Cue TrackCue;
        public Texture2D LogoImage;
        public Texture2D LogoImage2;
        public Texture2D LogoImage3;
        public Texture2D SpeckImage;
        public Texture2D RedSpeck;
        public Texture2D GreenSpeck;
        public Texture2D YellowSpeck;
        public Cue GrenEffect;
        public bcHighScores.HighScoreManager hsm = null;
        public LocalHighScores HighScoreData = null;
        public Type[] StateTypes = new Type[] { typeof(IntroState), typeof(GameRunningState),typeof(MenuState),typeof(ValueEntryState),typeof(HighScoreListState) };
        
        public Texture2D attackerTexture;
        public Texture2D healyTexture;
        public Texture2D redcircletexture;
        public Texture2D greencircletexture;
        public Texture2D yellowcircletexture;





        public iGameState CurrentState = null;

        public static void DrawVersion(DodgerX gameobj)
        {

            //draw version using statusfont.
            //upper-right...
            SpriteFont sf = gameobj.DefaultFont;

            Assembly us = Assembly.GetExecutingAssembly();
            String verinfo = us.GetName().Name + " V." + us.GetName().Version;
            Vector2 sizedvalue = sf.MeasureString(verinfo);
            gameobj.spriteBatch.DrawString(sf, verinfo, new Vector2(gameobj.Window.ClientBounds.Width - sizedvalue.X, 0), Color.Black);




        }
        public static double GetAngle(Vector2 PointA, Vector2 PointB)
        {

         
        
            return   Math.Atan2(PointB.Y - PointA.Y, PointB.X - PointA.X);
           

        

        
        }
        public DodgerX()
        {
            ActiveInstance = this;
            //Components.Add(new GamerServicesComponent(this));
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
            //graphics.ToggleFullScreen();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
           // EventInput.EventInput.Initialize(Window);
            EventInput.EventInput.Initialize(Window);
            base.Initialize();
        }
        private void CallLoadContent(Type ontype)
        {
            MethodInfo foundmethod = null;
            foreach (MethodInfo getmeth in ontype.GetMethods())
                if (getmeth.Name == "LoadContent")
                    foundmethod = getmeth;

            //ontype.InvokeMember("LoadContent", BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { this });
            if (foundmethod == null) return;
            foundmethod.Invoke(null, BindingFlags.Static, null, new object[] { this }, Thread.CurrentThread.CurrentCulture); 
        }
        protected override void OnExiting(object sender, EventArgs args)
        {
            hsm.Save();


            base.OnExiting(sender, args);
        }
        public bool ContentLoaded = false;
        
        public static Cue IntroMusic { get { return soundBank.GetCue("intro"); } }

        public static DodgerX ActiveInstance = null;


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            attackerTexture = Content.Load<Texture2D>(@"Images\bluecircle");
            healyTexture = Content.Load<Texture2D>(@"Images\healyOrb");
            DefaultFont = Content.Load<SpriteFont>(@"Fonts\BasicFont");
            DefaultFont_Embiggened = Content.Load<SpriteFont>(@"Fonts\BasicFontLarge");
            StatusFont = Content.Load<SpriteFont>(@"Fonts\StatusbarFont");
            MessageTitleFont = Content.Load<SpriteFont>(@"Fonts\dialogtitle");
            redcircletexture = Content.Load<Texture2D>(@"Images\redcircle");
            greencircletexture = Content.Load<Texture2D>(@"Images\greencircle");
            yellowcircletexture = Content.Load<Texture2D>(@"Images\yellowcircle");
            SpeckImage = Content.Load<Texture2D>(@"Images\whitespeck");
            RedSpeck = Content.Load<Texture2D>(@"Images\redspeck");
            GreenSpeck = Content.Load<Texture2D>(@"Images\greenspeck");
            YellowSpeck = Content.Load<Texture2D>(@"Images\yellowspeck");
            audioEngine = new AudioEngine(@"Content\Audio\GameSounds.xgs");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            LogoImage = Content.Load<Texture2D>(@"Images\bclogo");
            LogoImage2 = Content.Load<Texture2D>(@"Images\basecampBCDodgerdisclaim");
            LogoImage3 = Content.Load<Texture2D>(@"Images\toomanylogos");
            
            GrenEffect = soundBank.GetCue("gren");
            
            
            foreach (Type iteratetype in StateTypes)
            {

                CallLoadContent(iteratetype);

            }

            String usehighscorefile = GetHighScoresFile();
            if (File.Exists(usehighscorefile))
                hsm = HighScoreManager.FromFile(usehighscorefile);
            else
                hsm = new HighScoreManager(usehighscorefile);
            HighScoreData = hsm["BCDodger"];
            ContentLoaded = true;
            //TrackCue.Play();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            hsm.Save(GetHighScoresFile());
        }
        public static Random rgen = new Random();

        private void RearrangeItems(DodgerX gameobject, Texture2D imagelogo, MenuStateItem[] loopitem)
        {
            Vector2 currentlocation = new Vector2(0, imagelogo.Height + 8);
            
            for (int i = 0; i < loopitem.Length; i++)
            {
                Vector2 measuredsize = loopitem[i].sf.MeasureString(loopitem[i].Caption);
                loopitem[i].Location = new Vector2(gameobject.Window.ClientBounds.Width / 2 - measuredsize.X / 2, currentlocation.Y);
                currentlocation.Y += measuredsize.Y + 5;



            }


        }
        public String GetAppDataFolder()
        {
            String MakeFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //append BCDodgerX
            MakeFolder = Path.Combine(MakeFolder, "BCDodgerX");
            if (!Directory.Exists(MakeFolder))
                Directory.CreateDirectory(MakeFolder);

            return MakeFolder;



        }
        public String GetHighScoresFile()
        {

            return Path.Combine(GetAppDataFolder(), "Highscores.dat");



        }
        private MenuState getoptionsstate(iGameState returnstate)
        {
            List<MenuStateItem> createmenu = new List<MenuStateItem>();

            createmenu.Add(new MenuStateItem("Return", returnstate, new Vector2(0, 0), null));


            return new MenuState(this, createmenu.ToArray());
            


        }
        public KeyboardState PrevKeyboardState;
        public MouseState PrevMouseState;
        private Cue PlayingCue = null;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (CurrentState != null)
            {
                
                if ((PlayingCue == null) || ((CurrentState.MusicTrack!=null) && PlayingCue.Name != CurrentState.MusicTrack.Name))
                {
                    if (PlayingCue != null) PlayingCue.Stop(AudioStopOptions.AsAuthored);
                    PlayingCue = CurrentState.MusicTrack;
                    if (PlayingCue != null) try { PlayingCue.Play(); }
                        catch { }

                }

            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.Pause) && !PrevKeyboardState.IsKeyDown(Keys.Pause))
            {
                if (CurrentState is GameRunningState)
                {
                    //GameRunningState.TrackCue.Pause();
                    CurrentState = MenuState.GetPauseState(this, CurrentState);
                }

            }


            audioEngine.Update();
            if (!ContentLoaded) return;
            if (CurrentState == null)
            {
                MenuState ms = MenuState.getMainMenu(this);


                CurrentState = new IntroState(LogoImage2,new IntroState(LogoImage,new IntroState(LogoImage3, ms)));
                

                
            }
            //audioEngine.Update();
            CurrentState.Update(this, gameTime);

            base.Update(gameTime);
            PrevKeyboardState = Keyboard.GetState();
            PrevMouseState = Mouse.GetState();
        }
       
    
        public float SpeedMultiplier = 1f;
        public static readonly float DesiredFPS = 60;
        public double CurrentFPS;

        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (CurrentState == null) return;
            CurrentFPS = 1/gameTime.ElapsedGameTime.TotalSeconds;
            SpeedMultiplier = (float)(CurrentFPS / DesiredFPS);
            
            spriteBatch.Begin();

            CurrentState.Draw(this, gameTime);
          
            spriteBatch.End();
            // TODO: Add your drawing code here
            

            base.Draw(gameTime);
        }
    }
}
