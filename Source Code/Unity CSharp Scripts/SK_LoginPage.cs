using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SK_LoginPage : MonoBehaviour
{
    public SK_ApiConnector skApiConnector;
    public InputField inputEmail;
    public InputField inputPwd;
    public InfoBox infoBox;


    //public SK_NavBar navBar;
    //public GameObject signUpPage;

    public void LogInButtonPressed()
    {
        if (inputEmail.text != "" && inputPwd.text != "")
        {
            skApiConnector.StartCoroutine(skApiConnector.AuthenticateUser(inputEmail.text, inputPwd.text));

            // clear the input fields
            inputEmail.text = "";
            inputPwd.text = "";
        }
        else
        {
            infoBox.Info("Error!", "Email / Password cannot be empty");
        }
    }

    public void ForgottenPassword()
    {
        if (inputEmail.text == "")
        {
            infoBox.Info("Error! Email Unspecified", "Enter the account email whose associated password you forgot");
        }
        else
        {
            // initiate change password procedure here
            skApiConnector.StartCoroutine(skApiConnector.InitiatePasswordReset(inputEmail.text));
        }
    }

    public void SignUp()
    {
      
    }

    public void LogOut()
    {
        skApiConnector.StartCoroutine(skApiConnector.LogOut(skApiConnector.GetAuthId()));
    }
}
