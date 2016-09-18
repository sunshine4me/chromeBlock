using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace chromeBlock.chromeFactory {
    class EditStep : BaseStep {
        public string inputText { get; set; }

        public bool pressEnter { get; set; }



        public override void Excite() {
            IWebElement element = runFactory.findElement(this);
            if (element != null) {
                element.Clear();
                element.SendKeys(this.inputText);

                if (pressEnter) {
                    Actions builder = new Actions(runFactory.driver);
                    builder.SendKeys(Keys.Enter).Perform();
                }

            } else {
                executRecord.screenshot = runFactory.snapshot();
                executRecord.recordType = 404;
                executRecord.recordMessage = "找不到控件";
                return;
            }
            base.Excite();
        }
    }
}
