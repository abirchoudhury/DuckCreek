using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckCreek.BranchManagerApp.BankService;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Globalization;

namespace DuckCreek.BranchManagerApp
{
    public class Program
    {
        //TODO: Change file path value to test it on your own machine.
        private static string path = "C:/Users/beerw/Desktop/DuckCreek.BranchManagerApp-Candidate/DuckCreek.BranchManagerApp - Candidate/DuckCreek.BranchManagerApp/XmlOutput";
        static void Main(string[] args)
        {
            BankServiceClient bankServiceClient = new BankServiceClient();

            //Doesn't seem to work, returns all records even with state code specified.
            var banksFromService = bankServiceClient.GetBanksForState("mo").ToList();

            //Custom list to hold the branch details.
            var bankList = new List<BranchDetail>();

            //Looping through the service list, converting data into BranchDetail class,
            //Adding converted branches to list.
            foreach (var bank in banksFromService)
            {
                var x = new BranchDetail();

                x.Address = bank.Address;
                x.BranchName = bank.BranchName;
                x.ChangeDate = bank.ChangeDate;
                x.City = bank.City;
                x.NewRoutingNumber = bank.NewRoutingNumber;
                x.RoutingNumber = bank.RoutingNumber;
                x.StateCode = bank.StateCode;
                x.ZipCode = bank.ZipCode;
                x.ZipExtension = bank.ZipExtension;

                bankList.Add(x);
            }

            //Retrieve all branches from state code "MO"
            List<BranchDetail> moBranches = bankList.Where(a => a.StateCode == "MO").ToList();

            //Retrieve all branches from Springfield, to write to xml file.
            List<BranchDetail> springfieldBranches = moBranches.Where(a => a.City == "SPRINGFIELD").ToList();

            //Write to an xml file the details for all branches located in Springfield
            if (springfieldBranches.Any())
            {
                WriteXML(springfieldBranches, "SpringfieldBranches");
            }

            //List to hold branches that fail routing number validation.
            var validationFailedBranches = new List<BranchDetail>();
            foreach (var sb in springfieldBranches)
            {
                //If routing number is valid, move on to the next element.
                if (ValidateRoutingNumber(sb.RoutingNumber))
                {
                    continue;
                }
                //Otherwise, add the object to list declared above.
                else
                {
                    validationFailedBranches.Add(sb);
                }
            }

            //Write to an xml file the details for all branches that fail validation.
            if (validationFailedBranches.Any())
            {
                WriteXML(validationFailedBranches, "ValidationFailedBranches");
            }

            //List to hold branches returned from date range >= 01-01-2017
            var dateRangeBranches = new List<BranchDetail>();
            foreach (var b in bankList)
            {
                DateTime date = DateTime.MinValue;
                DateTime minRange = new DateTime(2017, 01, 01);
                //Assuming all ChangeDate values will contain the yyyy-mm-dd
                string split = b.ChangeDate.Substring(0, 10);
                if (DateTime.TryParse(split, out date))
                {
                    if (date >= minRange)
                    {
                        dateRangeBranches.Add(b);
                    }
                }
            }

            //Write the xml file
            if (dateRangeBranches.Any())
            {
                WriteXML(dateRangeBranches, "DateRangeBranches");
            }

            bankServiceClient.Close();
        }


        /// <summary>
        ///Creates and saves xml file in disk
        /// </summary>
        /// <param name="branchDetails"></param>
        /// <param name="fileName"></param>
        public static void WriteXML(List<BranchDetail> branchDetails, string fileName)
        {
            var xml = new XElement("BranchDetails", branchDetails.Select(a => new XElement("Bank",
                                                                               new XAttribute("Address", a.Address),
                                                                               new XAttribute("BranchName", a.BranchName),
                                                                               new XAttribute("ChangeDate", a.ChangeDate),
                                                                               new XAttribute("City", a.City),
                                                                               new XAttribute("NewRoutingNumber", a.NewRoutingNumber),
                                                                               new XAttribute("RoutingNumber", a.RoutingNumber),
                                                                               new XAttribute("StateCode", a.StateCode),
                                                                               new XAttribute("ZipCode", a.ZipCode),
                                                                               new XAttribute("ZipExtension", a.ZipExtension)
                                                                               )));
            xml.Save(path + "//" + fileName + ".xml");
        }

        /// <summary>
        /// Validates routing numbers for bank branches by using the algorithm
        /// provided on the instructions
        /// </summary>
        /// <param name="routingNum"></param>
        /// <returns></returns>
        public static bool ValidateRoutingNumber(string routingNum)
        {
            var numList = new List<int>();

            foreach (var n in routingNum)
            {
                if (int.TryParse(n.ToString(), out int result))
                {
                    numList.Add(result);
                }
            }

            if (numList.Count != 9 || numList.Any() == false)
            {
                return false;
            }

            int productTotal = 0;

            productTotal += numList[0] * 3;
            productTotal += numList[1] * 7;
            productTotal += numList[2] * 1;
            productTotal += numList[3] * 3;
            productTotal += numList[4] * 7;
            productTotal += numList[5] * 1;
            productTotal += numList[6] * 3;
            productTotal += numList[7] * 7;
            productTotal += numList[8] * 1;

            if (productTotal % 10 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
