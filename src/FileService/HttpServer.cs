﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FileService
{
    public class HttpServer
    {
        #region Fields

        private int Port;
        private TcpListener Listener;
        private HttpProcessor Processor;
        private bool IsActive = true;

        #endregion

        #region Public Methods
        public HttpServer(int port, List<Route> routes)
        {
            this.Port = port;
            this.Processor = new HttpProcessor();

            foreach (var route in routes)
            {
                this.Processor.AddRoute(route);
            }
        }

        public void Listen()
        {
            this.Listener = new TcpListener(IPAddress.Any, this.Port);
            this.Listener.Start();
            while (this.IsActive)
            {
                TcpClient s = this.Listener.AcceptTcpClient();

                //Thread thread = new Thread(() =>
                //{
                //    this.Processor.HandleClient(s);
                //});
                //thread.Start();

                ThreadPool.QueueUserWorkItem(this.Processor.HandleClient,s);

                Thread.Sleep(1);
            }
        }

        #endregion

    }
}



