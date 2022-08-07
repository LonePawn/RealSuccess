using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Specialized;
using System.Net;
using Newtonsoft.Json;

public class SK_ApiConnector : MonoBehaviour
{
    private static String skApiServerUrl = "http://localhost/RealSuccess/SmartKobo/";
    //private const Decimal SERVICE_CHARGE = 0.50m;   //fifty kobo. This is the service charge for making direct funds transfer. Amount in Naira and not yet finalised.

    private String _authId;         // This will hold the authId after user authentication in order to allow access to authorised data from OPS server
    private String _email;          // This will hold the authorised user's email. This App is limited to 1 concurrent user email per device.
                                    // Meaning 2 or more people can not simultaneously login on the same device at the same time.

    private SK_NavBar navBar;

    public BackgroundProcess backgroundProcessIndicator;
    private static BackgroundProcess bpi;

    public String GetEmail()
    {
        return _email;
    }

    public String GetAuthId()
    {
        return _authId;
    }


    // Start is called before the first frame update
    void Start()
    {
        bpi = backgroundProcessIndicator;
        navBar = GameObject.FindGameObjectWithTag("NavBar").GetComponent<SK_NavBar>();
    }

    public static void UpdateSkApiServerUrl(String s)
    {
        skApiServerUrl = s;
    }

    /// <summary>
    /// The following method will be used for interfacing with the RealSuccess Restful APIs.
    /// It serves as the only outlet and inlet for data exchange btw the client and server
    /// </summary>
    /// <param name="form">The form data to post to the remote resource</param>
    /// <param name="url">The url of the remote resource</param>
    /// <returns>The json encoded string response from the server</returns>
    public static String SkApiRequest(NameValueCollection form, String url)
    {
        bpi.ShowRunningProcess("Request Processing...");
        String rValue = "bug Snitch";
        try
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                byte[] result = wc.UploadValues(url, "POST", form);
                rValue = System.Text.Encoding.UTF8.GetString(result);
            }
        }
        catch (Exception e)
        {
            // An unexpected error ocurred and as such the server could not be reached.
            // manually construct an SkApiResponse that signifies failure to reach server
            rValue = "{'API' : 'SMK'," +
                "'RequestResult' : 'failure'," +
                "'failReason' : 'Could not reach server'," +
                "'Data' : {'':''}}";
        }

        // Could not reach server. Manually construct an SkApiResponse that signifies failure to reach server
        if (rValue.ToString() == "")
        {
            Debug.Log("rValue is null");
            rValue = "{'API' : 'SMK'," +
                "'RequestResult' : 'failure'," +
                "'failReason' : 'Could not reach server'," +
                "'Data' : {'':''}}";
        }

        return rValue;
    }

    #region SMK API request coroutines externally


    public IEnumerator UserSignUp(String email)
    {
        NameValueCollection form = new NameValueCollection();
        form.Add("email", email);
        String resultText = SkApiRequest(form, skApiServerUrl + "smk_api_signUp.php");
        yield return resultText;

        NormalOpsResponse jsonResponse = JsonConvert.DeserializeObject<NormalOpsResponse>(resultText);
        OnUserSignUp(jsonResponse.RequestResult == "success", email, jsonResponse.failReason);
    }

    public IEnumerator AuthenticateUser(String email, String pWord)
    {
        NameValueCollection form = new NameValueCollection();
        form.Add("email", email);
        form.Add("pwd", pWord);
        String resultText = SkApiRequest(form, skApiServerUrl + "smk_api_authenticateUser.php");
        yield return resultText;

        NormalOpsResponse jsonResponse = JsonConvert.DeserializeObject<NormalOpsResponse>(resultText);
        String authId = "";
        jsonResponse.Data.TryGetValue("AuthId", out authId);
        OnAuthenticateUser(jsonResponse.RequestResult.Equals("success"), email, authId, jsonResponse.failReason);
    }

    public IEnumerator ChangePassword(String email, String oldPword, String newPword)
    {
        NameValueCollection form = new NameValueCollection();
        form.Add("email", email);
        form.Add("oldPwd", oldPword);
        form.Add("newPwd", newPword);
        String resultText = SkApiRequest(form, skApiServerUrl + "smk_api_changePassword.php");
        yield return resultText;

        NormalOpsResponse jsonResponse = JsonConvert.DeserializeObject<NormalOpsResponse>(resultText);
        OnChangePassword(jsonResponse.RequestResult == "success", jsonResponse.failReason);
    }

    public IEnumerator CompletePasswordReset(String email, String resetToken, String newPword)
    {
        NameValueCollection form = new NameValueCollection();
        form.Add("email", email);
        form.Add("resetToken", resetToken);
        form.Add("newPwd", newPword);
        String resultText = SkApiRequest(form, skApiServerUrl + "smk_api_completePasswordReset.php");
        yield return resultText;

        NormalOpsResponse jsonResponse = JsonConvert.DeserializeObject<NormalOpsResponse>(resultText);
        OnCompletePasswordReset(jsonResponse.RequestResult == "success", jsonResponse.failReason);
    }

    /*
    public IEnumerator GetAccountBalance(String email, String authId)
    {
        NameValueCollection form = new NameValueCollection();
        form.Add("email", email);
        form.Add("authId", authId);
        String resultText = SkApiRequest(form, skApiServerUrl + "smk_api_getAccountBalance.php");
        yield return resultText;

        NormalOpsResponse jsonResponse = JsonConvert.DeserializeObject<NormalOpsResponse>(resultText);
        if (jsonResponse.RequestResult.Equals("success"))
        {
            OnGetAccountBalance(true, Decimal.Parse(jsonResponse.Data["AvailableBalance"]), Decimal.Parse(jsonResponse.Data["Balance"]), jsonResponse.failReason);
        }
        else
        {
            OnGetAccountBalance(false, 0, 0, jsonResponse.failReason);
        }
    }
    */

    public IEnumerator InitiatePasswordReset(String email)
    {
        NameValueCollection form = new NameValueCollection();
        form.Add("email", email);
        String resultText = SkApiRequest(form, skApiServerUrl + "smk_api_initiatePasswordReset.php");
        yield return resultText;

        Debug.Log(resultText);
        // The error we are currently getting here is due to the response that contains a php error notifying us of email client not set up for apache server
        NormalOpsResponse jsonResponse = JsonConvert.DeserializeObject<NormalOpsResponse>(resultText);
        OnInitiatePasswordReset(jsonResponse.RequestResult == "success", email, jsonResponse.failReason);
    }

    public IEnumerator LogOut(String authId)
    {
        NameValueCollection form = new NameValueCollection();
        form.Add("authId", authId);
        String resultText = SkApiRequest(form, skApiServerUrl + "smk_api_logOut.php");
        yield return resultText;

        NormalOpsResponse jsonResponse = JsonConvert.DeserializeObject<NormalOpsResponse>(resultText);
        OnLogOut(jsonResponse.RequestResult.Equals("success"), jsonResponse.failReason);
    }

    /*
    public IEnumerator MakePayment(String email, String authId, Decimal amount, String description, String beneficiary, String pWord)
    {
        NameValueCollection form = new NameValueCollection();
        form.Add("email", email);
        form.Add("pwd", pWord);
        form.Add("authId", authId);
        form.Add("amount", amount.ToString());
        form.Add("description", description);
        form.Add("beneficiary", beneficiary);
        String resultText = OpsApiRequest(form, OpsApiServerUrl + "ops_api_makePayment.php");
        yield return resultText;

        Debug.Log("raw text reply of make payment: " + resultText);
        NormalOpsResponse jsonResponse = JsonConvert.DeserializeObject<NormalOpsResponse>(resultText);
        String transactionId = "";
        jsonResponse.Data.TryGetValue("TransactionId", out transactionId);
        OnMakePayment(jsonResponse.RequestResult == "success",
            new LedgerReader(email, amount, TransactionType.Debit, DateTime.Now.ToString(), Guid.NewGuid().ToString(), description),
            transactionId, jsonResponse.failReason);
    }
    */


    #endregion

    #region SMK API callbacks
    protected void OnUserSignUp(bool success, String email, String failReason)
    {
        if (success)
        {
            bpi.ShowProcessComplete("Sign-up process successful", "Use your email - " + email + " and password to sign-in to your account");
        }
        else
        {
            bpi.ShowProcessComplete("Attempt to sign-up with OPS failed", failReason);
        }
    }

    protected void OnAuthenticateUser(bool success, String email, String authId, String failReason)
    {
        if (success)
        {
            // Authorization successful. Keep the authId and authorised email for future use
            _email = email;
            _authId = authId;
            Debug.Log("User " + _email + " successfully logged in");

            DataModel.Email = _email;
            DataModel.AuthId = _authId;

            //StartCoroutine(FetchProtectedPayments(_email, _authId));
            //StartCoroutine(FetchLedger(_email, _authId));

            // move the logIn Page out of the way and slide in the taskbar buttons
            navBar.SlideOutLogInPage();
            navBar.SlideInFooter();

            // update the login status and show the change password button
            Color g = new Color(45f / 255f, 137f / 255f, 67f / 255f);
            //navBar.UpdateSettingsPageStatus("Logged in as " + _email, g);
            //navBar.ShowChangePasswordButton();

            bpi.gameObject.SetActive(false);

            navBar.UpdateOverviewPage();
        }
        else
        {
            // log in failed. Let the user know what went wrong by displaying the failReason message from the OPS server
            Debug.Log(failReason);
            bpi.ShowProcessComplete("Log In Failed!", failReason);
        }
    }

    protected void OnChangePassword(bool success, String failReason)
    {

    }

    protected void OnCompletePasswordReset(bool success, String failReason)
    {
        if (success)
        {
            bpi.ShowProcessComplete("Successfully Changed Password", "Your password was successfully changed");
        }
        else
        {
            bpi.ShowProcessComplete("Attempt to Change Password failed!", failReason);
        }
    }

    protected void OnGetAccountBalance(bool success, Decimal availableBalance, Decimal balance, String failReason)
    {
        if (success)
        {
            bpi.OkButtonClicked();
        }
        else
        {
            bpi.ShowProcessComplete("Attempt to fetch account balance failed!", failReason);
        }
    }

    protected void OnInitiatePasswordReset(bool success, String email, String failReason)
    {
        if (success)
        {
            bpi.ShowProcessComplete("Successfully initiated password reset",
                "An email containing a reset token has been sent to " + email + ". Use the token to change your password");

            navBar.SlideInResetPasswordPage();

            Debug.Log("An email containing a reset token has been sent to " + email + ". Use the token to change your password");
        }
        else
        {
            bpi.ShowProcessComplete("Attempt to initiate password reset failed!", failReason);
            Debug.Log("Failed to initiate password reset - " + failReason);
        }
    }

    protected void OnLogOut(bool success, String failReason)
    {
        bpi.OkButtonClicked();
        // No need for checking for successful logout from server as we can force this by clearing _authId
        _email = "";
        _authId = "";

        DataModel.AuthId = "";
        DataModel.Ledger = new LedgerReader[0];
        // trigger closing of app functionality and reopen the main login page here

        // update the login status and hide the change password button
        navBar.UpdateSettingsPageStatus("Not logged in", Color.red);
        navBar.HideChangePasswordButton();

        navBar.SlideOutFooter();
        navBar.SlideInLogInPage();
    }

    #endregion

}
