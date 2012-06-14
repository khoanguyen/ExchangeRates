ExchangeRates
=============

Exchange Rates

# Requirement

* Visual Studio 2010 (not Express Edition)
* SQL Server 2008 R2
* Entity Framework 4.3.1

# Description

Exchange Rates is a webpage that show the exchange rate of currencies in th indicated range of time. The base currency is USD.

Download the zip package of the source code from Zip folder

# SetUp

* Install SQL Server 2008 R2. You can download it free from Microsoft page.
* Create database ExchangeRate database, you can name it yourself. E.g : ExchangeRateDB
* Update the connection string with the name "ExchangeRateContext" in application's Web.Config, remember to use the correct database name. 
    E.g : Data Source=.\SQL2008R2;Initial Catalog=ExchangeRateDB;Integrated Security=True
* Open the project with Visual Studio 2010
* Tools -> Library Package Manager -> Package Manager Console
* On the console type Update-Database for creating the ExchangeRates table
* Press F6 to build the project
* Press F5 to run the web Application

# Project Settings

In the Appsettings section of Web.config, there are 2 custom settings:

* ExchangeRateServiceURL : base address of Excahnge Rate service API, you better not to modify this.
* TrackedCurrencies : Comma delimited list of currency that can be choose to see the rate on the web page. You can add/remove currencies by set the value for this setting, e.g : "USD,VND,EUR,GBP"

# Additional Libraries

* Twitter Bootstrap
* jqPlot