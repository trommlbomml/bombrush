using System.Net;
using Microsoft.Xna.Framework;
using BombRush.Interfaces;
using BombRush.Logic;

namespace BombRush.Networking
{
    public class ClientInformation
    {
        public IPEndPoint EndPoint;
        public bool Ready;
        public bool LoadForGameReady;
        public FigureImp Figure;

        public byte Id { get { return Figure.Id; } }
        public string Name { get { return Figure.Name; } }
        public bool IsAlive { get { return Figure.IsAlive; } }

        public ClientInformation(IPEndPoint endPoint, FigureImp referringFigure)
        {
            EndPoint = endPoint;
            Ready = false;
            Figure = referringFigure;
        }

        public void ResetForNextMatch()
        {
            LoadForGameReady = false;
            Ready = false;
            //todo: refactor network interfaces
            //Figure.IsMatchWinner = false;
            //Figure.WalkDirection = Vector2.Zero;
        }
    }
}
