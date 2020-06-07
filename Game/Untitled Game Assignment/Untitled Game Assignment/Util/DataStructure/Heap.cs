using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.DataStructure
{
    public class Heap<T> where T : IHeapItem<T>
    {
        T[] data;

        int currentCount;

        public int Count => currentCount;

        public bool HasEntries => Count > 0;

        public Heap(int sizeMax)
        {
            data = new T[sizeMax];
            currentCount = 0;
        }

        public void Insert( T obj ) 
        {
            obj.HeapIndex = currentCount;

            data[currentCount] = obj;

            CascadeUp( obj );

            currentCount++;
        }

        public T Take() 
        {
            T first = data[0];
            currentCount--;
            data[0] = data[currentCount];
            data[0].HeapIndex = 0 ;
            CascadeDown( data[0] );
            return first;
        }

        public void UpdateItemUp( T obj ) 
        {
            CascadeUp( obj );
        }

        public void UpdateItemDown( T obj ) 
        {
            CascadeDown( obj );
        }

        public bool Contains( T obj ) 
        {
            return Equals( obj, data[obj.HeapIndex]);
        }

        private int GetParentIndex( T obj ) 
        {
            return (obj.HeapIndex - 1) / 2;
        }

        private int GetLeftChild( T obj ) 
        {
            return obj.HeapIndex * 2 + 1;
        }

        private int GetRightChild( T obj ) 
        {
            return obj.HeapIndex * 2 + 2;
        }

        private void CascadeDown( T obj )
        {
            while (true)
            {
                int leftChild = GetLeftChild(obj);
                int rightChild = GetRightChild(obj);

                int swap = 0;

                if (leftChild < currentCount)
                {
                    //not at end
                    swap = leftChild;

                    if (rightChild < currentCount && data[leftChild].CompareTo( data[rightChild] ) < 0)
                        swap = rightChild;

                    if (obj.CompareTo( data[swap] ) < 0)
                        Swap( obj, data[swap] );
                    else
                        return;
                } else
                    return;
            }
        }

        private void CascadeUp( T obj )
        {
            int parent = GetParentIndex(obj);

            while (true)
            {
                var parentData = data[parent];
                if (obj.CompareTo( parentData ) > 0)
                    Swap( obj, parentData );
                else
                    break;
                parent = GetParentIndex(obj);//index updated from swap therefore new parent index
            }

        }

        private void Swap( T A, T B )
        {
            data[A.HeapIndex] = B;
            data[B.HeapIndex] = A;
            var temp = A.HeapIndex;
            A.HeapIndex = B.HeapIndex;
            B.HeapIndex = temp;
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }
}
