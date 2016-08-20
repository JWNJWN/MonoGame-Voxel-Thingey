using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using Voxel.Engine.World;

namespace Voxel.Engine.Managers
{
    public struct ActionObject
    {
        public WaitCallback action;
        public object paramaters;
    }

    public class ThreadManager : BaseManager
    {

        Thread mainThread;
        Thread meshThread;

        Queue<Action> meshQueue;

        public ThreadManager(SceneGame game) : base(game)
        {
            mainThread = Thread.CurrentThread;

            meshQueue = new Queue<Action>();

            meshThread = new Thread(new ThreadStart(HandleMeshing));

            meshThread.Start();

            Initialize();
        }

        protected override string GetName()
        {
            return "Thread";
        }

        public override void UnloadContent()
        {
            meshThread.Abort();
            base.UnloadContent();
        }

        private void HandleMeshing()
        {
            while (true)
            {
                if(meshQueue.Count > 0)
                {
                    meshQueue.Dequeue().Invoke();
                }
            }
        }

        public void QueueMesh(Action meshAction)
        {
            meshQueue.Enqueue(meshAction);
        }

        public void QueueUpdate(Action<GameTime> updateAction, GameTime gameTime)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate { updateAction?.Invoke(gameTime); }));
            }
            catch
            { }
        }

        public void QueueChunk(Action<Chunk> chunkAction, Chunk chunk)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate { chunkAction?.Invoke(chunk); }));
            }
            catch
            { }
        }

        public void QueueEvent(Action action)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate { action?.Invoke(); }));
            }
            catch
            { }
        }
    }
}
