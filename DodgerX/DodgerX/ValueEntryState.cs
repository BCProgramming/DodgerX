using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DodgerX
{
    class ValueEntryState:iGameState
    {
        public Cue MusicTrack { get { return DodgerX.soundBank.GetCue("scoretheme"); } }
        /// <summary>
        /// used to validate the given entry.
        /// </summary>
        /// <param name="EnteredValue"></param>
        /// <returns>true to allow entry; false to reject and remain in valueentry mode.</returns>
        public delegate bool ValidateEntry(String EnteredValue);
        /// <summary>
        /// routine called after a entry is entered and validated.
        /// </summary>
        /// <param name="Entry"></param>
        public delegate void EntryEntered(DodgerX gameobj,String Entry);

        private String TitleCaption = "Value Entry";
        private String CurrentEntry = "";
        private String DialogText="Enter a Value";
        //private ValidateEntry _ValidationRoutine;
        private EntryEntered _EntryRoutine;


        //public ValidateEntry ValidationRoutine { get { return _ValidationRoutine; } set { _ValidationRoutine = value; } }
        public EntryEntered EntryRoutine { get { return _EntryRoutine; } set { _EntryRoutine = value; } }

        public ValueEntryState(String pTitleCaption, String InitialValue,String pdialogText, EntryEntered pEntryRoutine)
        {
            TitleCaption = pTitleCaption;
            
            _EntryRoutine = pEntryRoutine;
            CurrentEntry = InitialValue;
            DialogText=pdialogText;
        }

        public static char? KeyToChar(Keys keyconvert)
        {

            if (((int)keyconvert > (int)Keys.A) &&
                (int)keyconvert < (int)Keys.Z)
            {

                return (char)(65 + ((int)keyconvert - 65));

            }


            return null;
        }
        public static Texture2D whitepixel;
        public static void LoadContent(DodgerX gameobject)
        {

            whitepixel = gameobject.Content.Load<Texture2D>(@"images\whitepixel");

        }
        public void Draw(DodgerX gameobject, GameTime gameTime)
        {

            //rectangle for the "messagebox"
            var cb = gameobject.Window.ClientBounds;
            SpriteFont titlefont = gameobject.MessageTitleFont;
            SpriteFont DefFont = gameobject.DefaultFont_Embiggened;
            //(width/8,Height/4)-(width*(7/8),Height*3/4)
            Vector2 measuredtitlesize = gameobject.MessageTitleFont.MeasureString(TitleCaption);
            Vector2 measuredtextsize = gameobject.DefaultFont.MeasureString(CurrentEntry);


            String measureme = "################################";
            //draw a white box near the center...
            //RectangleF Inputboxcoordsf = new RectangleF(PicGame.ClientRectangle.Width/4,PicGame.ClientRectangle.Height/4,PicGame.ClientRectangle.Width/2,PicGame.ClientRectangle.Height/2);

            Vector2 InputboxSize = titlefont.MeasureString(measureme);
                
            

            Rectangle cli = gameobject.Window.ClientBounds;
            Rectangle IBox = new Rectangle((int)((cli.Width / 2) - InputboxSize.X / 2), (int)((cli.Height / 2f) - (InputboxSize.Y / 2)), (int)InputboxSize.X,(int)InputboxSize.Y);

            Rectangle titleBox = IBox;
            titleBox.Offset(0, -IBox.Height);

            String VInput = CurrentEntry;
         
            //g.DrawString(CurrentEntry, defFont, new SolidBrush(Color.Black), IBox);



            gameobject.spriteBatch.Draw(whitepixel, IBox, Color.White);

            gameobject.spriteBatch.Draw(whitepixel, titleBox, Color.Yellow);
            gameobject.spriteBatch.DrawString(titlefont, TitleCaption, new Vector2(titleBox.X, titleBox.Y), Color.Black);
            gameobject.spriteBatch.DrawString(DefFont, CurrentEntry, new Vector2(IBox.X, IBox.Y), Color.Black);
            //gameobject.MessageTitleFont 


        }
        KeyboardState? previouskeystate = null;

       
        bool isshowingInput = false;
     
        private DodgerX gameobj = null;
        public void Update(DodgerX gameobject, GameTime gameTime)
        {
            if (!isshowingInput)
            {
                gameobj = gameobject;
                isshowingInput = true;
                EventInput.EventInput.CharEntered += new EventInput.CharEnteredHandler(EventInput_CharEntered);
                EventInput.EventInput.KeyDown += new EventInput.KeyEventHandler(EventInput_KeyDown);
                EventInput.EventInput.KeyUp += new EventInput.KeyEventHandler(EventInput_KeyUp);
            }
        }

        void EventInput_KeyUp(object sender, EventInput.KeyEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void EventInput_KeyDown(object sender, EventInput.KeyEventArgs e)
        {
            
           // throw new NotImplementedException();
        }

        void EventInput_CharEntered(object sender, EventInput.CharacterEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Character == '\b')
            {
                if(CurrentEntry.Length>0)
                    CurrentEntry = CurrentEntry.Remove(CurrentEntry.Length - 1);
            }
            else if (e.Character == '\xD')
            {
                
                EntryRoutine(gameobj, CurrentEntry);

            }
            else
            {
                Debug.Print("character is " + (int)e.Character);

                CurrentEntry += e.Character;
            }
        }
    }
}
