using System.Web.Mvc;
using DataLibrary.BussinessLogic;
using InternshipProjectRetry.com.salesforce.enterprise;
using InternshipProjectRetry.Controllers;
using InternshipProjectRetry.Models;
using NUnit.Framework;

namespace InternshipProject.Tests
{
    
    [TestFixture]
    public class SalesForceTester
    {




        public SforceService login()
        {

            var controller = new LoginController();

            var loginModel = new LoginModel();

            loginModel.Username = "j.murphy87@nuigalway.ie";
            loginModel.Password = "mineology12";
            loginModel.Token = "xZdna0s3xeqoNFgRSBlF1fQ2y";

            controller.Login(loginModel);

            return LoginController.GetSforceService();
        }

        [Test]
        public void LoginTester()
        {

           login();

            bool loginStatus = LoginController.getLoginStatus();
            
            Assert.IsTrue(loginStatus);

        }



        [Test]
        public void QueryTester()
        {

            login();

            var controller = new QueryController();

            var soql = "Id,Name";
            var queryModel = new DataLibrary.Models.QueryModel();

            var results = controller.GenericQuery(soql);

            Assert.IsNotNull(results);

        }

        //[Test]
        //public void QueryFormatTester()
        //{

        //    login();

        //    var controller = new QueryController();

        //    var soql = "Id";

        //    var results = controller.SOQLResult(soql);

        //    Assert.IsNotNull(results);

        //}


        [Test]
        public void SForceServiceRetrievalTester()
        {

            var sForceServiceObject = LoginController.GetSforceService();
            var SFDC = new SforceService();

            Assert.IsNotNull(sForceServiceObject);

        }



    }
}
