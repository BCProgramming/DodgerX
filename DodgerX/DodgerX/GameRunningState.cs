using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using bcHighScores;

namespace DodgerX
{
    public class HighScoreListState : iGameState
    {
        String drawlisting = "";
        public static SpriteFont HighScoreFont = null;
        public Cue MusicTrack { get { return DodgerX.soundBank.GetCue("scoretheme"); } }
        public static void LoadContent(DodgerX gameobject)
        {

            HighScoreFont = gameobject.Content.Load<SpriteFont>(@"Fonts\monospace");
        }
        public void Draw(DodgerX gameobject, GameTime gameTime)
        {
            SpriteFont usehsfont = HighScoreFont;
            Vector2 stringsize = usehsfont.MeasureString(drawlisting);


            Vector2 DrawPosition = new Vector2(gameobject.Window.ClientBounds.Width/2-(stringsize.X/2),
                                            gameobject.Window.ClientBounds.Height/2-stringsize.Y/2);

            Vector2 ShadowPos = DrawPosition + new Vector2(2);
            gameobject.spriteBatch.DrawString(usehsfont, drawlisting, ShadowPos, Color.Gray);
            gameobject.spriteBatch.DrawString(usehsfont, drawlisting, DrawPosition, Color.Black);




        }

        public void Update(DodgerX gameobject, GameTime gameTime)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {

                gameobject.CurrentState = MenuState.getMainMenu(gameobject);

            }

            //throw new NotImplementedException();
            if (drawlisting == "")
            {
                StringBuilder buildlisting = new StringBuilder();

                int numenumerated = 0;

                int maxlength = 0;
                buildlisting.Append("  -------TOP TEN-------\n");
                
               /* for(int currshow=1;currshow < 5;currshow++)
                {

                    var iterate = gameobject.HighScoreData.Scores.ElementAt(currshow - 1).Value;
                    String built=iterate.Name + iterate.Score.ToString();
                    if (built.Length > maxlength)
                        maxlength = built.Length;


                }*/
                maxlength = gameobject.HighScoreData.Scores.Max((t) => (t.Value.Name.Length+t.Value.Score.ToString().Length));
                for(int currshow=1;currshow < 10;currshow++)
                {
                    var scoreentry = gameobject.HighScoreData.Scores.ElementAt(gameobject.HighScoreData.Scores.Count - (currshow)).Value;
                    int useamount = 3+ (maxlength) - (scoreentry.Name.Length);
                    String dotstr = new string(Enumerable.Repeat('.', useamount).ToArray());

                    if (numenumerated == 11) break;
                    string buildline = (numenumerated+1).ToString() + ":" + scoreentry.Name + dotstr + scoreentry.Score.ToString();
                    //buildlisting.AppendLine(numenumerated.ToString() + ":\t" + scoreentry.Name + "\t\t" + scoreentry.Score.ToString());
                    buildlisting.AppendLine(buildline);

                    numenumerated++;
                }
                buildlisting.AppendLine("\nPress <space>");

                drawlisting = buildlisting.ToString();


            }
        }
    }
      
    public class GameRunningState:iGameState,iGameRunner 
    {
        public Cue MusicTrack { get { return DodgerX.soundBank.GetCue("basestompx"); } }
        public Color[] LevelColors = new Color[] { Color.WhiteSmoke, Color.Aqua, Color.AliceBlue, Color.GreenYellow, Color.Honeydew, Color.IndianRed, Color.Khaki, Color.LemonChiffon, Color.LightGreen, Color.Olive, Color.Red };
        //public Player Playerobj;
        public static readonly TimeSpan Minimumspan = new TimeSpan(0, 0, 0, 0, 100);
        public int numgens = 0; //number of generations this cycle.
        public int ReleaseCount = 1; //number of balls to release at a time.
        public static Cue TrackCue = null;
        public int LevelNumber = 0;
        //public List<Particle> Particles = new List<Particle>(); 
        //public List<AttackingObject> Attackers = new List<AttackingObject>();


        private  GameRunnerData _grd = new GameRunnerData();

        public GameRunnerData grd
        {
            get { return _grd; }
            set { _grd = value; }
        }

        public long CurrentScore = 0;

        TimeSpan? NextTimeGen = null;
        public void Draw(DodgerX gameobject, GameTime gameTime)
        {
            Vector2 MousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            gameobject.GraphicsDevice.Clear(LevelColors[LevelNumber % LevelColors.Length]);
            if (grd.PlayerObject == null) return;

            grd.Draw(gameobject, gameTime);
              
              String ScoreString = "Score:" + CurrentScore.ToString();

             
              Vector2 ScoreSize = gameobject.StatusFont.MeasureString(ScoreString);

             
              gameobject.spriteBatch.DrawString(gameobject.StatusFont, ScoreString, new Vector2(gameobject.Window.ClientBounds.Width - ScoreSize.X, 0), Color.Black);
              gameobject.spriteBatch.Draw(MenuState.Cursor, MousePos, Color.White);


              String LevelString = "Level:" + LevelNumber.ToString();
              Vector2 LevelSize = gameobject.StatusFont.MeasureString(LevelString);
              gameobject.spriteBatch.DrawString(gameobject.StatusFont, LevelString, new Vector2(0, 0), Color.Black);

              

        }
        TimeSpan TotalRunningTime = new TimeSpan();
        float CurrPitch = 1;
        bool waspausedown = false;
        const float MaximumSpeed = 5;
        const float HalfMaxSpeed = MaximumSpeed / 2;
        public void Update(DodgerX gameobject, GameTime gameTime)
        {
            TotalRunningTime += gameTime.ElapsedGameTime;
          
            
                
            if(NextTimeGen ==null) NextTimeGen = TotalRunningTime + new TimeSpan(0,0,0,3);


            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                gameobject.Exit();



            if (grd.PlayerObject == null)
            {
                grd.Attackers.Clear();
                grd.PlayerObject = new Player(gameobject, 8);
                grd.PlayerObject.Position = new Vector2(gameobject.Window.ClientBounds.Width / 2, gameobject.Window.ClientBounds.Height / 2);
                grd.PlayerObject.PlayerDeath += new Action<DodgerX, Player>(PlayerObject_PlayerDeath);
            }

            grd.Update(gameobject, gameTime);




            if (TotalRunningTime > NextTimeGen.Value && !grd.Attackers.Any((w)=>w is HealyBall ))
            {
                CurrentScore += ((numgens + 1) * ReleaseCount) * grd.PlayerObject.NumItemsLeft();
                numgens++;
                //release 5+numgens healyOrbs.
           


                TimeSpan addthis = new TimeSpan(new TimeSpan(0, 0, 0, 2,500).Ticks - (numgens * 1000000));
                if (addthis < Minimumspan)
                {
                    numgens = 0;
                    ReleaseCount++;
                    LevelNumber++;
                    for (int addorb = 0; addorb < 8 + numgens; addorb++)
                    {
                        Vector2 chosenLocation = new Vector2((float)(gameobject.Window.ClientBounds.Width * DodgerX.rgen.NextDouble()),
                                                         (float)(gameobject.Window.ClientBounds.Y * DodgerX.rgen.NextDouble()));
                        Vector2 chosenspeed = new Vector2(((float)DodgerX.rgen.NextDouble() * MaximumSpeed) - HalfMaxSpeed,
                                                          ((float)DodgerX.rgen.NextDouble() * MaximumSpeed) - HalfMaxSpeed);
                        
                        //ao.SetRandomStartPosition(gameobject, MaximumSpeed);
                        HealyBall hb = new HealyBall(chosenLocation, chosenspeed*(3/4), gameobject.healyTexture);
                        hb.SetRandomStartPosition(gameobject, MaximumSpeed);
                        grd.Attackers.Add(hb);
                    }
                }
                Debug.Print("adding " + addthis.ToString() + " to nexttimegen...");
                NextTimeGen += addthis;

                for (int i = 0; i < ReleaseCount; i++)
                {

                    Vector2 chosenLocation = new Vector2((float) (gameobject.Window.ClientBounds.Width*DodgerX.rgen.NextDouble()),
                                                         (float) (gameobject.Window.ClientBounds.Y*DodgerX.rgen.NextDouble()));
                    Vector2 chosenspeed = new Vector2(((float) DodgerX.rgen.NextDouble()*MaximumSpeed) - HalfMaxSpeed,
                                                      ((float) DodgerX.rgen.NextDouble()*MaximumSpeed) - HalfMaxSpeed);
                    AttackingObject ao = new AttackingObject(chosenLocation, chosenspeed, gameobject.attackerTexture);
                    ao.SetRandomStartPosition(gameobject, 4);
                    grd.Attackers.Add(ao);
                    //SetRandomStartPosition
                    Debug.Print("attacker added at " + chosenLocation + " with speed " + chosenspeed);
                }
            }
        
        }
        [DllImport("Advapi32.dll", EntryPoint = "GetUserName",
           ExactSpelling = false, SetLastError = true)]
        static extern bool GetUserName(
    [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer,
    [MarshalAs(UnmanagedType.LPArray)] Int32[] nSize);

        private String GetUsername()
        {
            byte[] str = new byte[256];
            Int32[] len = new Int32[1];
            len[0] = 256;
            GetUserName(str, len);
            return System.Text.Encoding.ASCII.GetString(str).Replace('\0', ' ').Trim();
        }
        void PlayerObject_PlayerDeath(DodgerX arg1, Player arg2)
        {
            int pos = 0;
            if (-1 < (pos = arg1.HighScoreData.Eligible((int)CurrentScore)))
            {
                ValueEntryState ves = new ValueEntryState("High Score (#" + (pos+1).ToString() + ")", GetUsername(), "YOu got a High Score! Please Enter your name.", HighScoreEntered);
                arg1.CurrentState = ves;
            }
        }
        private void HighScoreEntered(DodgerX gameobj,String enteredname)
        {
            gameobj.HighScoreData.Submit(enteredname, (int)CurrentScore);
            gameobj.CurrentState = new HighScoreListState();

        }
        public static void LoadContent(DodgerX gameobject)
        {
           // throw new NotImplementedException();
            TrackCue = DodgerX.soundBank.GetCue("basestompx");
        }

        public void UnloadContent(DodgerX gameobject)
        {
           // throw new NotImplementedException();
        }
    }
}

