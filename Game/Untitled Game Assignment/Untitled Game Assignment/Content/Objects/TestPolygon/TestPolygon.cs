using GeoUtil.Polygons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Util.AssetManagment;
using Util.Rendering;

public class TestPolygon : GameObject
{
    public TestPolygon() : base()
    {
        var sc = AddComponent((obj)=>new PolygonContainer(
                                            new Polygon(new Vector2[]
                                            {
                                                new Vector2(-.5f,-.5f),
                                                new Vector2(-.5f,.5f),
                                                new Vector2(.5f,.5f),
                                                new Vector2(.5f,-.5f)
                                            }),
                                            obj)) ;

        AddComponent( ( obj ) => new PolygonRenderer( sc, 0, AssetManager.Load<Texture2D>( "Sprites/WhiteSquare" ), UnsortedBatchRenderer.BasicEffect, obj ) );


        AddComponent( ( obj ) => new MovementController( obj,
                                                         up: Keys.T,
                                                         down: Keys.G,
                                                         left: Keys.F,
                                                         right: Keys.H ) );
        AddComponent( ( obj ) => new KeyBasedRotationController( obj, Keys.R, Keys.Z, changeInDegrees: 45f ) );
    }
}