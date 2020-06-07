using System;
using Microsoft.Xna.Framework;
using GeoUtil.HelperCollections.Grids;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Util.CustomMath;
using System.Collections.Generic;

public enum Neighborhood
{
    Full,
    Cross
}

public abstract class TileMap<T> : Component
    where T : Tile
{

    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public int Size => Width * Height;

    public Vector2 TileSize { get; protected set; }

    protected T[,] tiles;

    public Neighborhood NeighborhoodCalculationType { get; protected set; }

    public TileMap( int width, int height, Vector2 tileSpriteSize, GameObject obj,Neighborhood neighborCalc= Neighborhood.Cross ) : base( obj )
    {
        Width = width;
        Height = height;
        TileSize = tileSpriteSize;
        NeighborhoodCalculationType = neighborCalc;
        GenerateTiles();
    }

    private void GenerateTiles()
    {
        tiles = new T[Width, Height];


        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                GenerateTileAt( x, y );
            }
        }

    }

    protected abstract void GenerateTileAt( int x, int y );
    public T this[int x, int y]
    {
        get { return GetTileAt( x, y ); }
    }

    public T this[Vector2Int p]
    {
        get { return GetTileAt( p.X, p.Y ); }
    }

    public T this[Vector2 wPos]
    {
        get { return this[WorldPositionTolTile( wPos )]; }
    }

    public T GetTileAt( int x, int y )
    {
        if (!IsInRange( x, y ))
            return null;
        return tiles[x, y];
    }

    public Vector2Int GetTileCoords( Vector2 worldPos ) 
    {
        throw new NotImplementedException();
    }

    public Vector2 TilePositionToWorld( int x, int y ) 
    {
        return Transform.Position + (new Vector2( x, y ) * TileSize);
    }

    public bool IsInRange( int x, int y ) 
    {
        if (x < 0 || x >= tiles.GetLength( 0 ) || y < 0 || y >= tiles.GetLength( 1 ))
            return false;
        return true;
    }

    public List<T> GetNeighbors( int x, int y ) 
    {
        switch (NeighborhoodCalculationType)
        {
            case Neighborhood.Full:
                return CalculateFullNeighborhood(x,y);
            case Neighborhood.Cross:
                return CalculateCrossNeighborhood(x,y);
            default:
                return CalculateFullNeighborhood(x,y);
        }
    }

    private List<T> CalculateCrossNeighborhood( int xPos, int yPos )
    {
        var neighbors = new List<T>();

        //keeping order as similar as possible to 3x3 neighbors
        Check( 0, -1 ); //up
        Check( -1, 0 );//left
        Check( 1, 0 ); //right
        Check( 0, 1 ); //down

        return neighbors;

        void Check(int xChange, int yChange )
        {
            var checkX = xPos + xChange;
            var checkY = yPos + yChange;

            if (IsInRange( checkX, checkY ))
            {
                neighbors.Add( tiles[checkX, checkY] );
            }

        }
    }

    private List<T> CalculateFullNeighborhood( int xPos, int yPos )
    {
        var neighbors = new List<T>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (!(x == 0 && y == 0))
                {
                    int checkX = xPos + x;
                    int checkY = yPos + y;
                    if (IsInRange(checkX,checkY))
                    {
                        neighbors.Add( tiles[checkX, checkY] );
                    }
                }
            }
        }
        return neighbors;
    }


    public Vector2 TilePositionToWorld( Vector2Int v ) 
    {
        return TilePositionToWorld( v.X, v.Y );    
    }

    public Vector2Int WorldPositionTolTile( Vector2 wPos ) 
    {
        var l = (wPos - Transform.Position)/TileSize;
        return l.RoundToInt();
    }
}
