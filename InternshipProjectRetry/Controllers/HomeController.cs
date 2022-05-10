using DataLibrary.BussinessLogic;
using DataLibrary.DataAccess;
using InternshipProjectRetry.com.salesforce.enterprise;
using InternshipProjectRetry.DataMappers;
using InternshipProjectRetry.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Services.Protocols;

namespace InternshipProjectRetry.Controllers
{


	public class HomeController : Controller
	{


		SforceService binding = LoginController.GetSforceService();
		LoginResult LoginResult = LoginController.GetLoginResult();

		public ActionResult Index()
		{
			return View();
		}






        //public void Run()
        //{
        //	// Make a login call 
        //	if (ConnectSFDC())
        //	{

        //		ClearDB();

        //		QueryAccounts();

        //		QueryFields();

        //		DataMapper.dataMapper();




        //	}
        //}


















	



	}

}
