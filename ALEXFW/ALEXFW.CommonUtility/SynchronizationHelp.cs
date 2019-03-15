using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ALEXFW.CommonUtility
{
    /// <summary>
    /// 内存中对Guid值加锁，防止进程互斥
    /// </summary>
    public static class SynchronizationHelp
    {
        public static Mutex _Mutex = new Mutex();
        private static readonly Dictionary<Guid, SemaphoreSlim> _Slim = new Dictionary<Guid, SemaphoreSlim>();
        private static readonly Dictionary<Guid, int> _Count = new Dictionary<Guid, int>();

        /// <summary>
        /// 内存中锁定给定的Guid
        /// </summary>
        /// <param name="id">对应的Guid</param>
        /// <returns></returns>
        public static async Task ThreadEnter(Guid id)
        {
            _Mutex.WaitOne();
            SemaphoreSlim slim;
            if (!_Slim.TryGetValue(id, out slim))
            {
                slim = new SemaphoreSlim(1);
                _Slim.Add(id, slim);
                _Count.Add(id, 0);
            }
            _Count[id]++;
            _Mutex.ReleaseMutex();
            await slim.WaitAsync();
        }

        /// <summary>
        /// 解锁对应的Guid
        /// </summary>
        /// <param name="id"></param>
        public static void ThreadExit(Guid id)
        {
            var slim = _Slim[id];
            if (_Count[id] == 1)
            {
                _Count.Remove(id);
                _Slim.Remove(id);
            }
            else
                _Count[id]--;
            slim.Release();
        }
    }
}