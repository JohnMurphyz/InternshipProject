using DataLibrary.DataAccess;
using DataLibrary.Models;
using InternshipProjectRetry.Models;
using System;
using System.Collections.Generic;

namespace DataLibrary.BussinessLogic
{
    public static class SalesForceProcessor
    {
        public static int CreateRecord(string accountNumber, string accountName, string website, string street, string city, string state, string country, string postalCode, string ownerName, string ownerEmail)
        {
            SalesForceAccountModel data = new SalesForceAccountModel
            {
                AccountNumber = accountNumber,
                AccountName = accountName,
                Website = website,
                Street = street,
                City = city,
                State = state,
                Country = country,
                PostalCode = postalCode,
                OwnerName = ownerName,
                OwnerEmail = ownerEmail

            };

            string sql = @"insert into dbo.SalesForceAccounts (AccountNumber, AccountName, Website, Street, City, State, Country, PostalCode, OwnerName, OwnerEmail) values (@AccountNumber, @AccountName, @Website, @Street, @City, @State, @Country, @PostalCode, @OwnerName, @OwnerEmail);";

            return SQLDataAccess.SaveData(sql, data);
        }
        // Everything

        public static List<SalesForceAccountModel> LoadAccounts()
        {
            string sql = @"select AccountNumber, AccountName, Website, Street, City, State, Country, PostalCode, OwnerName, OwnerEmail from dbo.SalesForceAccounts;";

            return SQLDataAccess.LoadData<SalesForceAccountModel>(sql);
        }

        // Just the names

        public static List<SalesForceAccountModel> LoadSalesForceAccounts()
        {
            string sql = @"select AccountName from dbo.SalesForceAccounts;";

            return SQLDataAccess.LoadData<SalesForceAccountModel>(sql);
        }


        public static int CreateFieldRecord(string fieldName, object fieldType)
        {
            FieldModel data = new FieldModel
            {
                FieldName = fieldName,
                FieldType = fieldType.ToString()

            };

            string sql = @"insert into dbo.Fields (FieldName, FieldType) values (@FieldName, @FieldType);";

            return SQLDataAccess.SaveData(sql, data);
        }




        public static List<FieldModel> LoadFields()
        {
            string sql = @"select FieldName, FieldType from dbo.Fields;";

            return SQLDataAccess.LoadData<FieldModel>(sql);
        }




        public static void CreateSavedQuery(string savedQuery, string parameters)
        {
            SavedQueryModel data = new SavedQueryModel

            {
                SavedQuery = savedQuery,
                Parameters = parameters

            };

            string sql = @"insert into dbo.Queries (SavedQuery, Parameters) values (@SavedQuery, @Parameters);";

            SQLDataAccess.SaveData(sql, data);

        }




        public static List<SavedQueryModel> LoadQueries()
        {
            string sql = @"select SavedQuery, Parameters from dbo.Queries;";

            return SQLDataAccess.LoadData<SavedQueryModel>(sql);
        }






        public static int CreateCMRecord(string partnerID, string companyName, string websiteUrl, string street, string city, string state, string country, string postalCode, string accountManagerName, string accountManagerEmail)
        {

            PartnerAccount data = new PartnerAccount
            {

                PartnerID = partnerID,
                CompanyName = companyName,
                WebsiteURL = websiteUrl,
                Street = street,
                City = city,
                State = state,
                Country = country,
                PostalCode = postalCode,
                AccountManagerName = accountManagerName,
                AccountManagerEmail = accountManagerEmail


            };

            string sql = @"insert into dbo.CMTable (PartnerID, CompanyName, WebsiteURL, Street, City, State, Country, PostalCode, AccountManagerName, AccountManagerEmail) values (@PartnerID, @CompanyName, @WebsiteURL, @Street, @City, @State, @Country, @PostalCode, @AccountManagerName, @AccountManagerEmail);";




            return SQLDataAccess.SaveData(sql, data);
        }


        public static int CreateCMRecord(PartnerAccount data)
        {

            string sql = @"insert into dbo.CMTable (PartnerID, CompanyName, WebsiteURL, Street, City, State, Country, PostalCode, AccountManagerName, AccountManagerEmail) values (@PartnerID, @CompanyName, @WebsiteURL, @Street, @City, @State, @Country, @PostalCode, @AccountManagerName, @AccountManagerEmail);";

            return SQLDataAccess.SaveData(sql, data);
        }


        public static List<PartnerAccount> LoadCMRecords()
        {
            string sql = @"select PartnerID, CompanyName, Street, City, State, Country, PostalCode, AccountManagerName, AccountManagerEmail from dbo.CMTable;";

            return SQLDataAccess.LoadData<PartnerAccount>(sql);
        }



        public static void CreateSOQLRecord(List<QueryModel> models)
        {
            foreach (QueryModel model in models)
            {
                string FieldName = model.FieldName;
                System.Diagnostics.Debug.WriteLine(FieldName);
                string columnexists = "select " + FieldName + "from dbo.SOQLTable";

                if (SQLDataAccess.ExecuteSql(columnexists) == 0)
                {
                    string addColumn = "alter table dbo.SOQLTable add " + FieldName + " VARCHAR(50);";
                    SQLDataAccess.ExecuteSql(addColumn);
                }

                foreach (string field in model.FieldValueList)
                {
                    if(field != null)
                    { 
                        try
                        {
                            string FieldValue = field;

                            // I need to add an ID finder part here

                            var ID = "Select Id from account WHERE " + FieldName + " = '" + FieldValue + "'";

                            var identifier = SQLDataAccess.ExecuteSql(ID);



                            string fieldsql = @"insert into dbo.SOQLTable (" + FieldName + ") VALUES ('" + FieldValue + " WHERE Id = " + identifier;
                            SQLDataAccess.SaveData(fieldsql, FieldValue);


                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message); 
                        }
                    }
                }
            }
        }

        public static List<QueryModel> LoadSOQLResults()
        {
            string sql = @"select Id from dbo.SOQLTable;";

            return SQLDataAccess.LoadData<QueryModel>(sql);
        }



        //I need to change these back to DateTime type & Bool

        public static int CreateScheduledQuery(string query, DateTime dateTime)
        {

            ScheduledQueryModel data = new ScheduledQueryModel
            {
                Query = query,
                ScheduledTime = dateTime

            };

            string sql = @"insert into dbo.ScheduledQueries (Query, ScheduledTime) values (@Query, @ScheduledTime);";

            return SQLDataAccess.SaveData(sql, data);

        }



        public static List<ScheduledQueryModel> LoadScheduledQueries()
        {
            string sql = @"select Query, ScheduledTime from dbo.ScheduledQueries;";

            return SQLDataAccess.LoadData<ScheduledQueryModel>(sql);
        }


    }
}



