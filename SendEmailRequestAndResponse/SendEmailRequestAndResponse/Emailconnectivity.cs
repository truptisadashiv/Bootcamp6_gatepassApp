using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;

namespace SendEmailRequestAndResponse
{
    public class Emailconnectivity
    {



        NpgsqlConnection connectionObj;
        NpgsqlDataAdapter adapterObj;
        NpgsqlCommand InsertCommand;
        NpgsqlCommand selectcommand;
        NpgsqlCommand deletecommand;
        NpgsqlCommand updatecommand;
        DataSet datasetobj;

        public Emailconnectivity()
        {
            string com = "Server=127.0.0.1;Port=5432;Database=Trupti;User Id=postgres; Password = W3lc0m3; ";
            connectionObj = new NpgsqlConnection(com);
            adapterObj = new NpgsqlDataAdapter();
        }


      


        public string select(string EmployeeId)
        {

            try
            {
                connectionObj.Open();
            }
            catch (SystemException e)
            {
                string a = e.Message;

            }

            selectcommand = new NpgsqlCommand("SELECT \"EmployeeEmailId\" FROM public.\"Employee\" where \"EmployeeId\"=@EmployeeId", connectionObj);
                selectcommand.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                datasetobj = new DataSet("mydataset");
                adapterObj = new NpgsqlDataAdapter(selectcommand);
                adapterObj.Fill(datasetobj);
                DataRow dtr = datasetobj.Tables[0].Rows[0];
            connectionObj.Close();
                return dtr["EmployeeEmailId"].ToString();
            
          
        }
        public int InsertToGatepassTable(string Empid,string from, string to, DateTime fromtime, DateTime totime,string status)
        {

            InsertCommand = new NpgsqlCommand("INSERT INTO public.\"EmployeeAcceptedGatepassDetails\"( \"EmployeeId\", \"FromLocation\", \"ToLocation\", \"FromDate\", \"ToDate\",\"Status\")VALUES(@Empid, @Efrom, @Eto, @Efdate,  @Etodate,@status); ", connectionObj);
            adapterObj.InsertCommand = InsertCommand;
           
            
            InsertCommand.Parameters.AddWithValue("@Empid", Empid);

          //  InsertCommand.Parameters.AddWithValue("@Ename", EmployeeName);

            InsertCommand.Parameters.AddWithValue("@Efrom", from);
            InsertCommand.Parameters.AddWithValue("@Eto", to);

            InsertCommand.Parameters.AddWithValue("@Efdate", fromtime);
            InsertCommand.Parameters.AddWithValue("@Etodate", totime);
            InsertCommand.Parameters.AddWithValue("@status", status);
            int RowsAffected = 0;

            try
            {
                connectionObj.Open();
                RowsAffected = InsertCommand.ExecuteNonQuery();
            }
            catch (System.Exception)
            {
            }
            finally
            {
                connectionObj.Close();
            }
            return RowsAffected;




          
        }
        public int  selectall(string EmployeeId,string from,string to,DateTime fromtime,DateTime totime)
        {
            
            try
            {
                connectionObj.Open();
            }
            catch (SystemException e)
            {
                string a = e.Message;

            }
            int RowsAffected = 0;
            int a1;
            selectcommand = new NpgsqlCommand("SELECT Count(\"EmployeeId\") FROM public.\"EmployeeAcceptedGatepassDetails\" where \"EmployeeId\"=@EmployeeId AND\"FromLocation\"=@FromLoca AND\"ToLocation\"=@ToLocation AND\"FromDate\"=@FromDate AND\"ToDate\"=@ToDate", connectionObj);
            
            selectcommand.Parameters.AddWithValue("@EmployeeId", EmployeeId);
            selectcommand.Parameters.AddWithValue("@FromLoca", from);
            selectcommand.Parameters.AddWithValue("@ToLocation", to);
            selectcommand.Parameters.AddWithValue("@FromDate", fromtime);
            selectcommand.Parameters.AddWithValue("@ToDate", totime);

            datasetobj = new DataSet("mydataset");
            adapterObj = new NpgsqlDataAdapter(selectcommand);
            adapterObj.Fill(datasetobj);
           // DataRow dtr = datasetobj.Tables[0].Rows[0];
            if(selectcommand.ExecuteNonQuery()!=0)
            {
               a1 = 1;
            }
            else
            {
                a1 = 0;
            }
          //  int a1 = (int)selectcommand.ExecuteScalar();
            connectionObj.Close();
            // return dtr["EmployeeEmailId"].ToString();
            return a1;


        }
        public string selectmanagerid(string Id)
        {

            try
            {
                connectionObj.Open();
            }
            catch (SystemException e)
            {
                string a = e.Message;
            }
            selectcommand = new NpgsqlCommand("SELECT \"EmployeeEmailId\",\"EmployeeId\",\"EmployeeName\",\"ManagerId\" FROM public.\"Employee\"where \"EmployeeId\" IN(SELECT \"ManagerId\" FROM public.\"Employee\"where \"EmployeeId\"=@EmployeeId);", connectionObj);
            selectcommand.Parameters.AddWithValue("@EmployeeId", Id);

            datasetobj = new DataSet("mydataset");
            adapterObj = new NpgsqlDataAdapter(selectcommand);
            adapterObj.Fill(datasetobj);
            DataRow dtr = datasetobj.Tables[0].Rows[0];
            connectionObj.Close();
            return dtr["EmployeeEmailId"].ToString();
        }

       /*  public int sl(string CustomerID)
         {
             
             adapterObj.Fill(da);
             int Rowa = 0;
             deletecommand = new NpgsqlCommand("Delete from public.\"Customer\" where \"CustomerID\"=@CustomerID", connectionObj);
             adapterObj.DeleteCommand = deletecommand;

             deletecommand.Parameters.AddWithValue("@CustomerID", CustomerID);

             foreach (DataRow catRow in da.Tables[0].Rows)
             {
                 if ((int)catRow["CustomerID"] == CustomerID)
                 {
                     catRow.Delete();
                     break;
                 }

             }


             try
             {
                 Rowa = adapterObj.Update(da);
             }
             catch (Exception e)
             {
                 string a = e.Message;
             }
             return Rowa;
         }*/

         /*  public int update(int CategoryID)
           {
               updatecommand.Parameters.AddWithValue("@Description", Description);
               updatecommand.Parameters.AddWithValue("@CategoryID", CategoryID);

               foreach (DataRow catRow in datasetobj.Tables[0].Rows)
               {
                   if ((int)catRow["CategoryID"] == CategoryID)
                   {
                       catRow["Description"] = Description;
                   }
               }

               int rows = adapterObj.Update(datasetobj);
               return rows;

           }*/

    }
}