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
        public ActionResult LoadQueryPage()
        {
            ClearDB();
            QueryFields();
            QueryAccounts();
            GetSOQLIds();
            DataMappers.DataMapper.dataMapper();

            return RedirectToAction("Query");

        }

        SforceService binding = LoginController.GetSforceService();


        List<DataLibrary.Models.QueryModel> queryModels;


            // This is to clear the DB
            public void ClearDB()
        {

            SqlConnection con = new SqlConnection(SQLDataAccess.GetConnectionString());
            con.Open();

            
            // This is to get the unknown field tables inside SOQLTable

            StringBuilder sb = new StringBuilder();

            sb.Append("ALTER TABLE dbo.SOQLTable DROP COLUMN ");


            var getColumns = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME like 'SOQLTable'";

            List<string> columns = SQLDataAccess.LoadData<string>(getColumns);

            foreach(string col in columns)
            {
                if(col == columns[0])
                {
                    continue;
                }

                if (col == columns.Last())
                {
                    sb.Append(col);
                } else

                sb.Append(col + ", ");
            }


            //Clear all the other tables

            string sql = @"DELETE FROM SalesForceAccounts; DELETE FROM Fields; DELETE FROM CMTable; DELETE FROM SOQLTable; DELETE FROM Queries; " + sb;


            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            con.Close();

        }
         
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


            public ActionResult Query()  {

            if (LoginController.getLoginStatus())
            {
                var fielddata = SalesForceProcessor.LoadFields();

                if (fielddata.Count > 0)  {return View(fielddata);}
                else { return RedirectToAction("Login", "Login"); }
            }
            else { return RedirectToAction("Login", "Login"); }
          
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

            return RedirectToAction("ViewScheduledQueries", "Read");
        }

        //Defining a new query from scheduler


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ScheduleNewQuery(string query, DateTime date)
        {
            // Loads most recent query

            var NewQuery = "Select " + query + " from Account";

            SalesForceProcessor.CreateScheduledQuery(NewQuery, date);

            return RedirectToAction("ViewScheduledQueries", "Read");

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToDatabase(FormCollection form)
        {
            // Obtains the previous Query, executes it and stores it in the database

            List<DataLibrary.Models.SavedQueryModel> querylist = SalesForceProcessor.LoadQueries();

            int count = querylist.Count;

            var oldquery =  querylist[count - 1].Parameters;

            // I need to Remove ID

            oldquery.Replace("Id,", "").Replace("Id", "");

            // And Reinsert ID, split up by commas and remove whitespace

            string[] words = oldquery.Split(',', ' ');

            StringBuilder sb = new StringBuilder();


            sb.Append("Id,");

            foreach (string word in words)
            {
                // make each word begin uppercase + add a comma + space on all except the last one and 

                if (word == words.Last())
                {
                    sb.Append(ToUpperFirstLetter(word));
                }
                else
                {
                    sb.Append(ToUpperFirstLetter(word) + ",");
                }
            }



            var modelList = GenericQuery(sb.ToString());

            SalesForceProcessor.CreateSOQLRecord(modelList);

            return RedirectToAction("ViewSoqlResults", "Read");

        }


        public void GetSOQLIds()
        {
           var querylist = GenericQuery("Id");

            foreach (var query in querylist)
            {
                foreach(var item in query.FieldValueList)
                {
                    if(item == null) {continue; }

                    SalesForceProcessor.CreateSOQLId(item.ToString());

                }

            }

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

