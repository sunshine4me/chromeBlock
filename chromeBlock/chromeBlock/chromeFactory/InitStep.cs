using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace chromeBlock.chromeFactory {
    class InitStep : BaseStep {
        public string url { get; set; }

        public int implicitlyWait { get; set; }


        public override void Excite() {


            string chromeDir = System.Environment.CurrentDirectory + "\\Resources\\";
            string chrome = chromeDir + "chromedriver.exe";
            ChromeDriverService cds = ChromeDriverService.CreateDefaultService(chromeDir);


            ChromeOptions co = new ChromeOptions();
            co.AddArgument("test-type");
            co.AddArgument("start-maximized");

            ChromeDriver driver;
            if (File.Exists(chrome)) {
                driver = new ChromeDriver(cds, co);
            } else {
                driver = new ChromeDriver(co);
            }

            runFactory.driver = driver;
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(implicitlyWait));//等待超时
            driver.Navigate().GoToUrl(url);
            

            runFactory.snapshot();

            base.Excite();
        }



    }
}

