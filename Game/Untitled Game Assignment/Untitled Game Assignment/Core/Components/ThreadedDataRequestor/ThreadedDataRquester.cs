using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UntitledGameAssignment.Core.GameObjects;

namespace UntitledGameAssignment.Core.Components 
{
    public class ThreadedDataRequestor : Component, IUpdate 
    {
        static ThreadedDataRequestor instance;

        public static ThreadedDataRequestor Instance {
            get {
                if (instance == null) {
                    instance = CreateInstance();
                }
                return instance;
            }
        }

        Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

        public void RequestData(Func<object> generateData, Action<object> callback)
        {
            ThreadStart tStart = delegate {
                instance.DataThread(generateData, callback);
            };
            Thread t = new Thread(tStart) { IsBackground = true };
            t.Start();
        }

        void DataThread(Func<object> generateData, Action<object> callback)
        {

            object data = generateData();

            lock (dataQueue)
            {
                dataQueue.Enqueue(new ThreadInfo(callback, data));
            }
        }
        public void Update()
        {
            HandleDataQueue();
        }

        void HandleDataQueue()
        {
            if (dataQueue.Count > 0)
            {
                ThreadInfo info;
                for (int i = 0; i < dataQueue.Count; i++)
                {
                    lock (dataQueue) {
                        info = dataQueue.Dequeue();
                    }
                    info.callback(info.parameter);
                }
            }
        }

        struct ThreadInfo
        {
            public readonly Action<object> callback;
            public readonly object parameter;

            public ThreadInfo(Action<object> callback, object parameter)
            {
                this.callback = callback;
                this.parameter = parameter;
            }
        }

        private ThreadedDataRequestor(GameObject obj) : base(obj)
        {}

        public static ThreadedDataRequestor CreateInstance() {

            if (instance != null)
                return instance;
            var go = new GameObject();
            return go.AddComponent( obj => new ThreadedDataRequestor( obj ) );
        }

        public override void OnDestroy()
        {
            // do something?
        }
    }
}