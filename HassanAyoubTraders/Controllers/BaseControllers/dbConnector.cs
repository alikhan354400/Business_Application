using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HassanAyoubTraders.Controllers.BaseControllers
{
    public class dbConnector
    {
        private SqlConnection SqlConn = null;

        public SqlConnection GetConnection
        {
            get { return SqlConn; }
            set { SqlConn = value; }
        }

        public dbConnector()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString; SqlConn = new SqlConnection(ConnectionString);
        }
    }
}