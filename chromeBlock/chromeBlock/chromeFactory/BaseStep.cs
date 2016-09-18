using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace chromeBlock.chromeFactory {
    public class BaseStep {
        
        //为了避免重名 使用两个下划线区分
        public string __describe { get; set; }

        #region 属性
        
        public string id { get; set; }
        public string name { get; set; }
        public string text { get; set; }
        public string className { get; set; }
        public string tagName { get; set; }
        public int index { get; set; }
        public int sleepTime { get; set; }
        public string xpath { get; set; }

        #endregion 属性

        public RunFactory runFactory;


        public ExecutRecord executRecord = new ExecutRecord();

        /// <summary>
        /// step启动执行
        /// </summary>
        public virtual void Excite() {
        
            Thread.Sleep(this.sleepTime * 1000);
            executRecord.screenshot = runFactory.snapshot();
            executRecord.recordType = 200;
            executRecord.recordMessage = "执行成功";

        }

    

    }
}
