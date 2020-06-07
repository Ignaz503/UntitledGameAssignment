using System;
using System.Collections.Generic;
using UntitledGameAssignment;

namespace Util.SortingLayers
{
    /// <summary>
    /// sprite draw sorting layer used by batch renderer
    /// </summary>
    public struct SortingLayer
    {
        /// <summary>
        /// max layer number
        /// used to convert to layer dept
        /// higher layer means further back
        /// </summary>
        static int _max = 1;
        /// <summary>
        /// max layer number accessor
        /// </summary>
        static int max 
        {
            get { return _max; }
            set { _max = System.Math.Max( _max, value ); }
        }

        public static SortingLayer Background => 0;
        public static SortingLayer Entities => 10;
        public static SortingLayer UI => 20;

        public static SortingLayer EntitesSubLayer( int subLayer ) => Entities + subLayer;

        public static SortingLayer BackgroundSubLayer( int subLayer ) => Background + subLayer;

        public static SortingLayer UISubLayer( int subLayer ) => UI + subLayer;
        /// <summary>
        /// the value of this layer
        /// </summary>
        int value;

        private SortingLayer(int layer)
        {
            this.value = layer;

            //try update max value
            max = layer;

        }

        public static implicit operator int( SortingLayer layer) 
        {
            return layer.value;
        }

        public static implicit operator SortingLayer( int value )
        {
            return new SortingLayer( value );
        }

        public static implicit operator float( SortingLayer layer ) 
        {
#if REVERSE_SORT
            return 1.0f - ((float)layer.value / (float)max);
#else
            return (float)layer.value / (float)max;
#endif
        }

        public static explicit operator SortingLayer( float value )
        {
            return new SortingLayer( (int)System.Math.Round( value * max ) );
        }


        public int CompareTo( SortingLayer other ) 
        {
            if (this < other)
                return -1;
            else if (this > other)
                return 1;
            return 0;
        }

        public override string ToString()
        {
            return value.ToString();
        }

    }

    public struct SortingLayerComparer : IComparer<SortingLayer>
    {
        public int Compare( SortingLayer x, SortingLayer y )
        {
            return Comparer<int>.Default.Compare( x, y );
        }
    }

}
