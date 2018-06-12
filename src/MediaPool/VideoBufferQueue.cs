using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ClientCore
{
    public class VideoBufferQueue
    {
        /// <summary>
        /// buffer缓冲的长度，大致值，有可能稍大于此值
        /// </summary>
        const int Buffer_Max_Length = 100;
        /// <summary>
        /// 小于此值时，马上通知上层开始传送数据到缓冲区
        /// </summary>
        const int Buffer_Min_Length = 20;

        bool _streamOver = false;
        /// <summary>
        /// 流媒体是否已经结束
        /// </summary>
        public bool StreamOver
        {
            get { return _streamOver; }
            set { _streamOver = value; }
        }
        object _obj = new object();

        
        Queue<byte[]> _qBuffers = new Queue<byte[]>();

        public void AddBuffer(byte[] bytes)
        {
            lock (_obj)
            {
                _qBuffers.Enqueue(bytes);
            }
        }

        public byte[] GetBuffer()
        {
            lock (_obj)
            {
                if (!IsAvailable())
                {
                    return null;
                }
                byte[] buffer = _qBuffers.Dequeue();
                return buffer;
            }
        }

        /// <summary>
        /// 是否需要暂停
        /// </summary>
        /// <returns>true，马上暂停，缓冲区已满。false，不用暂停，缓冲区有空余</returns>
        public bool IsNeedPause()
        {
            if (_qBuffers.Count > Buffer_Max_Length)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否需要继续接收码流
        /// </summary>
        /// <returns></returns>
        public bool IsNeedContinue()
        {
            if (_qBuffers.Count < Buffer_Min_Length && !_streamOver)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 清空缓冲区
        /// </summary>
        public void ClearBuffer()
        {
            lock (_obj)
            {
                _qBuffers.Clear();
            }
        }

        private bool IsAvailable()
        {
            return _qBuffers.Count > 0;
        }
    }
}
