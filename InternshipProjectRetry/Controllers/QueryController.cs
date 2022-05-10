using DataLibrary.BussinessLogic;
using DataLibrary.DataAccess;
using InternshipProjectRetry.com.salesforce.enterprise;
using InternshipProjectRetry.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;

namespace InternshipProjectRetry.Controllers
{
    public class QueryController : Controller
    {
        // GET: Query
        public void LoadQueryPage()
        {
            ClearDB();
            QueryFields();
            QueryAccounts();
            DataMappers.DataMapper.dataMapper();
        }

        SforceService binding = LoginController.GetSforceService();


        List<DataLibrary.Models.QueryModel> queryModels;


        // This is to clear the DB
        public void ClearDB()
        {

            SqlConnection con = new SqlConnection(SQLDataAccess.GetConnectionString());
            con.Open();

            string sql = @"DELETE FROM SalesForceAccounts; DELETE FROM Fields; DELETE FROM CMTable;";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            con.Close();

        }


        // These are hardcoded through a model into the DB because .NET doesn't like random data flying around

        // This is to query the general fields required by CM
        public void QueryAccounts()
        {

            QueryResult queryResults = binding.query("SELECT AccountNumber, Name, Website, BillingStreet, " +
                "BillingCity, BillingState, BillingCountry, BillingPostalCode, Owner.Name, Owner.Email FROM Account");
            if (queryResults != null)
            {
                sObject[] records = queryResults.records;

                for (int i = 0; i < records.Length; i++)
                {
                    var account = (Account)records[i];
                    SalesForceProcessor.CreateRecord(account.AccountNumber, account.Name, account.Website, account.BillingStreet,
                        account.BillingCity, account.BillingState, account.BillingCountry,
                        account.BillingPostalCode, account.Owner.Name, account.Owner.Email);
                }

            }
            else
            { System.Diagnostics.Debug.WriteLine("No Account found"); }
        }


        // This is to get available fields for an object

        public void QueryFields()
        {

            try
            {
                // Make the describe call
                DescribeSObjectResult describeSObjectResult = binding.describeSObject("Account");

                // Get sObject metadata 
                if (describeSObjectResult != null)
                {

                    // Get the fields
                    Field[] fields = describeSObjectResult.fields;

                    for (int i = 0; i < fields.Length; i++)
                    {
                        Field field = fields[i];
                        SalesForceProcessor.CreateFieldRecord(field.name, field.type);
                    }

                }

            }
            catch (SoapException e)
            {
                System.Diagnostics.Debug.WriteLine("An unexpected error has occurred: " +
                    e.Message + "\n" + e.StackTrace);
            }

        }


        // This is to load the available account names & Fields -- Requires QueryAccounts & QueryFields to be executed

        public ActionResult Query()
        {

            if (LoginController.getLoginStatus())
            {
                LoadQueryPage();

                var data = SalesForceProcessor.LoadAccounts();


                var fielddata = SalesForceProcessor.LoadFields();


                SelectList Accountlist = new SelectList(data, "AccountName", "AccountName");

                ViewBag.accountNameList = Accountlist;



                if (data.Count > 0)
                {
                    return View(fielddata);

                }
                else
                {
                    // I should pass a message here too

                    return RedirectToAction("Login", "Login");

                }


            }
            else
            {
                return RedirectToAction("Login", "Login");
            }



        }



        //For scheduling most recent query from SOQL Box

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ScheduleQuery(DateTime date)
        {
            // Loads most recent query
            List<DataLibrary.Models.SavedQueryModel> querylist = SalesForceProcessor.LoadQueries();
            int count = querylist.Count;
            var LastQuery = querylist[count - 1].SavedQuery;    

            SalesForceProcessor.CreateScheduledQuery(LastQuery, date);

            return RedirectToAction("Query");

        }

        //Defining a new query from scheduler


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ScheduleNewQuery(string query, DateTime date)
        {
            // Loads most recent query

            var NewQuery = "Select " + query + " from Account";

            SalesForceProcessor.CreateScheduledQuery(NewQuery, date);

            return RedirectToAction("Query");

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public void AddToDatabase(FormCollection form)
        {

            // Obtains the previous Query, executes it and stores it in the database

            List<DataLibrary.Models.SavedQueryModel> querylist = SalesForceProcessor.LoadQueries();

            int count = querylist.Count;

            var query = querylist[count - 1].Parameters;
            
            var modelList = GenericQuery(query);

            SalesForceProcessor.CreateSOQLRecord(modelList);


        }




        public static string ToUpperFirstLetter(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            // convert to lower case
            source.ToLower();
            // convert to char array of the string
            char[] letters = source.ToCharArray();
            // upper case the first char
            letters[0] = char.ToUpper(letters[0]);
            // return the array made of the new char array
            return new string(letters);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult SOQLResult(FormCollection form)
        {
            // this is what comes from the user ; Id,Name 

            string SOQL = form["Query"].ToString();

            SOQL.Replace(",", "");


            if (string.IsNullOrEmpty(SOQL))
            {
                return RedirectToAction("Query");
            }



            // split up by commas and remove whitespace

            string[] words = SOQL.Split(',', ' ');

            StringBuilder sb = new StringBuilder();

            foreach (string word in words)
            {
                // make each word begin uppercase 

                // add a comma + space on all except the last one and 

                if (word == words.Last())
                {
                    sb.Append(ToUpperFirstLetter(word));
                }
                else
                {
                    sb.Append(ToUpperFirstLetter(word) + ",");
                }
            }

            string Query = "Select " + sb.ToString() + " From Account";

            SalesForceProcessor.CreateSavedQuery(Query, SOQL);

            var model = GenericQuery(sb.ToString());

            if(model.Count == 0)
            {
                return RedirectToAction("Query");
            }


            return View(model);

        }



        //Generic Query - Just taking a string, or a set of tokens delimited by commas

        public List<DataLibrary.Models.QueryModel> GenericQuery(string userquery)
        {

            string SOQL = userquery;
            string Query = "Select " + SOQL + " From Account";
            string[] words = SOQL.Split(',');

            System.Diagnostics.Debug.WriteLine(SOQL);

            try
            {
                QueryResult qResult = binding.query(Query);

                queryModels = new List<DataLibrary.Models.QueryModel>();

                Boolean done = false;
                if (qResult.size > 0)
                {
                    while (!done)
                    {
                        sObject[] records = qResult.records;
                        // Two for loops. One to iterate through the selected fields, another to iterate over the records for said field. 

                        for (int j = 0; j < words.Length; ++j)
                        {

                            DataLibrary.Models.QueryModel model = new DataLibrary.Models.QueryModel();
                            var fieldlist = new List<string>();
                            model.FieldName = words[j];


                            for (int i = 0; i < records.Length; ++i)
                            {
                                Account acc = (Account)records[i];
                                var qr = records[i].GetType().GetProperty(words[j]).GetValue(acc, null);

                                if (qr != null)
                                {
                                    fieldlist.Add(qr.ToString());
                                    System.Diagnostics.Debug.WriteLine(qr.ToString());
                                }

                            }
                            model.FieldValueList = fieldlist;
                            queryModels.Add(model);
                        }

                        if (qResult.done)
                        {
                            done = true;
                        }
                        else
                        {
                            qResult = binding.queryMore(qResult.queryLocator);
                        }
                    }
                }
                else { Console.WriteLine("No records found."); }

                return queryModels;

            }
            catch (Exception ex)
            {
                queryModels = new List<DataLibrary.Models.QueryModel>();

                return queryModels;
            }


        }
    }
}

