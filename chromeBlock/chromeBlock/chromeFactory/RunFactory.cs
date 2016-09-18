using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace chromeBlock.chromeFactory {
    /// <summary>
    /// 案例执行工厂
    /// </summary>
    public class RunFactory {
        public  ChromeDriver driver { get; set; }

        /// <summary>
        /// 查找控件
        /// </summary>
        public IWebElement findElement(BaseStep ts) {
            IWebElement we = null;
            String xpath = ts.xpath;
            if (string.IsNullOrEmpty(xpath)) {
                xpath = this.getNativeXpath(ts);
            }
            if (ts.index > 0)
                we = driver.FindElementsByXPath(xpath)[ts.index - 1];
            else
                we = driver.FindElementByXPath(xpath);

            //将界面移动到element上
            //第一中办法
            //((IJavaScriptExecutor)ch).ExecuteScript("arguments[0].scrollIntoView();", we);

            //第二中办法
            //int y = we.Location.Y;
            //String js = String.Format("window.scroll(0, {0})", y / 2);
            //((IJavaScriptExecutor)ch).ExecuteScript(js);


            //第三种 
            Actions action = new Actions(driver);
            action.MoveToElement(we).Perform();

            return we;
        }

        public string getNativeXpath( BaseStep step) {
            StringBuilder xp = new StringBuilder();
            if (string.IsNullOrEmpty(step.tagName)) {
                xp.Append("//*");
            } else {
                xp.Append($"//{step.tagName}");
            }

            if (!string.IsNullOrEmpty(step.className)) {
                xp.Append($"[@class='{step.className}']");
            }

            if (!string.IsNullOrEmpty(step.id)) {
                xp.Append($"[@id='{step.id}']");
            }

            if (!string.IsNullOrEmpty(step.name)) {
                xp.Append($"[@name='{step.name}']");
            }

            if (!string.IsNullOrEmpty(step.text)) {
                xp.Append($"[text()='{step.text}']");
            }

            return xp.ToString();

        }

        /// <summary>
        /// 截图
        /// </summary>
        public string snapshot() {
            string fileName = $"{Environment.CurrentDirectory}/result/{DateTime.Now.ToString("yyyyMMddhhmmss")}.jpg";
            return snapshot(fileName);
        }

        /// <summary>
        /// 截图
        /// </summary>
        public string snapshot(string fileName) {
            try {
                driver.GetScreenshot().SaveAsFile(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            } catch( Exception e) {
                Console.WriteLine("[snapshot] error : " + e.StackTrace);
            }
            return fileName;
        }

        /// <summary>
        /// 创建step对象
        /// </summary>
        public BaseStep greateStep(oneBlockStep _step) {
            BaseStep tmp;
            
            try {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Object[] parameters = new Object[1];
                parameters[0] = _step;
                tmp = (BaseStep)assembly.CreateInstance("chromeBlock.chromeFactory." + _step.name, true);
                if (tmp != null) {
                    tmp.__describe = _step.describe;
                    assignPro(tmp, _step.attrs);
                    tmp.runFactory = this;
                } 
            } catch (Exception e) {
                Console.WriteLine("[greateStep] warring:" + e.StackTrace);
                tmp = new BaseStep();
            }
            return tmp;
        }

        /// <summary>
        /// 赋值
        /// </summary>
        public void assignPro(BaseStep _basestep, Dictionary<string, string> attrs) {
            Type type = _basestep.GetType();
            var properties = type.GetProperties();
            foreach (var attr in attrs) {
                var pi = properties.FirstOrDefault(x => x.Name.ToLower() == attr.Key.ToLower());
                if (pi != null) {
                    //防止int bool 等参数在做空值转换时的错误
                    if (pi.PropertyType != typeof(string) && string.IsNullOrEmpty(attr.Value))
                        continue;
                    try {
                        pi.SetValue(_basestep, Convert.ChangeType(attr.Value, pi.PropertyType), null);
                    } catch (Exception e) {
                        Console.WriteLine("[assignPro] warring:" + e.StackTrace);
                    }
                   
                }
                    
            }
        }

        /// <summary>
        /// 杀掉相关进程并释放所有资源(为了艾泽拉斯)
        /// </summary>
        public void killThemAll() {
            //chromeDriver 关闭
            try {
                driver.Quit();
            } catch {
                //todo
            }
        }
    }
}
