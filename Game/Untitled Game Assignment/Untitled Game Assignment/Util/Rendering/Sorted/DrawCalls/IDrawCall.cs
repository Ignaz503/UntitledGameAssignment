using System;
using Util.SortingLayers;

namespace Util.Rendering
{
    public interface IDrawCall : IComparable<IDrawCall>
    {
        SortingLayer Layer { get; }
        void MakeCall();
    }
}
