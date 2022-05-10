using DataLibrary.BussinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InternshipProjectRetry.Controllers
{
    public class ReadController : Controller
    {
      
		public ActionResult ViewPartnerRecords()
		{

			var CMRecords = SalesForceProcessor.LoadCMRecords();

			// Check if there's accounts to map

			if (CMRecords != null)
			{
				return View(CMRecords);
			}
			return RedirectToAction("Query", "Query", "Query");
		}




		public ActionResult ViewScheduledQueries()
		{

			var Queries = SalesForceProcessor.LoadScheduledQueries();

			// Check if there's accounts to map

			if (Queries != null)
			{
				return View(Queries);
			}
			return RedirectToAction("Query", "Query", "Query");
		}




		// I need a method to show previous queries - or perhaps I use them as the scheduler... I have one made in proccessor anyhow



		public ActionResult ViewSoqlResults()
		{

			var SOQLResults = SalesForceProcessor.LoadSOQLResults();

			if (SOQLResults != null)
			{
				return View(SOQLResults);
			}
			return RedirectToAction("Query", "Query", "Query");


		}



		}
}