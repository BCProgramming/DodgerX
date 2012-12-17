using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAColor = Microsoft.Xna.Framework.Color ;
namespace DodgerX
{
    public class Particle
    {
        private Vector2 _DecayFactor = new Vector2(0.95f, 0.95f);
        private TimeSpan TTL = new TimeSpan(0, 0, 0, 0,750);
        private TimeSpan AliveTime = new TimeSpan();
        public Vector2 Location { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 DecayFactor { get { return _DecayFactor; } set { _DecayFactor = value; }}
        public Texture2D ParticleTexture { get; set; }
        public System.Drawing.Color ParticleColor = System.Drawing.Color.Red;
        
        public Particle(Vector2 pLocation, Vector2 pVelocity, Texture2D pTexture)
        {
            Location = pLocation;
            Velocity = pVelocity;

            ParticleTexture = pTexture;
        }
        public virtual void Draw(DodgerX gameobject,GameTime gt)
        {
            var usevalue = (int)(255f - (GetLivePercent() * 255f));
            XNAColor usecolor = new XNAColor(usevalue, usevalue, usevalue,usevalue);
            gameobject.spriteBatch.Draw(ParticleTexture, 
                new Vector2(Location.X-((float)ParticleTexture.Width/2),Location.Y-((float)ParticleTexture.Height/2)), usecolor);

        }
        private float GetLivePercent()
        {

            //retrives the percent we are through our "life".
            return ((float)AliveTime.Ticks) / (float)TTL.Ticks;


        }

        /// <summary>
        /// returns true if this Particle's Life is over. A sad time, I guess.
        /// </summary>
        /// <param name="gameobject"></param>
        /// <returns></returns>
        public virtual bool Update(DodgerX gameobject, GameTime gt)
        {

            AliveTime += gt.ElapsedGameTime;
            

           


            Velocity *= DecayFactor;
            Location += Velocity;

            return AliveTime > TTL;

        }





    }
}
