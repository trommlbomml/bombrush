using System;
using System.Collections.Generic;
using System.Globalization;
using Bombrush.MonoGame.Gui;
using Game2DFramework;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.States
{
    internal class OptionMenuState : BackgroundState
    {
        private const string SoundPropertyName = "Sound";
        private const string MultiPlayerPortPropertyName = "MultiPlayerPort";
        private const string GameModePropertyName = "GameMode";

        private StackedMenu _optionsMenu;
        private StateChangeInformation _stateChangeInformation;

        private static List<string> _resolutions;

        private static IEnumerable<string> GetResolutions()
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

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);

            _optionsMenu = new StackedMenu(Game) { Title = "Options" };
            _optionsMenu.AppendMenuItem(new InputMenuItem(Game, "Port", 5, InputType.Numeric)
            {
                InputText = Game.GetPropertyIntOrDefault(MultiPlayerPortPropertyName, 1170).ToString(CultureInfo.InvariantCulture)
            });
            _optionsMenu.AppendMenuItem(new BoolMenuItem(Game, "Sound", DecisionType.OnOff, Game.GetPropertyBoolOrDefault(SoundPropertyName)));
            _optionsMenu.AppendMenuItem(new BoolMenuItem(Game, "FullScreen", DecisionType.YesNo, Game.GetPropertyBoolOrDefault(GameProperty.GameIsFullScreenProperty)));
            _optionsMenu.AppendMenuItem(new EnumMenuItem(Game, "Game Mode", new[] { "2D", "3D" }, Game.GetPropertyStringOrDefault(GameModePropertyName, "2D")));
            _optionsMenu.AppendMenuItem(new EnumMenuItem(Game, "Resolution", GetResolutions(), GetCurrentResultion()));
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

            var port = _optionsMenu.GetMenuItem<InputMenuItem>(0);
            var soundOnOff = _optionsMenu.GetMenuItem<BoolMenuItem>(1);
            var isFullScreen = _optionsMenu.GetMenuItem<BoolMenuItem>(2);
            var gameMode = _optionsMenu.GetMenuItem<EnumMenuItem>(3);
            var resolution = _optionsMenu.GetMenuItem<EnumMenuItem>(4);

            int width, height;
            GetResolutionFromString(resolution.SelectedItem, out width, out height);

            Game.SetProperty(SoundPropertyName, soundOnOff.IsTrue);
            Game.SetProperty(GameModePropertyName, gameMode.SelectedItem);
            Game.SetProperty(GameProperty.GameIsFullScreenProperty, isFullScreen.IsTrue);
            Game.SetProperty(GameProperty.GameResolutionXProperty, width);
            Game.SetProperty(GameProperty.GameResolutionYProperty, height);

            if (!string.IsNullOrEmpty(port.InputText))
            {
                Game.SetProperty(MultiPlayerPortPropertyName, int.Parse(port.InputText));
            }

            Game.SavePropertyChanges();

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
