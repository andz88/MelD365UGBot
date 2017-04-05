using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Description;
using System.Web;

namespace Barhead.Innovation.CRM
{
    [Serializable]
    public class CrmDataConnection
    {

        public static IOrganizationService GetOrgService()
        {
            var soapOrgUrl = ConfigurationManager.AppSettings["CrmServerUrl"].ToString() + "/XRMServices/2011/Organization.svc";
            var username = ConfigurationManager.AppSettings["CrmUsername"].ToString();
            var password = ConfigurationManager.AppSettings["CrmPassword"].ToString();

            var credentials = new ClientCredentials();
            credentials.UserName.UserName = username;
            credentials.UserName.Password = password;
            var serviceUri = new Uri(soapOrgUrl);
            var proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
            proxy.EnableProxyTypes();
            return proxy;
        }
    }
}