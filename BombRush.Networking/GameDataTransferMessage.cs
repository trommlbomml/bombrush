using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BombRush.Interfaces;
using Microsoft.Xna.Framework;

namespace BombRush.Networking
{
    public class GameDataTransferMessage : Message
    {
        public struct MapData
        {
            public BlockType GroundBlockType;
            public BlockType FringeBlockType;
            public ItemType FringeItemType;
        }

        public Vector2[] StartUpPositions { get; private set; }
        public string TileSetAssetName { get; private set; }
        public MapData[] Data { get; private set; }

        public GameDataTransferMessage()
        {            
        }

        public GameDataTransferMessage(double timeStamp, Vector2[] startUpPositions, string tileSetAssetName, IEnumerable<MapData> mapData)
            : base(timeStamp)
        {
            StartUpPositions = startUpPositions;
            TileSetAssetName = tileSetAssetName;
            Data = mapData.ToArray();

            Debug.Assert(StartUpPositions.Length == 4);
            //todo: refactor usage
            //Debug.Assert(Data.Length == GameLevel.GameLevelWidth * GameLevel.GameLevelHeight);
        }

        protected override void ReadFrom(Lidgren.Network.NetIncomingMessage incomingMessage)
        {
            base.ReadFrom(incomingMessage);

            StartUpPositions = new Vector2[4];
            for (var i = 0; i < 4; i++)
            {
                StartUpPositions[i].X = incomingMessage.ReadFloat();
                StartUpPositions[i].Y = incomingMessage.ReadFloat();
            }
            
            TileSetAssetName = incomingMessage.ReadString();

            //todo: refactor usage
            Data = new MapData[15 * 13];
            for(var i = 0; i < Data.Length; i++)
            {
                Data[i].GroundBlockType = (BlockType)incomingMessage.ReadByte();
                Data[i].FringeBlockType = (BlockType)incomingMessage.ReadByte();
                Data[i].FringeItemType = (ItemType)incomingMessage.ReadByte();
            }
        }

        protected override void WriteTo(Lidgren.Network.NetOutgoingMessage outgoingMessage)
        {
            base.WriteTo(outgoingMessage);

            for (var i = 0; i < 4; i++)
            {
                outgoingMessage.Write(StartUpPositions[i].X);
                outgoingMessage.Write(StartUpPositions[i].Y);
            }

            outgoingMessage.Write(TileSetAssetName);

            for (var i = 0; i < Data.Length; i++)
            {
                outgoingMessage.Write((byte) Data[i].GroundBlockType);
                outgoingMessage.Write((byte) Data[i].FringeBlockType);
                outgoingMessage.Write((byte) Data[i].FringeItemType);
            }
        }
    }
}
