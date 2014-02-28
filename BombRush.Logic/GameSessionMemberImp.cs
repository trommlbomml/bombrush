using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BombRush.Interfaces;
using BombRush.Logic;
using BombRush.Logic.AI;

namespace BombRush.Logic
{
    public class GameSessionMemberImp : GameSessionMember
    {
        private GameSessionImp Session { get; set; }

        public GameSessionMemberImp(GameSessionImp session, byte id, string name, MemberType memberType)
        {
            Session = session;
            Id = id;
            Type = memberType;
            Name = name;
        }

        public byte Id { get; private set; }
        public MemberType Type { get; private set; }
        public int PlayerIndex { get; set; }
        public string Name { get; set; }
        public int Wins { get; set; }

        public FigureImp SpawnFigure(LevelImp level)
        {
            switch(Type)
            {
                case MemberType.Computer:
                    return CreateComputerFigure(level);
                case MemberType.ActivePlayer:
                    return CreatePlayerFigure();
                default:
                    throw new InvalidOperationException("Watcher do not get figure");
            }
        }

        private FigureImp CreatePlayerFigure()
        {
            var figure = new FigureImp(Id)
            {
                FigureController = Session.GetFigureController(PlayerIndex)
            };
            return figure;
        }

        private FigureImp CreateComputerFigure(LevelImp level)
        {
            var figure = new FigureImp(Id)
            {
                FigureController = new ComFigureController(level)
            };
            return figure;
        }
    }
}
