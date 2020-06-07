using System;
using Microsoft.Xna.Framework;
using Util.CustomMath;
namespace GeoUtil.HelperCollections.Trees
{
    class QTNode<Dt>
    {
        public Vector2 Position { get; protected set; }
        public Dt Data { get; protected set; }

        public QTNode(Vector2 pos, Dt data)
        {
            this.Position = pos;
            this.Data = data;
        }

        public QTNode()
        {
            Data = default(Dt);
        }
    }

    class QTQuad<Dt>
    {
        Bounds2D bounds;

        QTNode<Dt> node;

        QTQuad<Dt> bottomLeftTree;
        QTQuad<Dt> bottomRightTree;
        QTQuad<Dt> topLeftTree;
        QTQuad<Dt> topRightTree;

        protected QTQuad()
        {
            node = default;
            bottomLeftTree = default;
            bottomRightTree = default;
            topLeftTree = default;
            topRightTree = default;
        }

        public QTQuad(Bounds2D bounds, QTNode<Dt> node, QTTree<Dt> tree):this()
        {
            this.bounds = bounds;
            this.node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public QTQuad(Bounds2D bounds):this()
        {
            this.bounds = bounds;            
        }

        public QTQuad(Vector2 min, Vector2 max):this(new Bounds2D(min,max))
        {}

        public void Insert(QTNode<Dt> insertNode)
        {
            if (insertNode == null)//
                return;

            if (!Contains(insertNode.Position))//not our concern
                return;

            var v = bounds.Max - bounds.Min;
            VectorMath.Abs( ref v );

            //minimum divison reached
            if (v.X <= 1 && v.Y <= 1)
            {
                //if no data yet: this is our data
                if (node == null)
                    node = insertNode;
                return;
            }

            if (bounds.Center.X >= insertNode.Position.X)
            {
                //on left
                if (bounds.Center.Y >= insertNode.Position.Y)
                {
                    if (bottomLeftTree == null)
                    {
                        bottomLeftTree = new QTQuad<Dt>(
                            bounds.Min, bounds.Center);
                    }
                    bottomLeftTree.Insert(insertNode);
                }
                else
                {
                    if (topLeftTree == null)
                    {
                        topLeftTree = new QTQuad<Dt>(
                            new Vector2(bounds.Min.X, bounds.Center.Y),
                            new Vector2(bounds.Center.X, bounds.Max.Y));
                    }
                    topLeftTree.Insert(insertNode);
                }
            }
            else
            {
                //on right
                if (bounds.Center.Y >= insertNode.Position.Y)
                {
                    if (bottomRightTree == null)
                    {
                        bottomRightTree = new QTQuad<Dt>(
                            new Vector2(bounds.Center.X, bounds.Min.Y),
                            new Vector2(bounds.Max.X, bounds.Center.Y));
                    }
                    bottomRightTree.Insert(insertNode);
                }
                else
                {
                    if (topRightTree == null)
                    {
                        topRightTree = new QTQuad<Dt>(bounds.Center, bounds.Max);
                    }
                    topRightTree.Insert(insertNode);
                }
            }
        }

        public QTNode<Dt> Search(Vector2 point)
        {
            if (!Contains(point))
                return null;
            //unit quad return data
            if (node != null)
                return node;

            if (bounds.Center.X > point.X)
            {
                //left
                if (bounds.Center.Y >= point.Y)
                {
                    if (bottomLeftTree == null)
                        return null;
                    return bottomLeftTree.Search(point);
                }
                else
                {
                    if (topLeftTree == null)
                        return null;
                    return topLeftTree.Search(point);
                }
            }
            else
            {
                //right
                if (bounds.Center.Y >= point.Y)
                {
                    if (bottomRightTree == null)
                        return null;
                    return bottomRightTree.Search(point);
                }
                else
                {
                    if (topRightTree == null)
                        return null;
                    return topRightTree.Search(point);
                }
            }
        }

        public bool Contains(Vector2 p)
        {
            return bounds.Contains(p);
        }
    }

    public struct QTQueryResult<Dt> 
    {
        public Vector2 Pos;
        public Dt Data;

        public QTQueryResult( Vector2 pos, Dt data )
        {
            Pos = pos;
            Data = data;
        }
    }

    public class QTTree<Dt>
    {
        public float scale;
        QTQuad<Dt> root;

        public QTTree(Bounds2D bounds, float scale = 1f)
        {
            this.scale = scale;
            root = new QTQuad<Dt>(bounds.Scale(scale));
        }

        public void Insert(Vector2 position, Dt data)
        {
            root.Insert(new QTNode<Dt>(position * scale, data));
        }

        public QTQueryResult<Dt>? Search(Vector2 point)
        {
            var found = root.Search(point);
            if (found != null)
            {
                return new QTQueryResult<Dt>(found.Position / scale, found.Data);
            }
            return null;
        }
    }

}
