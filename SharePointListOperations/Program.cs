using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using SP = Microsoft.SharePoint.Client;
using System.Data.SqlClient;
using System.Configuration;

namespace SharePointListOperations
{
    class Program
    {

        static void Main(string[] args)
        {
            //Versioninfo
            Console.WriteLine("Ver 1.0.1");


            string amplio_connstring = ConfigurationManager.AppSettings["amplio_connectionstring"] + "BalKoc%22;";
            string mes_connstring = ConfigurationManager.AppSettings["mod_connectionstring"] + "MoMe19l.";

            string testedQtyQuery = ConfigurationManager.AppSettings["tested_qty_query"];
            string plannedQtyQuery = ConfigurationManager.AppSettings["planned_qty_query"];
            string clockQuery = ConfigurationManager.AppSettings["clock_query"];

            bool dateBasedQuery = true;
            string testedQty = "";
            string plannedQty = "";

            List<string> deviations = new List<string>();

            SqlConnection amplio_conn = new SqlConnection(amplio_connstring);
            SqlConnection mes_conn = new SqlConnection(mes_connstring);

            bool amplio_is_connected = LSSqlOperations.ConnectToSqlServer(amplio_conn, amplio_connstring);
            bool mes_is_connected = LSSqlOperations.ConnectToSqlServer(mes_conn, mes_connstring);

            if (amplio_is_connected)
            {
                testedQty = LSSqlOperations.SqlQuery(amplio_conn, testedQtyQuery, dateBasedQuery, null, null, null, null, "tested_count");
            }

            if (mes_is_connected)
            {
                plannedQty = LSSqlOperations.SqlQuery(mes_conn, plannedQtyQuery, false, null, null, null, null, "other");
                deviations = LSSqlOperations.SQLStringListQueryFromScript(mes_conn, "deviations.sql", "devIron", "devGenerator", "devFinal", null, null);
            }           

            string siteCollectionUrl = "https://laurastar.sharepoint.com/sites/MonitoringDatas";
            string userName = "bakocsis@laurastar.com";
            string password = "BalKoc%42";
            Program obj = new Program();

            ClientContext ctx = new ClientContext(siteCollectionUrl);

            try
            {
                ctx = obj.ConnectToSharePointOnline(siteCollectionUrl, userName, password, ctx);
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                Console.WriteLine(msg);

            }

            SP.List oList = ctx.Web.Lists.GetByTitle("ProductionData");
            ListItem oListItem = oList.GetItemById(1);

            oListItem["Tested"] = Convert.ToInt32(testedQty);
            oListItem["Plan"] = Convert.ToInt32(plannedQty);
            oListItem["LastUpdate"] = DateTime.Parse(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"));
            oListItem["IronDeviation"] = Convert.ToInt32(deviations.ElementAt(0).ToString());
            oListItem["GeneratorDeviation"] = Convert.ToInt32(deviations.ElementAt(1).ToString());
            oListItem["FinalDeviation"] = Convert.ToInt32(deviations.ElementAt(2).ToString());


            oListItem.Update();

            ctx.ExecuteQuery();
        }
        public ClientContext ConnectToSharePointOnline(string siteCollUrl, string userName, string password, ClientContext ctx_local)
        {

            //Namespace: It belongs to Microsoft.SharePoint.Client
            //ClientContext ctx_local = new ClientContext(siteCollUrl);

            // Namespace: It belongs to System.Security
            SecureString secureString = new SecureString();
            password.ToList().ForEach(secureString.AppendChar);

            // Namespace: It belongs to Microsoft.SharePoint.Client
            ctx_local.Credentials = new SharePointOnlineCredentials(userName, secureString);

            // Namespace: It belongs to Microsoft.SharePoint.Client
            Site mySite = ctx_local.Site;

            ctx_local.Load(mySite);
            ctx_local.ExecuteQuery();

            Console.WriteLine(mySite.Url.ToString());

            return ctx_local;
        }

        


    }
}


