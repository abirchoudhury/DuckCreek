using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckCreek.BranchManagerApp
{
    [Serializable]
    public class BranchDetail
    {        

        public BranchDetail()
        {

        }

        public string Address
        {
            get; set;
        }

        public string BranchName
        {
            get; set;
        }

        public string ChangeDate
        {
            get; set;
        }

        public string City
        {
            get; set;
        }

        public string RoutingNumber
        {
            get; set;
        }

        public string NewRoutingNumber
        {
            get; set;
        }

        public string StateCode
        {
            get; set;
        }

        public string ZipCode
        {
            get; set;
        }

        public string ZipExtension
        {
            get; set;
        }


    }
}
