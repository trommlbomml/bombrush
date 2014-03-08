using BombRush.Interfaces;
using BombRush.Networking.Extensions;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace BombRush.Networking.ServerMessages
{
    public struct FigureInformation
    {
        public byte Id;
        public Vector2 Position;
        public Vector2 WalkDirection;
        public FigureDirection Direction;
        public float Speed;
        public bool IsAlive;
        public bool IsVisible;
    }

    public struct MapDataInformation
    {
        public bool FringeActive;
        public ItemType Item;
        public bool OverlayIsActive;
    }

    public struct BombInformation
    {
        public byte Id;
        public byte BombType;
        public Vector2 Position;
    }

    public class GameStatusMessage : Message
    {
        public float RemainingGameTime { get; private set; }
        public FigureInformation[] Figures { get; private set; }
        public MapDataInformation[] MapData { get; private set; }
        public BombInformation[] Bombs { get; private set; }

        public override NetDeliveryMethod DeliveryMethod
        {
            get
            {
                return NetDeliveryMethod.UnreliableSequenced;
            }
        }

        public GameStatusMessage()
        {
            
        }
        
        public GameStatusMessage(double timeStamp, float remainingGameTime, Level gameLevel) : base(timeStamp)
        {
            RemainingGameTime = remainingGameTime;
            Figures = new FigureInformation[gameLevel.Figures.Count];
            int f = 0;
            foreach (Figure figure in gameLevel.Figures)
            {
                Figures[f++] = new FigureInformation
                {
                    Direction = figure.Direction,
                    Id = figure.Id,
                    IsAlive = figure.IsAlive,
                    Position = figure.Position,
                    Speed = figure.Speed,
                    WalkDirection = figure.WalkDirection,
                    IsVisible = figure.IsVisible
                };
            }

            int gameDataLength = gameLevel.Data.Length;
            MapData = new MapDataInformation[gameDataLength];
            for (int i = 0; i < gameDataLength; i++)
            {
                var overlay = gameLevel.OverlayData[i];

                MapData[i] = new MapDataInformation
                {
                    FringeActive = gameLevel.Fringe[i].IsActive,
                    Item = gameLevel.ItemData[i].IsActive ? gameLevel.ItemData[i].Type : ItemType.Empty,
                    
                    OverlayIsActive = overlay.IsActive,
                };
            }

            Bombs = new BombInformation[gameLevel.Bombs.Count];
            int bbb = 0;
            foreach (Bomb bombDataProvider in gameLevel.Bombs)
            {
                Bombs[bbb].Id = bombDataProvider.Id;
                Bombs[bbb].BombType = (byte)bombDataProvider.BombType;
                Bombs[bbb].Position = gameLevel.GetWorldFromTilePosition(bombDataProvider.TilePosition);
                bbb++;
            }
        }

        protected override void WriteTo(NetOutgoingMessage msg)
        {
            base.WriteTo(msg);

            msg.Write(RemainingGameTime);
            msg.Write((byte)Figures.Length);

            foreach (FigureInformation f in Figures)
            {
                msg.Write(f.Id, 4);
                msg.Write(f.IsAlive);
                msg.Write((byte)f.Direction, 3);
                msg.WritePackedVector(f.Position);
                msg.WritePackedVector(f.WalkDirection);
                msg.Write(f.Speed);
                msg.Write(f.IsVisible);
            }

            msg.Write((byte)MapData.Length);
            foreach (MapDataInformation mapDataInformation in MapData)
            {
                msg.Write(mapDataInformation.FringeActive);
                msg.Write((byte)mapDataInformation.Item, 3);
                msg.Write(mapDataInformation.OverlayIsActive);
            }

            msg.Write((byte) Bombs.Length);
            foreach (BombInformation bombInformation in Bombs)
            {
                msg.Write(bombInformation.Id, 7);
                msg.WritePackedVector(bombInformation.Position);
                msg.Write(bombInformation.BombType, 3);
            }

            msg.WritePadBits();
        }

        protected override void ReadFrom(NetIncomingMessage msg)
        {
            base.ReadFrom(msg);

            RemainingGameTime = msg.ReadFloat();
            int figureCount = msg.ReadByte();

            Figures = new FigureInformation[figureCount];
            for(int i = 0; i < figureCount; i++)
            {
                Figures[i] = new FigureInformation
                {
                    Id = msg.ReadByte(4),
                    IsAlive = msg.ReadBoolean(),
                    Direction = (FigureDirection) msg.ReadByte(3),
                    Position = msg.ReadPackedVector2(),
                    WalkDirection = msg.ReadPackedVector2(),
                    Speed = msg.ReadFloat(),
                    IsVisible = msg.ReadBoolean()
                };
            }

            int mapLength = msg.ReadByte();
            MapData = new MapDataInformation[mapLength];
            for(int i = 0; i < mapLength; i++)
            {
               MapDataInformation m = new MapDataInformation
                {
                    FringeActive = msg.ReadBoolean(),
                    Item = (ItemType)msg.ReadByte(3),
                    OverlayIsActive = msg.ReadBoolean()
                };

                MapData[i] = m;
            }

            int bombLength = msg.ReadByte();
            Bombs = new BombInformation[bombLength];
            for(int i = 0; i < bombLength; i++)
            {
                Bombs[i].Id = msg.ReadByte(7);
                Bombs[i].Position = msg.ReadPackedVector2();
                Bombs[i].BombType = msg.ReadByte(3);
            }

            msg.ReadPadBits();
        }
    }
}
