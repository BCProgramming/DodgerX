using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace DodgerX
{
    class PauseGameState:iGameState 
    {
        public Cue MusicTrack { get { return null; } }
        private iGameState _PreviousState = null;
        
        public PauseGameState(iGameState PreviousState)
        {

            _PreviousState = PreviousState;

        }

        public void Draw(DodgerX gameobject, GameTime gameTime)
        {
            
            _PreviousState.Draw(gameobject, gameTime);



        }

        public void Update(DodgerX gameobject, GameTime gameTime)
        {
            //we're paused so do nothing :P
        }
    }
}
