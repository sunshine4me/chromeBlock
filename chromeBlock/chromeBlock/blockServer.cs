using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace chromeBlock {
    /// <summary>
    /// 用来接收oneBlock 案例的类
    /// </summary>
    public class blockServer {
        //定义delegate
        public delegate void runDelegate(oneBlockCase tc);

        /// <summary>
        /// 执行事件
        /// </summary>
        public event runDelegate runEvent;

        private int RunCount;

        /// <summary>
        /// 启动端口监听服务
        /// </summary>
        /// <param name="port">端口</param>
        public void StartListener(int port) {
            using (HttpListener listerner = new HttpListener()) {
                listerner.AuthenticationSchemes = AuthenticationSchemes.Anonymous;//指定身份验证 Anonymous匿名访问
                listerner.Prefixes.Add($"http://+:{port}/testRun/");

                listerner.Start();
                Console.WriteLine($"端口:{port}, blockServer已启动.......");
                //listerner.BeginGetContext();//异步调用(允许并发处理)
                while (true) {
                    //等待请求连接
                    HttpListenerContext ctx = listerner.GetContext();
                    ctx.Response.StatusCode = 200;//设置返回给客服端http状态代码
                    ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");


                    StreamReader reader = new StreamReader(ctx.Request.InputStream);

                    try {
                        var str = reader.ReadToEnd();
                        var testcase = JsonConvert.DeserializeObject<oneBlockCase>(str);

                        Console.WriteLine("收到案例,开始执行...");

                        if (RunCount <= 0) {
                            if (runEvent != null) { // 如果有对象注册  
                                RunCount = runEvent.GetInvocationList().Count();
                                foreach (runDelegate de in runEvent.GetInvocationList()) {
                                    de.BeginInvoke(testcase, new AsyncCallback(runCallBack), "执行完成!");
                                }
                            }

                        } else {
                            //还在执行
                            ctx.Response.StatusCode = 503;//设置返回给客服端http状态代码
                            System.IO.Stream output = ctx.Response.OutputStream;
                            System.IO.StreamWriter writer = new System.IO.StreamWriter(output);
                            writer.Write("正在执行其他案例,请稍后再试!");
                            // 必须关闭输出流
                            writer.Close();
                        }

                        

                    } catch (Exception e) {
                        ctx.Response.StatusCode = 500;//设置返回给客服端http状态代码

                        System.IO.Stream output = ctx.Response.OutputStream;
                        System.IO.StreamWriter writer = new System.IO.StreamWriter(output);
                        writer.Write( e.Message);
                        // 必须关闭输出流
                        writer.Close();
                        
                    }

                    ctx.Response.Close();

                }
            }
        }

        /// <summary>
        /// 回调函数(Todo something)
        /// </summary>
        private void runCallBack(IAsyncResult result) {
            RunCount--;
            Console.WriteLine(result.AsyncState);
        }
    }

    public class oneBlockStep {
        public string name { get; set; }

        public string describe { get; set; }
        public Dictionary<string, string> attrs { get; set; }
    }

    public class oneBlockCase {
        public List<oneBlockStep> steps { get; set; }
    }
}
