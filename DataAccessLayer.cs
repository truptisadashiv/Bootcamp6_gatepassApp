using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using System.Text;
using System.Data.SqlClient;
using GatePass1.Models;
namespace GatePass1
{
    public class DataAccessLayer
    {

        NpgsqlConnection connectionObj;
        NpgsqlDataAdapter adapterObjDisplay;
        NpgsqlDataAdapter adapterObjSelectDate=new NpgsqlDataAdapter();
        NpgsqlDataAdapter adapterObjSelectStatus;
        NpgsqlDataAdapter adapterObjSelectExistsId = new NpgsqlDataAdapter();
        NpgsqlDataAdapter adapterObjSelectLogOut = new NpgsqlDataAdapter();
        NpgsqlDataAdapter adapterObjUpdateLogOut = new NpgsqlDataAdapter();
        NpgsqlDataAdapter adapterObjDisplayAsset = new NpgsqlDataAdapter();
        NpgsqlCommand selectCommandDisplay;
        NpgsqlCommand selectCommandDate;
        NpgsqlCommand selectCommandStatus;
        NpgsqlCommand selectCommandDisplayAsset;
        NpgsqlCommand selectCommandExistsId;
        NpgsqlCommand selectCommandLogOut;
        NpgsqlCommand updateCommandLogOut;
        DataSet datasetObjDisplay=new DataSet();
        DataSet datasetObjSelectDate = new DataSet();
        DataSet datasetObjSelectStatus = new DataSet();
        DataSet datasetObjSelectExistsId = new DataSet();
        DataSet datasetObjSelectLogOut = new DataSet();
        DataSet datasetObjDisplayAsset = new DataSet();
        public DataAccessLayer()
        {
            string com = "Server=127.0.0.1;Port=5432;Database=Gatepass;User Id=postgres; Password = W3lc0m3;";
            connectionObj = new NpgsqlConnection(com);
            adapterObjDisplay = new NpgsqlDataAdapter();
        }
        
        public IList<Gate> Display(int serialNo)
        {
            connectionObj.Open();
            selectCommandDisplay = new NpgsqlCommand("SELECT \"SerialNo\",\"EmployeeId\",\"FromLocation\",\"ToLocation\",\"FromDate\",\"ToDate\",\"BusinessJustification\",\"IssuedTo\",\"Status\" FROM public.\"EmployeeGatepassDetails\" where \"SerialNo\"=@SerialNo", connectionObj);
            selectCommandDisplay.Parameters.AddWithValue("@SerialNo", serialNo);
            adapterObjDisplay.SelectCommand = selectCommandDisplay;
            adapterObjDisplay.Fill(datasetObjDisplay);
            int r= selectCommandDisplay.ExecuteNonQuery();
            var employeeList = new List<Gate>();
            
            foreach (DataRow row in datasetObjDisplay.Tables[0].Rows)
            {
                var employeeObj = new Gate
                {
                    serialNo = Convert.ToInt32(row["SerialNo"]),
                    employeeId = row["EmployeeId"].ToString(),
                    fromLocation = row["FromLocation"].ToString(),
                    toLocation = row["ToLocation"].ToString(),
                    fromDate =(DateTime)row["FromDate"],
                    toDate = (DateTime)row["ToDate"],
                    status = row["Status"].ToString(),
                    businessJustification = row["BusinessJustification"].ToString(),
                    issuedTo = row["IssuedTo"].ToString(),
                   

                };
                employeeList.Add(employeeObj);
            }
            connectionObj.Close();
            return employeeList;
        }
        public IList<Asset> DisplayAsset(int serialNo)
        {
            connectionObj.Open();
            selectCommandDisplayAsset = new NpgsqlCommand("SELECT \"ItemSerialNo\",\"ItemDescription\",\"Quantity\" FROM public.\"EmployeeGatepassDetails\",public.\"RequestedAssets\" where \"EmployeeGatepassDetails\".\"SerialNo\"=\"RequestedAssets\".\"SerialNo\" and \"EmployeeGatepassDetails\".\"SerialNo\"=@SerialNo", connectionObj);
            selectCommandDisplayAsset.Parameters.AddWithValue("@SerialNo", serialNo);
            adapterObjDisplayAsset.SelectCommand = selectCommandDisplayAsset;
            adapterObjDisplayAsset.Fill(datasetObjDisplayAsset);
            int r = selectCommandDisplayAsset.ExecuteNonQuery();
            var assetList = new List<Asset>();
            foreach (DataRow row in datasetObjDisplayAsset.Tables[0].Rows)
            {
                var assetObj = new Asset
                {
                   
                    itemSerialNo = row["ItemSerialNo"].ToString(),
                    itemDescription = row["ItemDescription"].ToString(),
                    quantity = Convert.ToInt32(row["Quantity"]),

                };
                assetList.Add(assetObj);
            }
            connectionObj.Close();
            return assetList;
        }
        public DateTime SelectDate(int serialNo)
        {
            connectionObj.Open();
            selectCommandDate = new NpgsqlCommand("SELECT \"ToDate\" FROM public.\"EmployeeGatepassDetails\" WHERE \"SerialNo\"=@SerialNo", connectionObj);
            selectCommandDate.Parameters.AddWithValue("@SerialNo", serialNo);
            adapterObjSelectDate.SelectCommand = selectCommandDate;
            datasetObjSelectDate = new DataSet("mydataset");
            adapterObjSelectDate.Fill(datasetObjSelectDate);
            int r = selectCommandDate.ExecuteNonQuery();
            DataRow row = datasetObjSelectDate.Tables[0].Rows[0];
            DateTime toDate = (DateTime)row["ToDate"];
            connectionObj.Close();
            return toDate;
        }
        public void Insert(int gatePassId,DateTime logInTime)
        {

            NpgsqlCommand insertCommand = new NpgsqlCommand("INSERT INTO public.\"logInDetails\"( \"GatePassId\", \"LogInTime\")VALUES(@GatePassId,@LoginTime)", connectionObj);
            insertCommand.Parameters.AddWithValue("@GatePassId", gatePassId);
            insertCommand.Parameters.AddWithValue("@LoginTime", logInTime);
            adapterObjDisplay.InsertCommand = insertCommand;
            int RowsAffected = 0;
            try
            {
                connectionObj.Open();
                RowsAffected = insertCommand.ExecuteNonQuery();
            }
            catch (System.Exception)
            {
            }
            finally
            {
            connectionObj.Close();
            }
        }
        public string SelectStatus(int serialNo)
        {
            try
            {
                connectionObj.Open();
            }
            catch (SystemException e)
            {
                string a = e.Message;
            }
            selectCommandStatus = new NpgsqlCommand("SELECT \"Status\" FROM public.\"EmployeeGatepassDetails\" WHERE \"SerialNo\"=@SerialNo ", connectionObj);
            selectCommandStatus.Parameters.AddWithValue("@SerialNo", serialNo);
            datasetObjSelectStatus = new DataSet("mydataset");
            adapterObjSelectStatus = new NpgsqlDataAdapter(selectCommandStatus);
            adapterObjSelectStatus.SelectCommand = selectCommandStatus;
            adapterObjSelectStatus.Fill(datasetObjSelectStatus);
            object dbProductId = selectCommandStatus.ExecuteScalar(); //get the result into object
            if (dbProductId != null)
            { 
                DataRow dataRow = datasetObjSelectStatus.Tables[0].Rows[0];
                connectionObj.Close();
                return dataRow["Status"].ToString();
            }
            else
            {
                return "notfound";
            }
           
        }

        public int SelectExistsId(int serialNo)
        {
            int i;
            try
            {
                connectionObj.Open();
            }
            catch (SystemException e)
            {
                string a = e.Message;
            }
            selectCommandExistsId = new NpgsqlCommand("SELECT max(\"GatePassEntryNo\") FROM public.\"logInDetails\" where \"GatePassId\"=@SerialNo ", connectionObj);
            selectCommandExistsId.Parameters.AddWithValue("@SerialNo", serialNo);

            datasetObjSelectExistsId = new DataSet("mydataset");
            adapterObjSelectExistsId = new NpgsqlDataAdapter(selectCommandExistsId);
            adapterObjSelectExistsId.SelectCommand = selectCommandExistsId;
            adapterObjSelectExistsId.Fill(datasetObjSelectExistsId);
            object dbProductId = selectCommandExistsId.ExecuteScalar();
            if (dbProductId != null || dbProductId != DBNull.Value) 
                i = Convert.ToInt32(dbProductId);
            else
                i=0;
            connectionObj.Close();
            return i;
        }
        public  int SelectLogOut(int SerialNo)
        {
            int i;
            try
            {
                connectionObj.Open();
            }
            catch (SystemException e)
            {
                string a = e.Message;
            }
            selectCommandLogOut = new NpgsqlCommand("SELECT \"GatePassEntryNo\" FROM public.\"logInDetails\" where \"GatePassEntryNo\"=@SerialNo AND \"LogOutTime\" IS NULL;", connectionObj);
            selectCommandLogOut.Parameters.AddWithValue("@SerialNo", SerialNo);

            datasetObjSelectLogOut = new DataSet("mydataset");
            adapterObjSelectLogOut = new NpgsqlDataAdapter(selectCommandLogOut);
            adapterObjSelectLogOut.SelectCommand = selectCommandLogOut;
            adapterObjSelectLogOut.Fill(datasetObjSelectLogOut);
            i = Convert.ToInt32(selectCommandLogOut.ExecuteScalar());
            connectionObj.Close();
            return i;
        }
        public void UpdateLogOut(int SerialNo,DateTime tout)
        {
            updateCommandLogOut = new NpgsqlCommand("UPDATE public.\"logInDetails\" SET  \"LogOutTime\"=@LogOutTime WHERE \"GatePassEntryNo\" = @SerialNo", connectionObj);
            updateCommandLogOut.Parameters.AddWithValue("@SerialNo", SerialNo);
            updateCommandLogOut.Parameters.AddWithValue("@LogOutTime", tout);
            adapterObjUpdateLogOut.UpdateCommand = updateCommandLogOut;
            connectionObj.Open();
            int up = updateCommandLogOut.ExecuteNonQuery();
            connectionObj.Close();
        }
    }
}
