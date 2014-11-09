using System;
using System.Collections.Generic;
using System.Globalization;
using Bombrush.MonoGame.Gui;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.States
{
    internal class OptionMenuState : BackgroundState
    {
        private StackedMenu _optionsMenu;
        private StateChangeInformation _stateChangeInformation;

        private static List<string> _resolutions;

        private static IEnumerable<string> GetResolutions(GraphicsDevice device)
        {
            if (_resolutions == null)
            {
                _resolutions = new List<string>();
                foreach (var displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                { 
                    var resolutionText = string.Format("{0}x{1}", displayMode.Width, displayMode.Height);
                    if (displayMode.Width >= 640 && displayMode.Height >= 480 || _resolutions.Contains(resolutionText))
                        _resolutions.Add(resolutionText);
                }
            }

            return _resolutions;
        }

        private string GetCurrentResultion()
        {
            return string.Format("{0}x{1}", Game.ScreenWidth, Game.ScreenHeight);
        }

        private static void GetResolutionFromString(string data, out int width, out int height)
        {
            string[] res = data.Split(new[] {'x'}, StringSplitOptions.None);
            width = int.Parse(res[0]);
            height = int.Parse(res[1]);
        }

        //todo: Datadriven
        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);

            _optionsMenu = new StackedMenu(Game) { Title = "Options" };
            _optionsMenu.AppendMenuItem(new InputMenuItem(Game, "Port", 5, InputType.Numeric)
            {
                InputText = "1170"
            });
            //_optionsMenu.AppendMenuItem(new BoolMenuItem(Game, "Sound", DecisionType.OnOff, Settings.Default.SoundOn));
            //_optionsMenu.AppendMenuItem(new BoolMenuItem(Game, "FullScreen", DecisionType.YesNo, Settings.Default.FullScreen));
            //_optionsMenu.AppendMenuItem(new EnumMenuItem(Game, "Game Mode", new[] { "2D", "3D" }, Settings.Default.GameMode));
            //_optionsMenu.AppendMenuItem(new EnumMenuItem(Game, "Resolution", GetResolutions(Game.GraphicsDevice), GetCurrentResultion()));
            _optionsMenu.AppendMenuItem(new ActionMenuItem(Game, "Configure Inputs", HandleConfigureInput));
            _optionsMenu.AppendMenuItem(new ActionMenuItem(Game, "Back", HandleBack, ActionTriggerKind.IsCancel));
        }

        public override void OnLeave()
        {
        }

        protected override void OnEntered(object enterInformation)
        {
            _optionsMenu.RecalculatePositions();
            _stateChangeInformation = StateChangeInformation.Empty;
        }

        private void HandleConfigureInput()
        {
            _stateChangeInformation = StateChangeInformation.StateChange(typeof(ConfigureInputState), typeof(SlideTransition));
        }

        private void HandleBack()
        {
            _stateChangeInformation = StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition));

            InputMenuItem port = _optionsMenu.GetMenuItem<InputMenuItem>(0);
            BoolMenuItem soundOnOff = _optionsMenu.GetMenuItem<BoolMenuItem>(1);
            BoolMenuItem isFullScreen = _optionsMenu.GetMenuItem<BoolMenuItem>(2);
            EnumMenuItem gameMode = _optionsMenu.GetMenuItem<EnumMenuItem>(3);
            EnumMenuItem resolution = _optionsMenu.GetMenuItem<EnumMenuItem>(4);

            int width, height;
            GetResolutionFromString(resolution.SelectedItem, out width, out height);

            //todo: Implement Data driven.
            //if (port.InputText != Settings.Default.MultiplayerPort.ToString(CultureInfo.InvariantCulture)
            // || soundOnOff.IsTrue != Settings.Default.SoundOn
            // || isFullScreen.IsTrue != Settings.Default.FullScreen
            // || gameMode.SelectedItem != Settings.Default.GameMode
            // || width != Settings.Default.ScreenWidth)
            //{
            //    Settings.Default.SoundOn = soundOnOff.IsTrue;
            //    if (!string.IsNullOrEmpty(port.InputText))
            //        Settings.Default.MultiplayerPort = int.Parse(port.InputText);
            //    Settings.Default.FullScreen = isFullScreen.IsTrue;
            //    Settings.Default.GameMode = gameMode.SelectedItem;
            //    Settings.Default.ScreenWidth = width;
            //    Settings.Default.ScreenHeight = height;
            //    Settings.Default.Save();    
            //}

            //todo: implement resource reloading
            //Game.ChangeScreenSettings(width, height, isFullScreen.IsTrue);
            Resources.ChangeResolution(width, height);
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);
            _optionsMenu.Update(elapsedTime);
            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);
            _optionsMenu.Draw(Game.SpriteBatch);
        }
    }
}
