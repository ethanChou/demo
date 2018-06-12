using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using Com.Utils;
using Com.SystemLib;

namespace ConvertToAvi
{
    /// <summary>
    /// 文件解码结束信息回调
    /// </summary>
    /// <param name="port"></param>
    delegate void FramePoolEventHandler(IntPtr yv12Buffer);
    /// <summary>
    /// 码流池  用来让解码
    /// </summary>
    class FramePool
    {
        /// <summary>
        /// 消耗码流的线程，编码
        /// </summary>
        Thread _consumeThread = null;
        /// <summary>
        /// 用来锁住队列进出
        /// </summary>
        object _lockQueue = new object();
        /// <summary>
        /// 用来锁住码流进出，当池子中码流满时，用此锁让解码线程暂停
        /// </summary>
        object _lockAddFrame = new object();
        /// <summary>
        /// 最大容纳的码流数
        /// </summary>
        int _maxFrameNum = 20;
        /// <summary>
        /// buffer  码流
        /// </summary>
        Queue<IntPtr> _buffers = new Queue<IntPtr>();
        /// <summary>
        /// 标志码流池是否已满
        /// </summary>
        bool _isFull = false;
        /// <summary>
        /// 码流向上回调
        /// </summary>
        FramePoolEventHandler _framePoolEvent;
        /// <summary>
        /// 停止线程时使用
        /// </summary>
        AutoResetEvent _stopThreadEvent = new AutoResetEvent(false);
        public void Start(FramePoolEventHandler poolEvent)
        {
            _framePoolEvent = poolEvent;
            _consumeThread = new Thread(new ThreadStart(StartConsume));
            _consumeThread.Start();
        }
        public void Stop()
        {
            if (_consumeThread != null && _consumeThread.IsAlive)
            {
                _stopThreadEvent.Set();
                _consumeThread.Join();
            }
            _consumeThread = null;

            while (_buffers.Count > 0)
            {
                IntPtr ptr = _buffers.Dequeue();
                Marshal.FreeHGlobal(ptr);
            }
            _buffers.Clear();
        }

        void StartConsume()
        {
            while (true)
            {
                if (_stopThreadEvent.WaitOne(1, false))
                {
                    break;
                }
                for (int i = 0; i < _buffers.Count; i++)
                {
                    IntPtr ptr = IntPtr.Zero;
                    Monitor.Enter(_lockQueue);
                    ptr = _buffers.Dequeue();
                    Monitor.Exit(_lockQueue);
                    if (ptr == IntPtr.Zero)
                    {
                        continue;
                    }
                    //消化这一帧数据
                    if (_framePoolEvent != null)
                    {
                        _framePoolEvent(ptr);
                    }
                    if (_isFull && _buffers.Count < _maxFrameNum / 2)
                    {
                        lock (_lockAddFrame)
                        {
                            Monitor.Pulse(_lockAddFrame);
                        }
                    }
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }
        /// <summary>
        /// 从SDK中返回的码流，在此进行采样或者拷贝后加入处理队列
        /// </summary>
        /// <param name="buffer">从SDK中返回的码流</param>
        /// <param name="length">要采样或者拷贝后生成的码流长度，实际内部也可以计算出来，但是为了节省时间，就直接从外面传</param>
        /// <param name="realWidth">从SDK中返回的码流，视频的宽度</param>
        /// <param name="realHeight">从SDK中返回的码流，视频的高度</param>
        public void AddFrame(IntPtr buffer, int length, int realWidth, int realHeight)
        {
            if (_buffers.Count >= _maxFrameNum)
            {
                _isFull = true;
                lock (_lockAddFrame)
                {
                    Monitor.Wait(_lockAddFrame);
                }
            }
            _isFull = false;
            //将内存拷贝出去，SDK返回的BUFFER，sdk内部分负责清理，所以要拷贝内存
            //拷贝内存完成后，处理层负责清理申请的内存
            IntPtr yv12 = Marshal.AllocHGlobal(length);
            Win32API.CopyMemory(yv12, buffer, (uint)length);
            Monitor.Enter(_lockQueue);
            _buffers.Enqueue(yv12);
            Monitor.Exit(_lockQueue);
        }
    }
}
