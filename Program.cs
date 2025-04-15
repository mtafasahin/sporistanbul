using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class BookRow
{
    public string Id { get; set; }
    public string Authors { get; set; }
    public string Title { get; set; }
    public string TitleLink { get; set; }  // Title'ın Linki
    public string Publisher { get; set; }
    public string Year { get; set; }
    public string Pages { get; set; }
    public string Language { get; set; }
    public string Size { get; set; }
    public string Extension { get; set; }
    public string Mirrors { get; set; }
    public string MirrorLink1 { get; set; }
    public string MirrorLink2 { get; set; }
    public string Html { get; set; }  // outerHTML of the <tr>
}

class Program
{
    static bool SeansBulVeSec(IWebDriver driver, string targetDate, 
                        TimeSpan startTime, TimeSpan endTime, 
                        List<string> allowedGenders)
    {

        var seansDivleri = driver.FindElements(By.CssSelector("#dvScheduler .panel-info"));
        var validBaslanicSaati = false;
        var validCinsyet = false;
        var validKontenjan = false;
        var validDate = false;

        foreach (var panel in seansDivleri)
        {
            try
            {
                // 1. Tarihi al
                var dateElement = panel.FindElement(By.CssSelector(".panel-heading .panel-title"));
                string panelDateText = dateElement.Text.Split("<br>").Last().Trim(); // "07.04.2025" olmalı

                validDate = panelDateText.Contains(targetDate);
                // if (!panelDateText.Contains(targetDate))
                //     continue;

                // 2. Seans kutularını tara
                var seansKutulari = panel.FindElements(By.CssSelector(".panel-body .well"));

                foreach (var seans in seansKutulari)
                {
                    try
                    {
                        // Saat
                        string saatAraligi = seans.FindElement(By.CssSelector("span[id*='lblSeansSaat']")).Text.Trim();
                        var saatler = saatAraligi.Split('-');
                        TimeSpan baslangic = TimeSpan.Parse(saatler[0].Trim());
                        TimeSpan bitis = TimeSpan.Parse(saatler[1].Trim());

                        validBaslanicSaati = baslangic >= startTime && baslangic <= endTime;
                        // if (baslangic < startTime || bitis > endTime)
                        //     continue;

                        // Cinsiyet
                        var cinsiyet = seans.FindElement(By.CssSelector("label.cinsiyet")).Text.Trim();
                        validCinsyet = allowedGenders.Contains(cinsiyet);
                        // if (!allowedGenders.Contains(cinsiyet))
                        //     continue;

                        // Kalan kontenjan
                        var kontenjanSpan = seans.FindElement(By.CssSelector("span[title='Kalan Kontenjan']"));
                        int kontenjan = int.Parse(kontenjanSpan.Text.Trim());
                        validKontenjan = kontenjan > 0;
                        // if (kontenjan <= 0)
                        //     continue;

                        if(!validBaslanicSaati || !validCinsyet || !validKontenjan || !validDate) {
                            Console.WriteLine($"{DateTime.Now} - Valid Bir Seans Değil : {panelDateText} {saatAraligi} - {cinsiyet} - Kontenjan: {kontenjan}");
                            continue;
                        }
                            
                        // Checkbox
                        var checkbox = seans.FindElements(By.CssSelector("input[type='checkbox']")).FirstOrDefault();
                        if (checkbox != null && checkbox.Enabled && !checkbox.Selected)
                        {
                            checkbox.Click();
                            Console.WriteLine($"Seans seçildi: {panelDateText} {saatAraligi} - {cinsiyet} - Kontenjan: {kontenjan}");
                            return true; // ilk bulduğunda çıkmak istersen
                        }
                    }
                    catch (Exception innerEx)
                    {
                        Console.WriteLine("Seans içi hata: " + innerEx.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Panel hata: " + ex.Message);
            }
        }

        Console.WriteLine("Uygun seans bulunamadı.");
        return false; // Seans bulunamadı
    }

    static void Main()
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
        

        // TC/Pasaport numarasını gir
        var tcInput = driver.FindElement(By.Name("txtTCPasaport"));
        tcInput.Clear();
        tcInput.SendKeys("34174407808"); // Buraya gerçek TC/Pasaport verisini yaz

        // Şifreyi gir
        var sifreInput = driver.FindElement(By.Name("txtSifre"));
        sifreInput.Clear();
        sifreInput.SendKeys("Mu5ty1618s*"); // Şifreni buraya yaz
        Thread.Sleep(1000);
        // Giriş butonuna tıkla
        var girisButonu = driver.FindElement(By.Name("btnGirisYap"));
        girisButonu.Click();
        Thread.Sleep(3000);
        // "https://online.spor.istanbul/uyespor.aspx" // Giriş yaptıktan sonra yönlendirilmesi gereken URL
        driver.Navigate().GoToUrl("https://online.spor.istanbul/uyespor.aspx");
        Thread.Sleep(3000);

        // Giriş sonrası Seans Seç butonu yüklenene kadar bekle
        wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("pageContent_rptListe_lbtnSeansSecim_0")));
        // Butona tıkla
        driver.FindElement(By.Id("pageContent_rptListe_lbtnSeansSecim_0")).Click();
        Thread.Sleep(3000);

        bool seansBulundu = false;
        while (!seansBulundu)
        {
            // Seans bulma işlemi
            seansBulundu = SeansBulVeSec(
                driver,
                "08.04.2025",
                TimeSpan.Parse("05:00"),
                TimeSpan.Parse("09:00"),
                new List<string> { "Erkek", "Karma" }
            );

            if (!seansBulundu)
            {
                // Eğer seans bulunamazsa, 5 saniye bekle ve tekrar dene
                Thread.Sleep(5000);
                driver.Navigate().GoToUrl("https://online.spor.istanbul/uyeseanssecim");
                Thread.Sleep(3000);
            }
        }
        

        driver.Quit();
    }
}