using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DodgerX
{
    class IntroState:iGameState  
    {
        public Cue MusicTrack { get { return DodgerX.IntroMusic; } }
        private enum LogoShowMode { 
            Logo_Fadein,
            Logo_Solid,
            Logo_Fadeout
        
        
        }
        private LogoShowMode mLogoMode = LogoShowMode.Logo_Fadein;
        TimeSpan? FadeinComplete = null;
        TimeSpan? FadeOutStart = null;
        TimeSpan? FirstCall = null;
        Texture2D LogoImage = null;
        float AlphaValue = 0;
        public TimeSpan MaxLogoShowTime = new TimeSpan(0, 0, 0, 5);
        public TimeSpan FadeTime = new TimeSpan(0, 0, 0, 2);
        private iGameState _NextState = null;


        public void Draw(DodgerX gameobject, GameTime gameTime)
        {

            Vector2 CalculatedDimensions;
            CalculatedDimensions.X = gameobject.Window.ClientBounds.Width - 100;

            float ratio = (float)LogoImage.Width / (float)LogoImage.Height; //800/600, 8/6.
            CalculatedDimensions.Y = CalculatedDimensions.X / ratio;


            //gameobject.GraphicsDevice.Clear(Color.Black);
            //spritebatch.start already called.
            gameobject.spriteBatch.Draw(LogoImage,
                new Rectangle(50, 50, (int)CalculatedDimensions.X,(int)CalculatedDimensions.Y), new Color((int)AlphaValue, (int)AlphaValue, (int)AlphaValue, (int)AlphaValue));


            DodgerX.DrawVersion(gameobject);


        }
        //given a image and a succeeding state, shows the logo (fade in, fade out on click or after delay) and then proceeds
        //to the indicated state.
        public IntroState(Texture2D pLogoImage,iGameState NextState)
        {
            LogoImage = pLogoImage;
            _NextState = NextState;
        }

        public void Update(DodgerX gameobject, GameTime gameTime)
        {
            if (FirstCall == null)
                FirstCall = gameTime.TotalGameTime;

            TimeSpan CurrentCall = gameTime.TotalGameTime;

            Debug.Print(mLogoMode.ToString() + " " + AlphaValue.ToString());
            //we want the Alpha value to ramp up to opaque, sit there for 5 seconds, and fade out when we detect a mouse click.
            switch (mLogoMode)
            {
                case LogoShowMode.Logo_Fadein:
                    TimeSpan es = CurrentCall-FirstCall.Value;
                    AlphaValue = ((float)es.Ticks / (float)FadeTime.Ticks)*255f;
                    AlphaValue = MathHelper.Clamp(AlphaValue, 0, 255);
                    if ((Mouse.GetState().LeftButton == ButtonState.Pressed))
                        es = FadeTime.Add(new TimeSpan(0,0,0,1));
                    if(es > FadeTime)
                    {
                        mLogoMode = LogoShowMode.Logo_Solid;
                        FadeinComplete = gameTime.TotalGameTime;
                    }
                    break;
                case LogoShowMode.Logo_Solid:
                    TimeSpan SolidElapsed = CurrentCall - FadeinComplete.Value;
                    AlphaValue = 255;
                    if ((Mouse.GetState().LeftButton == ButtonState.Pressed) || SolidElapsed > MaxLogoShowTime)
                        mLogoMode = LogoShowMode.Logo_Fadeout;

                    break;

                case LogoShowMode.Logo_Fadeout:
                    if (FadeOutStart == null) FadeOutStart = gameTime.TotalGameTime;
                     TimeSpan eso = CurrentCall-FadeOutStart.Value;
                     AlphaValue = ((float)eso.Ticks / (float)FadeTime.Ticks) * 255f;
                    AlphaValue = 255-MathHelper.Clamp(AlphaValue, 0, 255);
                    if(eso > FadeTime)
                    {
                        gameobject.CurrentState = _NextState;
                        FadeinComplete = gameTime.TotalGameTime;
                    }
                    break;
            }




        }

        public static void LoadContent(DodgerX gameobject)
        {
            //LogoImage = gameobject.Content.Load<Texture2D>("BASeCamp_Logo");
        }

      
    }
}
