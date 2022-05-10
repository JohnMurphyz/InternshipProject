using System;
using System.ComponentModel.DataAnnotations;

namespace InternshipProjectRetry.Models
{
    public class PartnerAccount
    {

        [Key]
        public string PartnerID { get; set; }

        public string CompanyName { get; set; }

        public string WebsiteURL { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        // I can't access Currency in what I'm doing
        public string Currency { get; set; }

        public string AccountManagerName { get; set; }

        public string AccountManagerEmail { get; set; }



    }
}
