using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.DataStructure
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        enum ChildOrder 
        {
            First = 1,
            Second = 2
        }

        List<T> data;

        public int Count => data.Count;

        public bool HasEntries => Count > 0;

        public PriorityQueue()
        {
            data = new List<T>();
        }

        public void Enqueue( T item ) 
        {
            data.Add( item );

            int itemIdx = data.Count - 1;

            //ensure correctness
            CascadeUp( itemIdx );
        }

        int GetParentIndex( int idx ) 
        {
            return (idx - 1) / 2;
        }

        int GetChild( int parentIdx, ChildOrder order ) 
        {
            return (2 * parentIdx) + (int)order;
        }

        public T Dequeue() 
        {            
            var item = data[0];
            //remove item with last index swap trick for efficency
            int end = data.Count -1;
            data[0] = data[end];
            data.RemoveAt( end );

            //ensure correctness
            CascadeDown();

            return item;
        }

        /// <summary>
        /// cascades node down to correct position
        /// </summary>
        /// <param name="currentNode">the node to cascade down, default root</param>
        private void CascadeDown(int currentNode = 0)
        {
            int end = data.Count - 1;

            while (true)//loop until correct
            {
                int childToSwap = GetChild(currentNode,ChildOrder.First);//get first child

                if (childToSwap > end)//swaped to end, no more children
                    break;

                int secondChild = GetChild(currentNode,ChildOrder.Second);

                if (secondChild <= end)//a second child exists
                    if (data[secondChild].CompareTo( data[childToSwap] ) < 0)//second child has higher priority
                        childToSwap = secondChild;//we now swap with second child

                if (data[currentNode].CompareTo( data[childToSwap] ) <= 0)//parent has higher or equal priority as child to swap
                    break;

                //we need to swap
                Swap( currentNode, childToSwap );
                currentNode = childToSwap;
            }
        }

        /// <summary>
        /// cascades item idex up to ensure correctness
        /// </summary>
        /// <param name="itemIdx">index of item to cascade down</param>
        private void CascadeUp( int itemIdx ) 
        {
            while (itemIdx > 0)// do untill we are root or break out
            {
                int parentIdx = GetParentIndex(data.Count-1);

                if (data[itemIdx].CompareTo( data[parentIdx] ) >= 0)
                    break;//item is lower priority than parent so we are at correct location
                //we are higher in priority than parent
                //so we swap
                Swap( parentIdx, itemIdx );
                itemIdx = parentIdx;
            }
        }

        /// <summary>
        /// peek first item
        /// </summary>
        /// <returns>the first item in queue</returns>
        public T Peek() 
        {
            return data[0];
        }

        /// <summary>
        /// swaps two data entries
        /// </summary>
        /// <param name="idx1"></param>
        /// <param name="idx2"></param>
        void Swap( int idx1, int idx2 ) 
        {
            T tmp = data[idx1];
            data[idx1] = data[idx2];
            data[idx2] = tmp;
        }

        /// <summary>
        /// clears data
        /// </summary>
        void Clear() 
        {
            data.Clear();
        }

        public bool Contains( T item ) 
        {
            //TODO make smarter
            return data.Contains( item );
        }

    }
}
