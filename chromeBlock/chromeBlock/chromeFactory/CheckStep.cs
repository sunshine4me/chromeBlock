using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace chromeBlock.chromeFactory {
    class CheckStep : BaseStep {



        public override void Excite() {
            IWebElement element = runFactory.findElement(this);
            if (element == null) {
                executRecord.screenshot = runFactory.snapshot();
                executRecord.recordType = 404;
                executRecord.recordMessage = "找不到控件";
                return;
            }
            runFactory.snapshot();

            base.Excite();
        }
    }

}
