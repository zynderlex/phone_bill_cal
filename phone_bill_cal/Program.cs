using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace phone_bill_cal
{
    /*bill entity for store data from log file*/
    public class Bill
    {
        public DateTime UsedDate;
        public double UsedTime;
        public double Charge;
        public string PhoneNO;
        public string Promotion;
    }

    class Program
    {
        
        static void Main(string[] args)
        {
            
            string str;
            string[] strArray;
            DateTime StartTime;
            DateTime EndTime;
            double UsedTime;
            double Charge;
            /*create list for store data*/
            List<Bill> Bills = new List<Bill>();
            /*read log*/
            StreamReader sr = new StreamReader("E:log.txt");
            
            while (sr.Peek() > 0) { 

                str = sr.ReadLine();
                strArray = str.Split('|');
                /*create transactiob obj for add transaction to list*/
                Bill CurrentTransaction = new Bill();

                /*convert string to datetime*/
                CurrentTransaction.UsedDate = DateTime.ParseExact(strArray[0],"dd/mm/yyyy", CultureInfo.InvariantCulture);

                /*convert string start time for calculate*/
                StartTime = DateTime.ParseExact(strArray[1].Substring(0, 8),"HH:mm:ss", CultureInfo.InvariantCulture);
                /*convert string end time for calculate*/
                EndTime = DateTime.ParseExact(strArray[1].Substring(strArray[1].Length -8), "HH:mm:ss", CultureInfo.InvariantCulture);

                /*calculate used time*/
                UsedTime = (EndTime - StartTime).TotalSeconds;

                CurrentTransaction.UsedTime = UsedTime;

                /*calculate charge with promotion*/
                if (UsedTime <= 60) {
                    Charge = (UsedTime/60) * 1.50;
                }
                else {
                    
                    Charge = ((UsedTime - 60)/60) * 0.50 + 1.5;
                }
                /*2 point decimal*/
                CurrentTransaction.Charge = Math.Round(Charge, 2);
                CurrentTransaction.PhoneNO = strArray[2];
                CurrentTransaction.Promotion = strArray[3];

                /*add current transaction to list*/
                Bills.Add(CurrentTransaction);
            }
            /*group and sum data in list*/
            var sumbill = Bills
                .GroupBy(b => b.PhoneNO)
                .Select(b => new {
                    PhoneNo = b.Key,
                    Charge = b.Sum(s => s.Charge),
                    Promotion = b.First().Promotion

                });

            foreach (var ts in sumbill) {
                /*show result*/
                Console.WriteLine(ts.Charge + " "+ ts.PhoneNo + " " + ts.Promotion);
            }

            Console.ReadKey();
        }
    }
}
