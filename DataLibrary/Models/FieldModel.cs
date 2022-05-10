using System.Collections.Generic;

namespace InternshipProjectRetry.Models
{
    public class FieldModel
    {
        public string FieldName { get; set; }

        public string FieldType { get; set; }

    }


    public class FieldViewModel
    {
        public IList<FieldModel> FieldItems { get; set; }

    }

}