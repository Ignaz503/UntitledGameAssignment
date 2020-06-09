using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Util.DataStructure;
using Util.CustomDebug;
public static class PathFinder
{
    const float SimilarityError = 0.001f;

    public struct Result 
    {
        public bool WasSuccess;
        public Path Path;

        public Result( bool wasSuccess, Path path )
        {
            this.WasSuccess = wasSuccess;
            this.Path = path;
        }

        public static implicit operator bool( Result res ) 
        {
            return res.WasSuccess;
        }

    }

    public static Result FindPath( AStarTileMap map, Vector2 worldPosFrom, Vector2 worldPosTo, bool simplifyPath = false) 
    {
        bool success = false;

        var start = map[worldPosFrom];
        var end = map[worldPosTo];
        if (end == null || start == null)
            return new Result( success, null );

        if (start.Walkable && end.Walkable)
        {
            Heap<AStarTile> openSet = new Heap<AStarTile>(map.Size);
            HashSet<AStarTile> closedSet = new HashSet<AStarTile>();
            openSet.Insert( start );
            while (openSet.HasEntries)
            {
                var currentNode = openSet.Take();
                closedSet.Add( currentNode );

                if (currentNode == end)
                {
                    success = true;
                    break;
                }

                var neighbors = map.GetNeighbors(currentNode.TilePosition.X,currentNode.TilePosition.Y);

                for (int i = 0; i < neighbors.Count; i++)
                {
                    var n = neighbors[i];

                    if (closedSet.Contains( n ) || !n.Walkable)
                        continue;//already visited or not wakable

                    var dist = CaluclateDistance(currentNode,n);
                    int moveCost = currentNode.globalCost + dist + n.WalkCost;
                    if (moveCost < n.globalCost || !openSet.Contains( n ))//we found cheaper path, or we have never been here before
                    {
                        n.globalCost = moveCost;
                        n.heuristicCost = dist;
                        n.Parent = currentNode;

                        if (!openSet.Contains( n ))
                            openSet.Insert( n );
                        else
                            openSet.UpdateItemUp( n );
                    }

                }


            }//end while
        }//end if start and end wakable

        Path p = null;
        if (success)
        {
            if (simplifyPath)
                p = BuildPathSimplified( map, start, end );
            else
                p = BuildPath(map,start,end);
        }
        return new Result( success, p );
    }

    private static int CaluclateDistance( AStarTile n0, AStarTile n1 )
    {
        int distX = Math.Abs(n0.TilePosition.X - n1.TilePosition.X);
        int distY = Math.Abs(n0.TilePosition.Y - n1.TilePosition.Y);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        else
            return 14 * distX + 10 * (distY - distX);
    }

    private static Path BuildPathSimplified(AStarTileMap map, AStarTile start, AStarTile end )
    {
        List<Vector2> path = new List<Vector2>();

        var current = end;

        var prevDirection = Vector2.Zero;
        var prevPos = GetWorldPos(map, end);

        while (current != start)
        {
            var currentPos = GetWorldPos(map, current);

            var newDir = currentPos - prevPos;
            newDir.Normalize();

            if (Vector2.Dot( newDir, prevDirection ) <= (1-SimilarityError))
            {
                Debug.Log( $"Adding new path point, similarity old {Vector2.Dot( newDir, prevDirection )}" );
                path.Add( currentPos );
            }

            prevPos = currentPos;
            prevDirection = newDir;

            current = current.Parent;
        }
        path.Add( GetWorldPos(map,start) );
        var res = path.ToArray();
        Array.Reverse( res );

        return new Path(res );
    }

    static Vector2 GetWorldPos( AStarTileMap map, AStarTile t )
    {
        return map.TilePositionToWorld( t.TilePosition );
    }

    private static Path BuildPath(AStarTileMap map, AStarTile start, AStarTile end )
    {
        List<Vector2> path = new List<Vector2>();

        var current = end;

        while (current != start)
        {
            var currentPos = GetWorldPos(map, current);
            path.Add( currentPos );
            current = current.Parent;
        }
        path.Add( GetWorldPos(map,start) );
        var res = path.ToArray();
        Array.Reverse( res );

        return new Path(res );
    }
}

