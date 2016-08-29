using System;
using System.Collections.Generic;
using System.Linq;
using BombRush.Interfaces;

namespace BombRush.Logic
{
    public class GameSessionImp : GameSession
    {
        private readonly string _levelAssetPath;
        private readonly Random _random;
        private readonly List<GameSessionMemberImp> _members;
        private int _matchesToWin;
        private readonly int _matchTimeSeconds;
        private byte _currentId;
        private LevelImp _currentLevel;
        private GameUpdateResult _updateResult;
        private readonly Func<int, FigureController> _provideCustomFigureController;

        private static readonly string[] ComNames =
        {
            "Oliver", "Daniel", "Thomas", "Andre", 
            "Richard", "Nils", "Katja", "Tanja",
            "Robert", "Antje", "Marie", "Rene",
            "Erik", "Stephan", "Michael",
            "Enrico",
            "Mario"
        };

        private string GetUniqueComFigureName()
        {
            var name = ComNames[_random.Next(ComNames.Length)];
            while (_members.Where(m => m.Type == MemberType.Computer).Any(m => m.Name == name))
            {
                name = ComNames[_random.Next(ComNames.Length)];
            }
            return name;
        }

        public GameSessionImp(GameSessionStartParameters parameters)
        {
            if (parameters.ProvidePlayerFigureController == null) throw new ArgumentException("ProvidePlayerFigureController is null");

            _random = new Random();
            _currentId = 1;
            _members = new List<GameSessionMemberImp>();

            State = GameSessionState.Disconnected;
            SessionName = parameters.SessionName;
            _provideCustomFigureController = parameters.ProvidePlayerFigureController;
            _matchesToWin = parameters.MatchesToWin;
            _matchTimeSeconds = parameters.MatchTime;
            _levelAssetPath = parameters.LevelAssetPath;
        }

        private int GetNextFreePlayerIndex()
        {
            var index = 1;
            while (_members.Where(m => m.PlayerIndex > 0).Any(m => m.PlayerIndex == index)) index++;

            if (index > 4) throw new InvalidOperationException("More than 4 active players per session are not valid");

            return index;
        }

        public void AddMember(MemberType memberType, string name = null)
        {
            var member = new GameSessionMemberImp(this, _currentId++, name, memberType);

            if (memberType != MemberType.Watcher) member.PlayerIndex = GetNextFreePlayerIndex();
            GenerateDefaultNameIfEmpty(memberType, member);
                        
            _members.Add(member);
        }

        private void GenerateDefaultNameIfEmpty(MemberType memberType, GameSessionMemberImp member)
        {
            if (!string.IsNullOrEmpty(member.Name)) return;

            switch (memberType)
            {
                case MemberType.ActivePlayer:
                    member.Name = "Player" + member.PlayerIndex;
                    break;
                case MemberType.Computer:
                    member.Name = GetUniqueComFigureName();
                    break;
                case MemberType.Watcher:
                    member.Name = "Watcher";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(memberType), memberType, null);
            }
        }

        public void StartMatch()
        {
            State = GameSessionState.PreparingMatchLoadData;
        }

        public string SessionName { get; }

        public Level CurrentLevel => _currentLevel;
        public GameSessionState State { get; private set; }
        public float RemainingStartupTime { get; private set; }
        public MatchResultType CurrentMatchResultType { get; private set; }
        public bool IsRunningAsLocalGame { get; private set; }

        public void OnQuit()
        {
            throw new NotImplementedException();
        }

        public GameUpdateResult Update(float elapsedTime)
        {
            _updateResult = GameUpdateResult.None;

            switch(State)
            {
                case GameSessionState.PreparingMatchLoadData:
                    HandlePrepareMatch();
                    break;
                case GameSessionState.PreparingMatchSynchronizeStart:
                    HandleSynchronizeStart(elapsedTime);
                    break;
                case GameSessionState.InGame:
                    HandleInGame(elapsedTime);
                    break;
            }

            return _updateResult;
        }

        private void HandlePrepareMatch()
        {
            _currentLevel = new LevelImp(_matchTimeSeconds);
            _currentLevel.GenerateLevel(_levelAssetPath);
            foreach(var member in _members.Where(m => m.Type != MemberType.Watcher))
            {
                _currentLevel.AddFigure(member.SpawnFigure(_currentLevel));
            }

            State = GameSessionState.PreparingMatchSynchronizeStart;
            RemainingStartupTime = 3;
        }

        private void HandleInGame(float elapsedTime)
        {
            var resulType = _currentLevel.Update(elapsedTime);
            if(resulType == MatchResultType.None) return;

            CurrentMatchResultType = resulType;
            _updateResult = GameUpdateResult.MatchFinished;
            State = GameSessionState.MatchResult;
        }

        private void HandleSynchronizeStart(float elapsedTime)
        {
            RemainingStartupTime -= elapsedTime;
            if(RemainingStartupTime <= 0)
            {
                State = GameSessionState.InGame;
                _updateResult = GameUpdateResult.SwitchToGame;
            }
        }

        public GameSessionMember[] Members
        {
            get { return _members.Cast<GameSessionMember>().ToArray(); }
        }

        internal FigureController GetFigureController(int playerIndex)
        {
            return _provideCustomFigureController(playerIndex);
        }
    }
}
