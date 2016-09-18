using chromeBlock.chromeFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace chromeBlock {
    class Program {


        static void Main(string[] args) {


            Console.ForegroundColor = ConsoleColor.White;

            blockServer _runFactory = new blockServer();

            _runFactory.runEvent += startRun;
            _runFactory.StartListener(8500);
        }

        //其实这个方法也可以放到 RunFactor里,为了直观所以放在这里
        static void startRun(oneBlockCase _case) {
            Console.ForegroundColor = ConsoleColor.White;
            RunFactory runFactory = new RunFactory();
            List<BaseStep> runSteps = new List<BaseStep>();

            foreach (var st in _case.steps) {
                var tmp = runFactory.greateStep(st);
                if (tmp != null)
                    runSteps.Add(tmp);
            }

            foreach (var step in runSteps) {
                try {
                    Console.Write($"{step}:{step.__describe} 开始执行...");
                    step.Excite();
                    Console.WriteLine("完成!");
                } catch (Exception e) {
                    step.executRecord.screenshot = runFactory.snapshot();
                    step.executRecord.recordType = 500;
                    step.executRecord.recordMessage = e.StackTrace;
                    break;//结束执行
                }
            }

            Console.WriteLine();

            Console.WriteLine("执行结果:");
            runFactory.killThemAll();
            result(runSteps);

            
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void result(List<BaseStep> runSteps) {

            string FilePath = $"{Environment.CurrentDirectory}/result/{DateTime.Now.ToString("yyyyMMddhhmmss")}.txt";
            if (!File.Exists(FilePath)) {
                FileStream myFs = new FileStream(FilePath, FileMode.Create);
                StreamWriter mySw = new StreamWriter(myFs);
                int i = 1;
                foreach (var step in runSteps) {
                    if (step.executRecord.recordType != 200)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else
                        Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"{i}.{step.__describe} 执行结果: {step.executRecord.recordMessage}");

                    mySw.WriteLine($"{i}.{step.__describe} 执行结果: {step.executRecord.recordMessage}");
                    i++;
                }
                mySw.Close();
                myFs.Close();
            }
        }
    }
}
