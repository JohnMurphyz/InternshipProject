using System.ComponentModel.DataAnnotations.Schema;

namespace InternshipProjectRetry.Models
{
    public class SalesForceAccountModel
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AutoID { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public string Website { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }  

        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }

    }

}