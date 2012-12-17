using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace DodgerX
{
    //used to create one-shot gamestates using lambda functions for Draw and/or update.
    class DynaState:iGameState
    {
        public Cue MusicTrack { get { return null; } }
        public static DynaState QuitState = new DynaState((dx, gt) => dx.Exit(), null);

        public delegate void DrawRoutine(DodgerX gameobject, GameTime gt);
        public delegate void UpdateRoutine(DodgerX gameobject, GameTime gt);

        public DrawRoutine DrawFunction { get; set; }
        public UpdateRoutine UpdateFunction { get; set; }

        public DynaState(UpdateRoutine updatingRoutine, DrawRoutine DrawingRoutine)
        {
            DrawFunction = DrawingRoutine;
            UpdateFunction = updatingRoutine;


        }

        public void Draw(DodgerX gameobject, GameTime gameTime)
        {
            if (DrawFunction != null) DrawFunction(gameobject, gameTime);
        }

        public void Update(DodgerX gameobject, GameTime gameTime)
        {
            if (UpdateFunction != null) UpdateFunction(gameobject, gameTime);
            
        }
    }
}
