using OpenQA.Selenium;

class MHRS {
    static void MHRS_2()
    {
        IWebDriver driver = new ChromeDriver();
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        driver.Navigate().GoToUrl($"https://mhrs.gov.tr/vatandas/#/");
        Thread.Sleep(2000);
        // 19514194532 // TC Kimlik Numarası
        // Bal10cuk* // Şifre
        // Sayfanın tamamen yüklenmesini bekle (gerekirse)
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        // <a href="uyegiris.aspx"> etiketini href'e göre bul
        // Butonu XPath ile bulup tıklayın
        
        bool continueSearch = true;

        while(continueSearch) {
            try
            {
                IWebElement button = wait.Until(driver =>
                driver.FindElement(By.XPath("//button[@id='randevu-ara-buton']"))
            );

                // Butonu tıklayın
                button.Click();

                // WebDriverWait tanımla
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                Thread.Sleep(5000);
                // Belirtilen öğeyi bulmaya çalışıyoruz
                IWebElement tarihNutonu = wait.Until(driver =>
                    driver.FindElement(By.XPath("//div[span[contains(@class, 'ant-tag ant-tag-has-color')]]"))
                );

                // Eğer öğe bulunduysa, üzerine tıklama işlemi yapılabilir
                tarihNutonu.Click();

                Thread.Sleep(3000);
                IWebElement collapseItem = wait.Until(driver =>
                    driver.FindElement(By.XPath("//div[contains(@class, 'ant-collapse-item')]"))
                );
                // Öğeyi bulduktan sonra üzerine tıklayın
                collapseItem.Click();
                Thread.Sleep(3000);

IWebElement saatBtn = wait.Until(driver =>
                driver.FindElement(By.XPath("//button[@class='ant-btn slot-saat-button slot-saat-is-kurali ant-btn-primary']"))
            );

            saatBtn.Click();
                Thread.Sleep(5000);

                IWebElement tmmButton = wait.Until(driver =>
                driver.FindElement(By.XPath("//div[@class='ant-modal-body']//button[@class='ant-btn ant-btn-primary']"))
            );

            // Eğer buton bulunduysa, üzerine tıklama işlemi yapılabilir
            tmmButton.Click();

            Thread.Sleep(3000);

             IWebElement onayButton = wait.Until(driver =>
                driver.FindElement(By.XPath("//div[@class='ant-modal-footer']//button[@class='ant-btn ant-btn-primary']"))
            );

            // Eğer buton bulunduysa, üzerine tıklama işlemi yapılabilir
            onayButton.Click();


                continueSearch = false; // Öğeyi bulduğumuzda döngüyü sonlandırıyoruz
                Console.WriteLine("Öğe başarıyla bulundu ve tıklandı.");
            }
            catch (TimeoutException)
            {
                // Eğer öğe 10 saniye içinde bulunamazsa yapılacak işlemi burada belirleyin
                Console.WriteLine("Öğe 10 saniye içinde bulunamadı. Başka bir işlem yapılacak.");
                
                // Örneğin, başka bir işlemi buraya ekleyebilirsiniz
                // Diğer işlerinizi burada yapabilirsiniz
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Beklenmeyen bir hata oluştu: {ex.Message}");
                    Thread.Sleep(2000);
                    IWebElement bilgiTamamButton = wait.Until(driver =>
                    driver.FindElement(By.XPath("//div[@class='ant-modal-confirm-btns']//button[@class='ant-btn ant-btn-primary']"))
                );


                // Eğer buton bulunduysa, üzerine tıklama işlemi yapılabilir
                bilgiTamamButton.Click();

                Thread.Sleep(2000);
                    IWebElement backTamamButton = wait.Until(driver =>
                    driver.FindElement(By.XPath("//div[@class='ant-page-header-back-button']"))
                );
                backTamamButton.Click();

                Thread.Sleep(60000);

            }
        }
    }
}
