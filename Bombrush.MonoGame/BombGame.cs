using System;
using System.Globalization;
using Bombrush.MonoGame.Rendering;
using Bombrush.MonoGame.States;
using Game2DFramework;
using Game2DFramework.Drawing;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bombrush.MonoGame
{
    class BombGame : Game2D
    {
        internal const int MaxPlayerCount = 4;

        internal const int GameLevelWidth = 15;
        internal const int GameLevelHeight = 13;
        internal const int Tilesize = 32;
        internal const int MenuStartY = 90;

        public BombGame()
            : base(800, 600, false, depthFormat: DepthFormat.Depth16)
        {
            Content.RootDirectory = "Content";
        }

        private void InitializeDefaultSettings()
        {
            var changed = false;
            string value;
            if (!TryGetPropertyString("ServerName", out value))
            {
                SetProperty("ServerName", Environment.MachineName);
                changed = true;
            }
            if (!TryGetPropertyString("Nickname", out value))
            {
                SetProperty("Nickname", Environment.UserName);
                changed = true;
            }
            if (!TryGetPropertyString("Player1Config", out value))
            {
                SetProperty("Player1Config", string.Format("{0},{1},{2},{3},{4}", Keys.A, Keys.D, Keys.W, Keys.S, Keys.LeftShift));
                changed = true;
            }
            if (!TryGetPropertyString("Player2Config", out value))
            {
                SetProperty("Player2Config", string.Format("{0},{1},{2},{3},{4}", Keys.J, Keys.L, Keys.I, Keys.K, Keys.H));
                changed = true;
            }
            if (!TryGetPropertyString("Player3Config", out value))
            {
                SetProperty("Player3Config", string.Format("{0},{1},{2},{3},{4}", Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Enter));
                changed = true;
            }
            if (!TryGetPropertyString("Player4Config", out value))
            {
                SetProperty("Player4Config", string.Format("{0},{1},{2},{3},{4}", Keys.NumPad1, Keys.NumPad3, Keys.NumPad5, Keys.NumPad2, Keys.NumPad0));
                changed = true;
            }
            if (!TryGetPropertyString("PlayerNetConfig", out value))
            {
                SetProperty("PlayerNetConfig", string.Format("{0},{1},{2},{3},{4}", Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Space));
                changed = true;
            }

            int intValue;
            if (!TryGetPropertyInt("MultiPlayerPort", out intValue))
            {
                SetProperty("MultiPlayerPort", intValue.ToString(CultureInfo.InvariantCulture)); 
            }

            if (changed) SavePropertyChanges();
        }

        protected override void Initialize()
        {
            IsMouseVisible = false;
            InitializeDefaultSettings();
            base.Initialize();
        }

        protected override Type RegisterStates()
        {
            RegisterState(new MatchState());
            RegisterState(new MainMenuState());
            RegisterState(new LocalGameConfigurationState());
            RegisterState(new NetworkGameState());
            RegisterState(new LobbyState());
            RegisterState(new OptionMenuState());
            RegisterState(new WaitState());
            RegisterState(new MatchResultState());
            RegisterState(new WinnerState());
            RegisterState(new CreditsState());
            RegisterState(new ConfigureInputState());
            RegisterState(new GuiTestState());

            return typeof(MainMenuState);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Heads.LoadContent(Content);
            Resources.LoadContent(Content);
            Resources.ChangeResolution(ScreenWidth, ScreenHeight);
            Cursor.AddCursorType("Arrow", new CursorType(Content.Load<Texture2D>("textures/cursor")));
        }

#if WINDOWS || LINUX
        static class Program
        {
            static void Main()
            {
                Game2D game = null;

                try
                {
                    game = new BombGame();
                    game.Run();
                }
                catch (Exception ex)
                {
                    if(game != null) game.IsMouseVisible = true;

                    Log.WriteException(ex);
                    //Todo Error Messagebox.
                    //System.Windows.Forms.MessageBox.Show(ex.ToString(), "Unhandled Exception", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                }
            }
        }
#endif
    }
}
