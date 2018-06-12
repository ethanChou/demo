using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FileService
{
    /// <summary>
    /// 设计思想非常好，结构清晰
    /// </summary>
    class SimpleHttpServer
    {
        public class Processor
        {
            public TcpClient _socket;
            public Server _srv;

            private Stream _inputStream;
            public StreamWriter OutputStream;

            public String http_method;
            public String http_url;
            public String http_protocol_versionstring;
            public Hashtable httpHeaders = new Hashtable();


            private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

            public Processor(TcpClient s, Server srv)
            {
                this._socket = s;
                this._srv = srv;
            }

            private string StreamReadLine(Stream inputStream)
            {
                int next_char;
                string data = "";
                while (true)
                {
                    next_char = inputStream.ReadByte();
                    if (next_char == '\n') { break; }
                    if (next_char == '\r') { continue; }
                    if (next_char == -1) { Thread.Sleep(1); continue; };
                    data += Convert.ToChar(next_char);
                }
                return data;
            }
            public void Process()
            {
                // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
                // "processed" view of the world, and we want the data raw after the headers
                _inputStream = new BufferedStream(_socket.GetStream());

                // we probably shouldn't be using a streamwriter for all output from handlers either
                OutputStream = new StreamWriter(new BufferedStream(_socket.GetStream()));
                try
                {
                    ParseRequest();
                    ReadHeaders();
                    if (http_method.Equals("GET"))
                    {
                        HandleGetRequest();
                    }
                    else if (http_method.Equals("POST"))
                    {
                        HandlePostRequest();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.ToString());
                    WriteFailure();
                }
                OutputStream.Flush();
                // bs.Flush(); // flush any remaining output
                _inputStream = null; OutputStream = null; // bs = null;            
                _socket.Close();
            }

            public void ParseRequest()
            {
                String request = StreamReadLine(_inputStream);
                string[] tokens = request.Split(' ');
                if (tokens.Length != 3)
                {
                    throw new Exception("invalid http request line");
                }
                http_method = tokens[0].ToUpper();
                http_url = tokens[1];
                http_protocol_versionstring = tokens[2];

                Console.WriteLine("starting: " + request);
            }

            public void ReadHeaders()
            {
                Console.WriteLine("readHeaders()");
                String line;
                while ((line = StreamReadLine(_inputStream)) != null)
                {
                    if (line.Equals(""))
                    {
                        Console.WriteLine("got headers");
                        return;
                    }

                    int separator = line.IndexOf(':');
                    if (separator == -1)
                    {
                        throw new Exception("invalid http header line: " + line);
                    }
                    String name = line.Substring(0, separator);
                    int pos = separator + 1;
                    while ((pos < line.Length) && (line[pos] == ' '))
                    {
                        pos++; // strip any spaces
                    }

                    string value = line.Substring(pos, line.Length - pos);
                    Console.WriteLine("header: {0}:{1}", name, value);
                    httpHeaders[name] = value;
                }
            }

            public void HandleGetRequest()
            {
                _srv.HandleGetRequest(this);
            }

            private const int BUF_SIZE = 4096;
            public void HandlePostRequest()
            {
                // this post data processing just reads everything into a memory stream.
                // this is fine for smallish things, but for large stuff we should really
                // hand an input stream to the request processor. However, the input stream 
                // we hand him needs to let him see the "end of the stream" at this content 
                // length, because otherwise he won't know when he's seen it all! 

                Console.WriteLine("get post data start");
                int content_len = 0;
                MemoryStream ms = new MemoryStream();
                if (this.httpHeaders.ContainsKey("Content-Length"))
                {
                    content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                    if (content_len > MAX_POST_SIZE)
                    {
                        throw new Exception(
                            String.Format("POST Content-Length({0}) too big for this simple server",
                              content_len));
                    }
                    byte[] buf = new byte[BUF_SIZE];
                    int to_read = content_len;
                    while (to_read > 0)
                    {
                        Console.WriteLine("starting Read, to_read={0}", to_read);

                        int numread = this._inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                        Console.WriteLine("read finished, numread={0}", numread);
                        if (numread == 0)
                        {
                            if (to_read == 0)
                            {
                                break;
                            }
                            else
                            {
                                throw new Exception("client disconnected during post");
                            }
                        }
                        to_read -= numread;
                        ms.Write(buf, 0, numread);
                    }
                    ms.Seek(0, SeekOrigin.Begin);
                }
                Console.WriteLine("get post data end");
                _srv.HandlePostRequest(this, new StreamReader(ms));

            }

            public void WriteSuccess(string content_type = "text/html", long len = 0)
            {
                // this is the successful HTTP response line
                OutputStream.WriteLine("HTTP/1.0 200 OK");
                // these are the HTTP headers...          
                OutputStream.WriteLine("Content-Type: " + content_type);
                OutputStream.WriteLine("Content-Length", len);

                OutputStream.WriteLine("Connection: close");
                // ..add your own headers here if you like

                OutputStream.WriteLine(""); // this terminates the HTTP headers.. everything after this is HTTP body..
            }

            public void WriteFailure()
            {
                // this is an http 404 failure response
                OutputStream.WriteLine("HTTP/1.0 404 File not found");
                // these are the HTTP headers
                OutputStream.WriteLine("Connection: close");
                // ..add your own headers here

                OutputStream.WriteLine(""); // this terminates the HTTP headers.
            }
        }

        public abstract class Server
        {

            protected int port;
            TcpListener listener;
            bool is_active = true;

            public Server(int port)
            {
                this.port = port;
            }

            public void Listen()
            {
                listener = new TcpListener(System.Net.IPAddress.Any, port);
                listener.Start();
                while (is_active)
                {
                    TcpClient s = listener.AcceptTcpClient();
                    Processor processor = new Processor(s, this);
                    Thread thread = new Thread(new ThreadStart(processor.Process));
                    thread.Start();
                    Thread.Sleep(1);
                }
            }

            public abstract void HandleGetRequest(Processor p);
            public abstract void HandlePostRequest(Processor p, StreamReader inputData);
        }

        public class MyHttpServer : Server
        {
            public MyHttpServer(int port)
                : base(port)
            {
            }
            public override void HandleGetRequest(Processor p)
            {
                if (p.http_url.EndsWith(".jpg") || p.http_url.EndsWith(".png") || p.http_url.EndsWith(".bmp"))
                {
                    using (Stream fs = File.Open(Path.Combine("E:\\image", p.http_url.TrimStart('/')), FileMode.Open))
                    {

                        p.WriteSuccess("image/jpg",fs.Length);

                        fs.CopyTo(p.OutputStream.BaseStream);
                        p.OutputStream.BaseStream.Flush();
                    }
                }

                Console.WriteLine("request: {0}", p.http_url);
                p.WriteSuccess();
                p.OutputStream.WriteLine("<html><body><h1>test server</h1>");
                p.OutputStream.WriteLine("Current Time: " + DateTime.Now.ToString());
                p.OutputStream.WriteLine("url : {0}", p.http_url);

                p.OutputStream.WriteLine("<form method=post action=/form>");
                p.OutputStream.WriteLine("<input type=text name=foo value=foovalue>");
                p.OutputStream.WriteLine("<input type=submit name=bar value=barvalue>");
                p.OutputStream.WriteLine("</form>");
            }

            public override void HandlePostRequest(Processor p, StreamReader inputData)
            {
                Console.WriteLine("POST request: {0}", p.http_url);
                string data = inputData.ReadToEnd();

                p.WriteSuccess();
                p.OutputStream.WriteLine("<html><body><h1>test server</h1>");
                p.OutputStream.WriteLine("<a href=/test>return</a><p>");
                p.OutputStream.WriteLine("postbody: <pre>{0}</pre>", data);


            }
        }
    }
}
