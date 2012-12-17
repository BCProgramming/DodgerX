using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DodgerX
{
    //represents the player.
    //the player sits at the mouse position, and has a number of circles
    public class Player
    {

        //represents a circle that rotates around the player.
        public class PlayerCircleItem
        {

            public float CurrentAngle = 0;
            public float AngleSpeed = 0;
            public float Radius = 16;
            private int _hitcount = 0;
            public Texture2D[] Textures;
            public Texture2D Texture { get { return Textures[hitcount]; } }
            public Texture2D[] ParticleImages;
            public Texture2D ParticleImage { get { return ParticleImages[_hitcount]; } }
            public float X=0, Y = 0;
            public int hitcount { get { return _hitcount; } set { _hitcount = value; } }
            
            public PlayerCircleItem(float pCurrentAngle, float pAngleSpeed, float pRadius,Texture2D[] pTextures,Texture2D[] Emissions)
            {
                CurrentAngle = pCurrentAngle;
                AngleSpeed = pAngleSpeed;
                Radius = pRadius;
                Textures = pTextures;
                ParticleImages = Emissions;

            }
            
            public Vector2 GetSpeedVector(Player playerobject)
            {
                
                    Vector2 CurrentLocation = new Vector2(X, Y);
                    Vector2 NextPosition = new Vector2(
                        (float)(Math.Cos(CurrentAngle + AngleSpeed) * Radius + playerobject.Position.X),
                        (float)(Math.Sin(CurrentAngle + AngleSpeed) * Radius + playerobject.Position.Y));


                        
                    return NextPosition-CurrentLocation+new Vector2(playerobject.Speed().X/5,playerobject.Speed().Y/5);




                
                
                
                
            }

            public void PerformFrame(DodgerX gameobject,Player playerobject)
            {
                CurrentAngle += AngleSpeed;
                

                X = (float)(Math.Cos(CurrentAngle) * Radius) + playerobject.Position.X;
                Y = (float)(Math.Sin(CurrentAngle) * Radius) + playerobject.Position.Y;
            }

            public void Draw(DodgerX GameObject)
            {

                GameObject.spriteBatch.Draw(Texture, new Vector2(X - Texture.Width / 2, Y - Texture.Height / 2), Color.White);




            }



        }
        private TimeSpan ParticleSpawnDelay = new TimeSpan(0, 0, 0, 0, 54);
        private TimeSpan Accum = new TimeSpan();
        public Vector2 Position { get; set; }
        public event Action<DodgerX,Player> PlayerDeath = null;
        Vector2 PreviousPosition = new Vector2();
        public List<PlayerCircleItem> CircleItems = new List<PlayerCircleItem>();
        public void Heal(int amount)
        {


            var hurtcircles = from p in CircleItems where p.hitcount > 0 select p;

            int i = 0;
            foreach (var iterate in hurtcircles)
            {
                i++;
                iterate.hitcount--;


                if (i == amount) return;
            }




        }
        public void PerformFrame(DodgerX gameobj, GameTime gt)
        {
            Accum += gt.ElapsedGameTime;
            PreviousPosition = Position;
            Vector2 MousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Vector2 diff = MousePosition - Position;

            
            if (Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y) > MaxSpeed)
            {
                diff.Normalize();
                Position += diff * MaxSpeed;
            }
            
            if (Position.X < 0) Position = new Vector2(0, Position.Y);
            if (Position.X > gameobj.Window.ClientBounds.Width) Position = new Vector2(gameobj.Window.ClientBounds.Width, Position.Y);

            if (Position.Y < 0) Position = new Vector2(Position.X, 0);
            if (Position.Y > gameobj.Window.ClientBounds.Height) Position = new Vector2(Position.X, gameobj.Window.ClientBounds.Height);
            
            foreach (PlayerCircleItem pci in CircleItems)
            {
                pci.PerformFrame(gameobj,this);

            }

            if (Accum > ParticleSpawnDelay)
            {
                Accum-=ParticleSpawnDelay;
                foreach (PlayerCircleItem pci in CircleItems)
                {
                    
                    
                    
                    Particle Spawnparticle = new Particle(new Vector2(pci.X, pci.Y), pci.GetSpeedVector(this), pci.ParticleImage);
                    Spawnparticle.Velocity += this.Speed();
                    iGameRunner gr =  gameobj.CurrentState as iGameRunner;
                    if (gr != null)
                        gr.grd.Particles.Add(Spawnparticle);
                }


            }
            if (!CircleItems.Any())
            {
                if (PlayerDeath != null)
                    PlayerDeath(gameobj,this);


            }


        }
        public float MaxSpeed = 8f;
        public Vector2 Speed()
        {
            return Position - PreviousPosition;


        }
        public int NumItemsLeft()
        {

            return CircleItems.Count();

        }

        public Player(DodgerX gameobj,int NumCircles)
        {

            double currentangle = 0;
            double increment = (Math.PI * 2) / NumCircles;
            int curritem = 0;
            while (curritem < NumCircles)
            {
                float useradius = 32 * (curritem % 2 == 0 ? 1 : 2);
                float usespeed = ((float)increment / 8) * (curritem % 2 == 0 ? 1 : 1.25f);
                PlayerCircleItem rci = new PlayerCircleItem((float)(curritem*increment),usespeed,useradius,
                    new Texture2D[]{gameobj.greencircletexture,gameobj.yellowcircletexture,gameobj.redcircletexture},
                    new Texture2D[]{gameobj.GreenSpeck,gameobj.YellowSpeck,gameobj.RedSpeck});
                CircleItems.Add(rci);

                curritem++;
            }






        }

        public void Draw(DodgerX gameobj,GameTime gt)
        {

            foreach (PlayerCircleItem pci in CircleItems)
            {

                pci.Draw(gameobj);

            }


        }
    }

    }

