using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity; 
using HassanAyoubTraders.Models.EF;

namespace HassanAyoubTraders.PrintReports
{
    public class BaseUI : System.Web.UI.Page
    {
        public HassanAyoubDBEntities dbContext = new HassanAyoubDBEntities();
        ApplicationUserManager _userManager;

        public BaseUI()
        {
        }

        public string PopulateRegEmailBody(string userName, string url)
        {
            string body = string.Empty;

            using (var reader = new StreamReader(Server.MapPath("~/CustomHtml/Registration.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{UserName}", userName);
            body = body.Replace("{Url}", url);

            return body;
        }

        public string GetAppSettings(string Name)
        {
            if (ConfigurationManager.AppSettings[Name] != null)
                return ConfigurationManager.AppSettings[Name];
            else
                return string.Empty;
        }

        public void FillDDL<T>(DropDownList DDL, List<T> list, string textField, string valueField)
        {
            DDL.DataTextField = textField;
            DDL.DataValueField = valueField;
            DDL.DataSource = list;
            DDL.DataBind();
            DDL.Items.Insert(0, new ListItem("Select", "0"));
            DDL.SelectedIndex = 0;
        }

        public static int GetUserId()
        {
            FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string userData = ticket.UserData;
            string[] split = userData.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);

            return int.Parse(split[1].ToString());
        }

        public static string GetUserRole()
        {
            FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string userData = ticket.UserData;
            string[] split = userData.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);

            return split[0].ToString();
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public string GetCurrentUserRole()
        {
            try
            {
                var user = dbContext.AspNetUsers.FirstOrDefault(x => x.UserName.Equals(User.Identity.Name));
                if (user == null || user.Id == null)
                {
                    return "";
                }
                else
                {
                    _userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    var roles = _userManager.GetRoles(user.Id);
                    return roles[0];
                }
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        public DataTable GetReportData(string header, string title)
        {
            DataTable dtReport = new DataTable();
            dtReport.Columns.Add("ID", typeof(Int32));
            dtReport.Columns.Add("Header", typeof(string));
            dtReport.Columns.Add("Title", typeof(string));

            dtReport.Rows.Add(1, header, title);
            return dtReport;
        }


    }
}