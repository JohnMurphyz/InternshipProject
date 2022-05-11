using InternshipProjectRetry.com.salesforce.enterprise;
using InternshipProjectRetry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;

namespace InternshipProjectRetry.Controllers
{
    public class LoginController : Controller
    {
        public static Boolean Loginstatus { get; set; }

        private static SforceService binding { get; set; }

        private static LoginResult LoginResult { get; set; }

        // I need to ensure it's logged in before returning it 
        public static SforceService GetSforceService()
        {
            if (binding == null)
            {
                binding = new SforceService();
                return binding;

            }
            else

                return binding;

        }


        public static bool getLoginStatus()
        {
            return Loginstatus;
        }


        public static LoginResult GetLoginResult()
        {
            if (LoginResult == null)
            {
                LoginResult = new LoginResult();
                return LoginResult;
            }
            else
                return LoginResult;

        }
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                binding = new SforceService();

                try
                {
                    LoginResult = binding.login(model.Username, model.Password + model.Token);
                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {
                    // This is likley to be caused by bad username or password
                    binding = null;
                    //lOG ex
                    return View();
                }
                catch (Exception)
                {
                    // This is something else, probably communication issue
                    binding = null;
                    //lOG ex

                    return View();
                }
                //Change the binding to the new endpoint
                binding.Url = LoginResult.serverUrl;
                //Create a new session header object and set the session id to that returned by the login
                binding.SessionHeaderValue = new SessionHeader();
                binding.SessionHeaderValue.sessionId = LoginResult.sessionId;

                // create required sessions 
                System.Diagnostics.Debug.WriteLine("Logged In");
                Loginstatus = true;

                return RedirectToAction("LoadQueryPage", "Query");

            }
            else
                System.Diagnostics.Debug.WriteLine("Not Logged In");
            Loginstatus = false;
            return View();

        }


        public ActionResult logout() {
            try
            {
                binding.logout();
                System.Diagnostics.Debug.WriteLine("Logged out.");
                Loginstatus=false;
                return RedirectToAction("Index", "Home");
            }
            catch (SoapException e)
            {
                System.Diagnostics.Debug.WriteLine
                ("An unexpected error has occurred: " +
                                           e.Message + "\n" + e.StackTrace);
                return RedirectToAction("Index", "Home");

            }

        }

    }
}