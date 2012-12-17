using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DodgerX
{
    public class HealyBall : AttackingObject
    {
        public HealyBall(Vector2 pLocation,Vector2 pVelocity,Texture2D pTexture)
            : base(pLocation, pVelocity, pTexture)
        {
    

        }
        protected override AttackingObject.DestructionReturnType Destroy(Player.PlayerCircleItem impactwith)
        {
            return DestructionReturnType.Destruct_Heal | DestructionReturnType.Destruct_Self;
        }

    }
    //attacking object is the object that flies in from the sides of the screen.
    public class AttackingObject
    {
        [Flags]
        public enum DestructionReturnType
        {
            Destruct_Self = 1,
            Destruct_PlayerPiece=2,
            Destruct_Heal=4,
            Destruct_Both = Destruct_Self|Destruct_PlayerPiece

        }

        public Texture2D Texture { get; set; }
        public Vector2 Location { get; set; }
        public Vector2 Velocity { get; set; }
        
        public AttackingObject(Vector2 pLocation, Vector2 pVelocity,Texture2D pTexture)
        {
            Location = pLocation;
            Velocity = pVelocity;
            Texture = pTexture;


        }
        /// <summary>
        /// chooses a random starting location along the outside border of the window.
        /// </summary>
        /// <param name="gobject"></param>
        /// <returns></returns>
        public void SetRandomStartPosition(DodgerX gobject,float usespeed)
        {
            Rectangle cb = gobject.Window.ClientBounds;

            //first choose random X position.
            float RandomX=0, RandomY=0;
            float randompercent = (float)DodgerX.rgen.NextDouble();
            int randomside = DodgerX.rgen.Next(0, 4);


            Vector2 RandomPosition = new Vector2((float)((cb.Width / 2) * DodgerX.rgen.NextDouble() + cb.Width / 4), (float)((cb.Height / 2) * DodgerX.rgen.NextDouble() + cb.Height / 4));
            
            switch (randomside)
            {
                case 0: //Left side
                    RandomX = -Texture.Width+1;
                    RandomY = ((cb.Height - Texture.Height) * randompercent) + Texture.Height;
                    break;
                case 1: //Top side
                    RandomX = ((cb.Width - Texture.Width) * randompercent) + Texture.Width;
                    RandomY = -Texture.Height + 1;
                    break;
                case 2: //bottom
                    RandomX = ((cb.Width - Texture.Width) * randompercent) + Texture.Width;
                    RandomY = cb.Height - 1;
                    break;
                case 3: //Right
                    RandomX = cb.Width - 1;
                    RandomY = ((cb.Height - Texture.Height) * randompercent) + Texture.Height;
                    
                    break;


            }
            Vector2 startpos = new Vector2(RandomX, RandomY);
            double gotangle = DodgerX.GetAngle(startpos, RandomPosition);

            Vector2 makespeed = new Vector2((float)(Math.Cos(gotangle) * usespeed), (float)(Math.Sin(gotangle) * usespeed));


            Location = startpos;
            Velocity = makespeed;





        
        }
        public void Boom(iGameRunner gd)
        {






        }

        private bool EntirelyWithinBounds(DodgerX gameobject)
        {
            Rectangle cb = gameobject.Window.ClientBounds;

            
            return !((Location.X > cb.Width) ||
                (Location.X + Texture.Width < 0) ||
                (Location.Y > cb.Height) ||
                (Location.Y + Texture.Height < 0));



        }
        private float Distance(Vector2 pointA, Vector2 pointB)
        {
            float XDiff = pointB.X-pointA.X;
            float YDiff = pointB.Y - pointA.Y;
            return (float)Math.Sqrt(XDiff * XDiff + YDiff * YDiff);
            


        }

        private bool CheckPlayerCollision(DodgerX go,Player playerobject)
        {
            if (playerobject == null) return false;
            List<Player.PlayerCircleItem> removethese = new List<Player.PlayerCircleItem>();

            Vector2 ourCenter = new Vector2(Location.X + Texture.Width / 2, Location.Y + Texture.Height / 2);
            bool returnvalue = false;
            foreach (var iterate in playerobject.CircleItems)
            {
                Vector2 iteratecenter = new Vector2(iterate.X + iterate.Texture.Width / 2, iterate.Y + iterate.Texture.Height / 2);
                if(Distance(ourCenter,iteratecenter)
                    < (Texture.Width / 2 + iterate.Texture.Width / 2))
                {
                    //HIT.
                    //destroy.

                    DestructionReturnType destructiontype = Destroy(iterate);
                    if ((destructiontype & DestructionReturnType.Destruct_PlayerPiece) == DestructionReturnType.Destruct_PlayerPiece)
                    {
                        iterate.hitcount++;
                        if(iterate.hitcount > iterate.Textures.Length-1)
                            removethese.Add(iterate);

                        DodgerX.soundBank.GetCue("gren").Play();

                    }
                    if ((destructiontype & DestructionReturnType.Destruct_Heal) == DestructionReturnType.Destruct_Heal)
                    {

                        if (iterate.hitcount > 0)
                        {
                            DodgerX.soundBank.GetCue("charge").Play();
                            iterate.hitcount--;
                        }
                        else
                        {
                            playerobject.Heal(1);
                        }

                    }
                    if ((destructiontype & DestructionReturnType.Destruct_Self) == DestructionReturnType.Destruct_Self)
                    {
                        returnvalue = true;
                        break;
                    }

                   


                }


            }

            foreach (var removethis in removethese)
            {

                playerobject.CircleItems.Remove(removethis);
            }


            return returnvalue;
        }
        protected virtual DestructionReturnType Destroy(Player.PlayerCircleItem impactwith)
        {
            return DestructionReturnType.Destruct_Both;

        }

        private static Vector2 RandomSpeed(float speedamount)
        {

            float chosenangle = (float)(Math.PI * (DodgerX.rgen.NextDouble()));

            return new Vector2((float)(Math.Sin(chosenangle) * speedamount), (float)(Math.Cos(chosenangle) * speedamount));


        }
        TimeSpan Spawnparticledelay = new TimeSpan(0, 0, 0, 0, 50);
        TimeSpan accumtime = new TimeSpan();
        public bool PerformFrame(DodgerX GameObject,GameTime gt)
        {
            //increment location.
            Location += new Vector2(Velocity.X*GameObject.SpeedMultiplier,Velocity.Y*GameObject.SpeedMultiplier);

            //return GameObject.Window.ClientBounds.Contains((new Rectangle((int)Location.X, (int)Location.Y, (int)Texture.Width, (int)Texture.Height)));
            Player pobj = null;
            var casted = (GameObject.CurrentState as  iGameRunner);
            if (casted != null)
            {
                pobj = casted.grd.PlayerObject;
            }
            bool playercollide = CheckPlayerCollision(GameObject,pobj);


            accumtime += gt.ElapsedGameTime;
            if (accumtime > Spawnparticledelay)
            {
                accumtime -= Spawnparticledelay;
                if (casted != null )
                {
                    Particle spawnparticle = new Particle(new Vector2(Location.X, Location.Y), RandomSpeed(2),GameObject.SpeckImage);
                    spawnparticle.Velocity += Velocity;
                    casted.grd.Particles.Add(spawnparticle);
                }
            }

            return !EntirelyWithinBounds(GameObject) || playercollide;



                ;
        }
        public void Draw(DodgerX GameObject)
        {
            GameObject.spriteBatch.Draw(Texture, new Vector2(Location.X -Texture.Width/2,Location.Y-Texture.Height/2) , Color.White);
            //GameObject.spriteBatch.DrawString(GameObject.DefaultFont, Location.ToString(), Location, Color.Black);

        }
    }

    
}
