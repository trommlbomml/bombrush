using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BombRush.Interfaces
{
    public interface Level
    {
        float RemainingGameTime { get; }

        List<Figure> Figures { get; }
        List<Bomb> Bombs { get; }
        TileBlock[] Data { get; }
        TileBlock[] Fringe { get; }
        Item[] ItemData { get; }
        ExplosionFragment[] OverlayData { get; }
        ExplosionFragment GetOverlayInformation(Point t);

        Vector2 GetWorldFromTilePositionCentered(Point p);
        Vector2 GetWorldFromTilePosition(Point p);
        Point GetTilePositionFromWorld(Vector2 position);
    }
}
