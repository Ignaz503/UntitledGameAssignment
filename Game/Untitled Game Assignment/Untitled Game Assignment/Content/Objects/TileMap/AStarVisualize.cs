using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Util.Input;
using Util.CustomDebug;
using GeoUtil.HelperCollections.Grids;

public class AStarVisualize : Component, IUpdate
{
    TestTileMap testMap;
    AStarTileMap astarMap;
    Path pFull;
    Path pSimple;

    bool sIsSet = false;
    bool eIsSet = false;
    Vector2 startTilePos;
    Vector2 endTilePos;

    public AStarVisualize(TestTileMap testVis, AStarTileMap star, GameObject obj ) : base( obj )
    {
        testMap = testVis;
        astarMap = star;
        pFull = null;
        pSimple = null;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        HandleReset();

        if (Input.IsKeyDown( Keys.Y ))
        {
            if (sIsSet)
                ColorTile( startTilePos, Color.White );

            startTilePos = Camera.Active.ScreenToWorld( Input.MousePosition );
            sIsSet = true;
        }
        if (Input.IsKeyDown( Keys.X ))
        {
            if (eIsSet)
                ColorTile( endTilePos, Color.White );

            endTilePos = Camera.Active.ScreenToWorld( Input.MousePosition );
            eIsSet = true;
        }

        HandleStartEndColoring();
        HandleCostIncrease();

        if (Input.IsKeyDown( Keys.Enter ))
        {
            sIsSet = eIsSet = false;
            if (pFull != null)
                ClearPath(pFull);
            if (pSimple != null)
                ClearPath(pSimple);

            Debug.Log( "Finding Path" );
            var res = PathFinder.FindPath(astarMap,startTilePos,endTilePos);
            if (res.WasSuccess)
            { 
                pFull = res.Path;
            }

            var sRes = PathFinder.FindPath(astarMap,startTilePos,endTilePos,simplifyPath:true);
            if (sRes.WasSuccess)
            {
                pSimple = sRes.Path;
            }

            if (sRes && res)
            {
                MarkPaths();
            }
        }

    }

    private void HandleReset()
    {
        if (Input.IsKeyDown( Keys.C ))
        { 
            ClearPath(pFull);
            ClearPath(pSimple);
        }
        if (Input.IsKeyDown( Keys.R ))
           Reset();
    }

    private void Reset()
    {
        for (int x = 0; x < testMap.Width; x++)
        {
            for (int y = 0; y < testMap.Height; y++)
            {
                astarMap[x, y].UpdateWalkCost( 1 );
                testMap[x, y].Tint = Color.White;
            }
        }

        if (eIsSet)
        { 
            eIsSet = false;
            ColorTile( endTilePos, Color.White );
        }

        if (sIsSet)
        {
            sIsSet = false;
            ColorTile( startTilePos, Color.White );
        }
    }

    private void HandleCostIncrease()
    {
        if (Input.IsKeyPressed( Keys.Q ))
        {
            var wPos = Camera.Active.ScreenToWorld( Input.MousePosition );

            var aT =astarMap[wPos];
            aT?.UpdateWalkCost(500);

            var tT =testMap[wPos];
            if(tT!= null) tT.Tint = Color.Blue;
        }  
        
        if (Input.IsKeyPressed( Keys.E ))
        {
            var wPos = Camera.Active.ScreenToWorld( Input.MousePosition );

            var aT =astarMap[wPos];
            aT?.UpdateWalkCost(1);

            var tT =testMap[wPos];
            if(tT!= null) tT.Tint = Color.White;
        }
    }

    private void HandleStartEndColoring()
    {
        if (sIsSet)
        {
            ColorTile( startTilePos, Color.Green );
        }
        if (eIsSet)
        {
            ColorTile( endTilePos,Color.Red );
        }
    }

    void ColorTile( Vector2 wPos, Color c )
    {
        var tPos = testMap.WorldPositionTolTile(wPos);

        var tile = testMap[tPos];
        if (tile != null)
        {
            tile.Tint = c;
        }
    }

    void ColorPath(Color c, Path p) 
    {
        if (p != null)
        {
            while (p.HasNext)
            {
                var cords = p.Current;

                testMap[cords].Tint = c;

                p.MoveForward();
            }
            var end = p.Current;

            testMap[end].Tint = c;

        }
    }

    void ClearPath(Path p) 
    {

        ColorPath( Color.White, p);

    }

    void MarkPaths() 
    {
        ColorPath( Color.Black, pFull);
        ColorPath( Color.Yellow, pSimple );
    }

}

