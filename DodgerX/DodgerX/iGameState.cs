using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace DodgerX
{

    public class GameRunnerData
    {
        private List<Particle> _Particles=new List<Particle>();
        private List<AttackingObject> _Attackers= new List<AttackingObject>();
        private Player _PlayerObject;

        public List<Particle> Particles
        {
            get { return _Particles; }
            set { _Particles = value; }
        }

        public List<AttackingObject> Attackers
        {
            get { return _Attackers; }
            set { _Attackers = value; }
        }
        public Player PlayerObject { get { return _PlayerObject; } set { _PlayerObject = value; } }

        public void Draw(DodgerX gameobject, GameTime gt)
        {
            foreach (Particle p in Particles)
            {

                p.Draw(gameobject, gt);

            }
            foreach (AttackingObject ao in Attackers)
                ao.Draw(gameobject);

        
            if (PlayerObject != null) PlayerObject.Draw(gameobject,gt);
            String FPSString = Math.Round(gameobject.CurrentFPS, 2).ToString() + " fps";
            Vector2 FPSSize = gameobject.DefaultFont.MeasureString(FPSString);

            String ParticleCount = Particles.Count.ToString();
            gameobject.spriteBatch.DrawString(gameobject.DefaultFont, FPSString + " " + ParticleCount + " Particles.", new Vector2(0, gameobject.Window.ClientBounds.Height - FPSSize.Y), Color.Black);
        }

        public void Update(DodgerX gameobject, GameTime gt)
        {
            List<AttackingObject> removeattackers = new List<AttackingObject>();
            List<Particle> removeparticles = new List<Particle>();
            foreach (AttackingObject loopattacker in Attackers)
            {
                if (loopattacker.PerformFrame(gameobject,gt))
                {

                    removeattackers.Add(loopattacker);
                }

            }


            foreach (Particle loopparticle in Particles)
            {


                if (loopparticle.Update(gameobject, gt))
                {

                    removeparticles.Add(loopparticle);


                }

            }

            foreach (var iterate in removeattackers)
                Attackers.Remove(iterate);

            foreach (var remparticle in removeparticles)
            {
                Particles.Remove(remparticle);

            }






            if(_PlayerObject !=null)
                _PlayerObject.PerformFrame(gameobject,gt);

        }

    }



    public interface iGameRunner
    {
        GameRunnerData grd { get; set; }

    }

    public interface iGameState
    {

        Cue MusicTrack { get; }
        void Draw(DodgerX gameobject,GameTime gameTime);
        void Update(DodgerX gameobject,GameTime gameTime);
    }
}
