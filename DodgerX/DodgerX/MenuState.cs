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
    public class MenuStateItem
    {
        public SpriteFont sf = null;
        public Vector2 Location { get; set; }
        private String _Caption = "";
        /// <summary>
        /// caption of this menu.
        /// </summary>
        /// 
        public String Caption { get { return _Caption; } set { _Caption = value; Remeasureitem(); } }
        public delegate void StateAdvanceRoutine(MenuStateItem msi, DodgerX dx);
        //state to advance too.
        private StateAdvanceRoutine _AdvanceRoutine=null;


        public static StateAdvanceRoutine DefaultAdvanceRoutine(iGameState advanceState)
        {

            return new StateAdvanceRoutine((msi, dx) => dx.CurrentState = advanceState);

        }

        public StateAdvanceRoutine AdvanceRoutine { get { return _AdvanceRoutine; } set { _AdvanceRoutine = value; } }


        //public iGameState AdvanceState { get; set; }
        public Vector2 DrawSize { get; set; }
        private void Remeasureitem()
        {


            DrawSize = sf.MeasureString(Caption);

        }

        public MenuStateItem(String pCaption, iGameState pAdvanceState, Vector2 pLocation,SpriteFont usefont)
            : this(pCaption, DefaultAdvanceRoutine(pAdvanceState), pLocation, usefont)
        {
            
            
        }
        
        public MenuStateItem(String pCaption, StateAdvanceRoutine sar, Vector2 pLocation, SpriteFont usefont)
          
        {

            sf = usefont;
            Caption = pCaption;
            //AdvanceState = pAdvanceState;
            _AdvanceRoutine = sar;
            Location = pLocation;


        }
        /// <summary>
        /// hittests this Menu Item with the given position. returns true if within rectangle bounds.
        /// </summary>
        /// <param name="TestLocation"></param>
        /// <returns></returns>
        public bool HitTest(DodgerX gameobject,Vector2 TestLocation)

        {
            return TestLocation.X > Location.X &&
                TestLocation.X < Location.X + DrawSize.X &&
                TestLocation.Y > Location.Y &&
                TestLocation.Y < Location.Y + DrawSize.Y;

        }

        public virtual void Draw(DodgerX gameobject, bool Selected)
        {

            gameobject.spriteBatch.DrawString(sf, Caption, Location, Selected ? Color.Blue : Color.White);


        }
    }


    public class MenuStateItemChoiceSet : MenuStateItem
    {
        public class ItemChoiceValue
        {
            public String Caption { get; set; }
            public Object Data { get; set; }

            public ItemChoiceValue(String pCaption, Object pData)
            {
                Caption = pCaption;
                Data = pData;


            }

        }
        private ItemChoiceValue[] ItemChoices;
        private int SelectedIndex = 0;

        public MenuStateItemChoiceSet(String pCaption,Vector2 pLocation,SpriteFont usefont,
            ItemChoiceValue[] pItemChoices):
            base(pCaption,(StateAdvanceRoutine)null,pLocation,usefont)
            

        {
            AdvanceRoutine = AdvanceState;
        ItemChoices = pItemChoices;
        
        }
        private void AdvanceState(MenuStateItem msi,DodgerX gameobject)
        {

            SelectedIndex = (SelectedIndex + 1) % ItemChoices.Length;

        }
        public override void Draw(DodgerX gameobject, bool Selected)
        {


            Caption = ItemChoices[SelectedIndex].Caption;
            base.Draw(gameobject, Selected);
        }
    }


    public class MenuState:iGameState,iGameRunner 
    {
        public Cue MusicTrack { get { return DodgerX.IntroMusic; } }
        public static MenuState getMainMenu(DodgerX gameobject)
        {

            return new MenuState(gameobject, new string[] { "Play","High Scores", "Exit" }, new Object[] { new GameRunningState(),new HighScoreListState(), DynaState.QuitState }); 

        }
        public static MenuState GetPauseState(DodgerX dx, iGameState currentstate)
        {


            String[] pauseItems = new string[] { "Resume Game", "Quit" };
            var copied = currentstate;
            var pausemenu = new MenuState(dx, pauseItems, new Object[] { new MenuStateItem.StateAdvanceRoutine((msi,dxa)=>
                {
                    dxa.CurrentState = copied;
                    if(copied is GameRunningState)
                    {
                        GameRunningState grs = copied as GameRunningState;
                        GameRunningState.TrackCue.Resume();


                    }

                })
                
                
                
                , MenuState.getMainMenu(dx) });

            pausemenu.MenuLogo = pauselogo;
            return pausemenu;



        }


        public event Action<MenuStateItem,DodgerX> ItemClicked;
        public static Cue MenuSelect;
        public static Texture2D Cursor;
        public static SpriteFont MenuFont;
        public static Texture2D intrologo;
        public static Texture2D pauselogo;
        public List<MenuStateItem> MenuItems = new List<MenuStateItem>();
        public MenuStateItem SelectedItem = null;
        
        private GameRunnerData _grd = new GameRunnerData();

        public GameRunnerData grd
        {
            get { return _grd; }
            set { _grd = value; }
        }
        public MenuState(DodgerX dodgerobj, String[] MenuOptions, iGameState[] MenuStates)
            :this(dodgerobj,MenuOptions,(from p in MenuStates select MenuStateItem.DefaultAdvanceRoutine(p)).ToArray())
        {


        }
        protected void InvokeClick(MenuStateItem itemclicked,DodgerX gameobject)
        {
            var copied = ItemClicked;
            if (copied != null) copied(itemclicked,gameobject);


        }
        public MenuState(DodgerX dodgerobj, MenuStateItem[] menuitems)
        {

            MenuItems = menuitems.ToList();


        }
        public MenuState(DodgerX dodgerobj, String[] MenuOptions, MenuStateItem.StateAdvanceRoutine[] stateroutines)
            :this(dodgerobj,MenuOptions,(Object[])stateroutines)
        {



        }
        public MenuState(DodgerX dodgerobj,String[] MenuOptions, Object[] MenuStates)
        {


            Vector2 CurrentLocation= new Vector2(dodgerobj.Window.ClientBounds.Width/2-intrologo.Width/2,intrologo.Height+50);

            for (int i = 0; i < Math.Min(MenuOptions.Length, MenuStates.Length); i++)
            {
                MenuStateItem msi = null;
                if (MenuStates[i] is MenuStateItem.StateAdvanceRoutine)
                {
                    msi = new MenuStateItem(MenuOptions[i], MenuStates[i] as MenuStateItem.StateAdvanceRoutine, CurrentLocation, MenuFont);
                }
                else if (MenuStates[i] is iGameState)
                    msi = new MenuStateItem(MenuOptions[i], MenuStates[i] as iGameState, CurrentLocation, MenuFont);


                msi.Location = new Vector2((dodgerobj.Window.ClientBounds.Width / 2) - msi.DrawSize.X / 2, msi.Location.Y);

                CurrentLocation = new Vector2(CurrentLocation.X, CurrentLocation.Y + msi.DrawSize.Y + 5);
                MenuItems.Add(msi);
            }





        }

        public static void LoadContent(DodgerX gameobject)
        {
            MenuFont = gameobject.Content.Load<SpriteFont>(@"Fonts\menufont");
            Cursor = gameobject.Content.Load<Texture2D>(@"Images\arrow");
            intrologo = gameobject.Content.Load<Texture2D>(@"Images\bcdodgertitle");
            pauselogo = gameobject.Content.Load<Texture2D>(@"Images\bcdodgerpause");
        }
        //private Cue _MenuMusic = null;
        //private Cue _PreviousMusic = null;
        private Texture2D _ourLogo = null;
        //public Cue MenuMusic { get { return _MenuMusic; } set { _MenuMusic = value; } }
        //public Cue PreviousMusic { get { return _PreviousMusic; } set { _PreviousMusic = value; } }
        public Texture2D MenuLogo { get { return _ourLogo ?? intrologo; } set { _ourLogo = value; } }
        protected virtual Texture2D getintrologo()
        {

            return MenuLogo;

        }
        public virtual void Draw(DodgerX gameobject, GameTime gameTime)
        {



            _grd.Draw(gameobject, gameTime);

            gameobject.spriteBatch.Draw(getintrologo(), new Vector2(gameobject.Window.ClientBounds.Width / 2 - getintrologo().Width / 2, 0), Color.White);

            //
            //draw our Menu items.
            Vector2 MousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            foreach (var msi in MenuItems)
            {

                msi.Draw(gameobject, msi == SelectedItem);

            }
            
            gameobject.spriteBatch.Draw(Cursor, MousePos,Color.White);
            DodgerX.DrawVersion(gameobject);
        }
        private MouseState? prevmousestate;
        private TimeSpan SpawnObjectDelay = new TimeSpan(0, 0, 0,0, 100);
        private TimeSpan ElapseCounter = new TimeSpan();
        public virtual void Update(DodgerX gameobject, GameTime gameTime)
        {
            if (prevmousestate == null) prevmousestate = Mouse.GetState();
            ElapseCounter += gameTime.ElapsedGameTime;
            if (ElapseCounter > SpawnObjectDelay)
            {

                ElapseCounter = ElapseCounter - SpawnObjectDelay;
                for (int i = 0; i < 2; i++)
                {
                    AttackingObject ao = new AttackingObject(new Vector2(0, 0), new Vector2(0, 0),
                                                             gameobject.attackerTexture);
                    ao.SetRandomStartPosition(gameobject, 8);

                    grd.Attackers.Add(ao);
                }

            }
            _grd.Update(gameobject, gameTime);
            MenuSelect = DodgerX.soundBank.GetCue("MenuSel");
            //Update the selected item based on the mouse position.
            MenuStateItem foundhit = null;
            foreach (var msi in MenuItems)
            {

                if (msi.HitTest(gameobject, new Vector2(Mouse.GetState().X, Mouse.GetState().Y)))
                {
                    foundhit = msi;
                    break;
                }


            }
            if (SelectedItem != foundhit)
            {
                if (MenuSelect.IsPlaying) MenuSelect.Stop(AudioStopOptions.AsAuthored);
                try
                {
                    MenuSelect.Play();
                } catch
                {
                }
            }
            SelectedItem = foundhit;
            Debug.Print("SelectedItem is now " + (SelectedItem==null?"Null":SelectedItem.Caption));

            if (SelectedItem != null)
            {


                if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevmousestate.Value.LeftButton==ButtonState.Released)
                {

                    //gameobject.CurrentState = SelectedItem.AdvanceState;
                    InvokeClick(SelectedItem,gameobject);
                    SelectedItem.AdvanceRoutine(SelectedItem, gameobject);


                }

            }

            prevmousestate = Mouse.GetState();
        }
    }
}
