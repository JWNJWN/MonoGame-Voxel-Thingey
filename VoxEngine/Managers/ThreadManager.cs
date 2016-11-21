using System;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using VoxEngine.Voxels;

namespace VoxEngine.Managers
{
    public class ThreadManager : GameComponent
    {

        private static bool _initialized = false;
        public static bool Initialized
        {
            get { return _initialized; }
        }

        private static ConcurrentDictionary<string, Thread> _threads = new ConcurrentDictionary<string, Thread>();

        public ThreadManager(Game game) : base(game)
        {
            Thread _mainThread = Thread.CurrentThread;
            _mainThread.Priority = ThreadPriority.Highest;
            _mainThread.Name = "Main Thread";

            AddThread("MainThread", _mainThread);

            _initialized = true;
        }
        public static void CreateThread(string threadID, Action action)
        {
            Thread tempThread = new Thread(new ThreadStart(action));
            tempThread.IsBackground = true;
            tempThread.Name = threadID;
            tempThread.Start();
            _threads.TryAdd(threadID, tempThread);
        }

        public static void AddThread(string threadID, Thread thread)
        {
            _threads.TryAdd(threadID, thread);
        }

        public static void RemoveThread(string threadID)
        {
            Thread tempThread;
            _threads.TryRemove(threadID, out tempThread);
        }

        public static Thread GetThread(string threadID)
        {
            Thread tempThread;
            _threads.TryGetValue(threadID, out tempThread);
            return tempThread;
        }
    }
}
