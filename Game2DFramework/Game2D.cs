using System;
using System.Collections.Generic;
using Game2DFramework.Drawing;
using Game2DFramework.Input;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game2DFramework
{
    public abstract class Game2D : Game
    {
        private readonly Dictionary<Type, IState> _availableStates;
        private readonly Dictionary<Type, ITransition> _availableTransitions;
        private bool _afterTransitionQuit;
        private RenderTarget2D _sourceRenderTarget;
        private RenderTarget2D _targetRenderTarget;
        private bool _transitionInProgress;
        private bool _transitionStarted;
        private ITransition _currentTransition;
        private IState _oldStateForSourceTransition;
        private IState _currentState;
        private GamePadEx _gamePad;
        private Type _startupState;

        public int ScreenWidth { get { return GraphicsDevice.Viewport.Width; } }
        public int ScreenHeight { get { return GraphicsDevice.Viewport.Height; } }
        public DepthRenderer DepthRenderer { get; private set; }
        public ShapeRenderer ShapeRenderer { get; private set; }
        public KeyboardEx Keyboard { get; private set; }
        public MouseEx Mouse { get; private set; }

        public GamePadEx GamePad
        {
            get
            {
                if (_gamePad == null) throw new InvalidOperationException("Gamepad must be enabled for game to use");
                return _gamePad;
            }
            private set { _gamePad = value; }
        }

        public SpriteBatch SpriteBatch { get; private set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        
        protected Game2D(int screenWidth, int screenHeight, bool fullscreen, bool useGamePad = false, DepthFormat depthFormat = DepthFormat.None)
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = screenWidth,
                PreferredBackBufferHeight = screenHeight,
                PreferredDepthStencilFormat = depthFormat,
                IsFullScreen = fullscreen
            };
            Content.RootDirectory = "Content";

            _availableStates = new Dictionary<Type, IState>();
            _availableTransitions = new Dictionary<Type, ITransition>();
            Keyboard = new KeyboardEx();
            Mouse = new MouseEx();
            DepthRenderer = new DepthRenderer();
            if (useGamePad) GamePad = new GamePadEx();
        }

        protected abstract Type RegisterStates();

        protected override void Initialize()
        {
            _sourceRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _targetRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            
            RegisterTransition(new BlendTransition());
            RegisterTransition(new FlipTransition(GraphicsDevice));
            RegisterTransition(new GrowTransition(GraphicsDevice));
            RegisterTransition(new SlideTransition(GraphicsDevice));
            RegisterTransition(new CardTransition(GraphicsDevice));
            RegisterTransition(new ThrowAwayTransition(GraphicsDevice));
            RegisterTransition(new ZappoutTransition(GraphicsDevice));

            _startupState = RegisterStates();
            base.Initialize();
        }

        public void RegisterTransition(ITransition transition)
        {
            var t = transition.GetType();
            if (_availableTransitions.ContainsKey(t)) throw new InvalidOperationException("Transition Type alread exists");
            if (transition == null) throw new ArgumentNullException("transition");

            _availableTransitions.Add(t, transition);
        }

        public void RegisterState(IState state)
        {
            if (_availableStates.ContainsKey(state.GetType())) throw new InvalidOperationException("State Type already exists");
            if (state == null) throw new ArgumentNullException("state");

            _availableStates.Add(state.GetType(), state);
            state.Game = this;
        }

        public void SetCurrentState(Type type, object enterInformation)
        {
            IState newState;
            if (!_availableStates.TryGetValue(type, out newState)) throw new InvalidOperationException("State not registered");

            if (_currentState != null) _currentState.OnLeave();
            _currentState = newState;
            newState.OnEnter(enterInformation);
        }

        public void QuitGame(Type transition)
        {
            _afterTransitionQuit = true;
            _transitionInProgress = true;
            _transitionStarted = false;
            _currentTransition = _availableTransitions[transition];
            _currentTransition.Source = null;
            _currentTransition.Target = null;
            _oldStateForSourceTransition = _currentState;

            _currentState.OnLeave();
            _currentState = null;
        }

        public void ChangeToState(Type state, Type transition, object enterInformation)
        {
            IState newState;
            if (!_availableStates.TryGetValue(state, out newState)) throw new InvalidOperationException("State not registered");

            _afterTransitionQuit = false;
            _transitionInProgress = true;
            _transitionStarted = false;
            _currentTransition = _availableTransitions[transition];
            _currentTransition.Source = null;
            _currentTransition.Target = null;
            _oldStateForSourceTransition = _currentState;

            _currentState.OnLeave();
            DepthRenderer.Clear();
            _currentState = newState;
            newState.OnEnter(enterInformation);
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            ShapeRenderer = new ShapeRenderer(SpriteBatch, GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (_currentState == null) SetCurrentState(_startupState, null);

            var elapsed = gameTime.ElapsedGameTime.Milliseconds * 0.001f;

            if (_transitionInProgress)
            {
                if (!_transitionStarted)
                {
                    _currentTransition.Begin();
                    _transitionStarted = true;
                }

                _currentTransition.Update(elapsed);
                if (_currentTransition.TransitionReady)
                {
                    _transitionInProgress = false;
                    if (_afterTransitionQuit) Exit();
                }
            }
            else
            {
                Keyboard.Update();
                Mouse.Update();
                if (_gamePad != null) GamePad.Update();

                var stateChangeInformation = _currentState.OnUpdate(elapsed);
                if (stateChangeInformation != StateChangeInformation.Empty)
                {
                    if (stateChangeInformation.QuitGame)
                    {
                        QuitGame(stateChangeInformation.Transition);
                    }
                    else
                    {
                        ChangeToState(
                            stateChangeInformation.TargetState,
                            stateChangeInformation.Transition,
                            stateChangeInformation.EnterInformation);
                    }
                }
            }
            
            base.Update(gameTime);
        }

        private static void DrawState(float elapsedTime, SpriteBatch spriteBatch, IState toRender)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null);
            toRender.OnDraw(elapsedTime);
            spriteBatch.End();
        }

        private void PreRenderSourceAndTargetState(float elapsedTime)
        {
            GraphicsDevice.SetRenderTarget(_sourceRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);
            DrawState(elapsedTime, SpriteBatch, _oldStateForSourceTransition);

            if (_currentState != null)
            {
                GraphicsDevice.SetRenderTarget(_targetRenderTarget);
                GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);
                _currentState.TransitionRenderTarget = _targetRenderTarget;
                DrawState(elapsedTime, SpriteBatch, _currentState);
                _currentState.TransitionRenderTarget = null;
            }

            GraphicsDevice.SetRenderTarget(null);
        }

        protected override void Draw(GameTime gameTime)
        {
            float elapsedTime = gameTime.ElapsedGameTime.Milliseconds * 0.001f;

            if (_transitionInProgress)
            {
                if (_currentTransition.Source == null && _currentTransition.Target == null)
                {
                    PreRenderSourceAndTargetState(elapsedTime);
                    _currentTransition.Source = _sourceRenderTarget;
                    _currentTransition.Target = _targetRenderTarget;
                }

                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
                SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null);
                _currentTransition.Render(SpriteBatch);
                SpriteBatch.End();
            }
            else if (_currentState != null)
            {
                DrawState(elapsedTime, SpriteBatch, _currentState);
            }
            base.Draw(gameTime);
        }
    }
}
