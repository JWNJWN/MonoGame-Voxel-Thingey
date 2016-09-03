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
    public class ThreadManager : BaseManager
    {

        Thread mainThread;
        Thread chunkThread;

        Queue<Action<GameTime>> chunkQueue;

        GameTime gameTime;

        public ThreadManager(SceneGame game) : base(game)
        {
            mainThread = Thread.CurrentThread;

            chunkQueue = new Queue<Action<GameTime>>();

            chunkThread = new Thread(new ThreadStart(HandleChunks));
            chunkThread.Start();

            //Initialize();
        }

        protected override string GetName()
        {
            return "Thread";
        }

        public override void UnloadContent()
        {
            chunkThread.Abort();
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            base.Update(gameTime);
        }

        private void HandleChunks()
        {
            while (true)
            {
                if(chunkQueue != null && chunkQueue.Count > 0)
                {
                    chunkQueue.Dequeue()?.Invoke(gameTime);
                }
            }
        }
        
        public void QueueChunkUpdateEvent(Action<GameTime> chunkAction)
        {
            try
            {
                if(chunkAction != null)
                    chunkQueue?.Enqueue(chunkAction);
            }
            catch (Exception e)
            {

                System.Diagnostics.Debug.Print(e.StackTrace);
            }
        }

        public void QueueUpdate(Action<GameTime> updateAction)
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
