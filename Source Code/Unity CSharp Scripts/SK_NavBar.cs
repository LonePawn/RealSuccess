using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SK_NavBar : MonoBehaviour
{
    [SerializeField]
    private GameObject overviewPage;

    public Text balanceDisplay;

    [SerializeField]
    private GameObject transactionsPage;

    [SerializeField]
    private GameObject topUpPage;

    [SerializeField]
    private GameObject cashOutPage;

    [SerializeField]
    private SK_LoginPage logInPage;

    [SerializeField]
    private GearBehaviour settingsGear;

    [SerializeField]
    private SettingsPage settingsPage;

    [SerializeField]
    private GameObject signUpPage;

    [SerializeField]
    private ChangePasswordPage changePasswordPage;

    [SerializeField]
    private GameObject footer;

    public MO_ScrollableClickableContainer ledgerViewer;      // This holds the container that displays the statement of account. When there is a change in ledger,
                                                              // this container should be repopulated with the new ledger entries.

    public MO_ScrollableClickableContainer awaitingActionsContainer;


    private Stack<GameObject> homeStack, transactionsStack, cashOutStack, topUpStack;
    private Stack<GameObject> activeStack;

    public EnairaApiConnector eNairaApiConnector;

    // Start is called before the first frame update
    void Start()
    {
        ResetBaseStacks();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ResetBaseStacks()
    {
        homeStack = new Stack<GameObject>();
        transactionsStack = new Stack<GameObject>();
        cashOutStack = new Stack<GameObject>();
        topUpStack = new Stack<GameObject>();

        // initiate the active stack
        activeStack = homeStack;

        // push in the base pages for the 4 main page Stacks
        homeStack.Push(overviewPage);
        transactionsStack.Push(transactionsPage);
        topUpStack.Push(topUpPage);
        cashOutStack.Push(cashOutPage);
    }

    // helper method for external use
    /// <summary>
    /// This method arranges a collection texts horizontally on a RectTransform.
    /// </summary>
    /// <param name="recordObject">The game object that holds the RectTransform UI on which to arrange the texts</param>
    /// <param name="navButtons">The navigation buttons to arrange horizontally
    public static void arrangeButtonsOnRecordPanel(GameObject recordObject, List<GameObject> navButtons)
    {
        for (int k = 0; k < navButtons.Count; k++)
        {
            GameObject recordItem = navButtons[k];
            //recordItem.GetComponentInChildren<Text>().text = itemsOnRecord[k];

            // attach the record item to its parent record object
            recordItem.transform.SetParent(recordObject.transform);

            // keep old measurements
            float itemMinAnchorY = recordItem.GetComponent<RectTransform>().anchorMin.y;
            float itemMaxAnchorY = recordItem.GetComponent<RectTransform>().anchorMax.y;

            // calculate the x-axis anchor positions
            float itemMinAnchorX = (float)k / navButtons.Count;
            float itemMaxAnchorX = itemMinAnchorX + (1.0f / navButtons.Count);

            //adjust the width of each record item
            recordItem.GetComponent<RectTransform>().anchorMin = new Vector2(itemMinAnchorX, itemMinAnchorY);
            recordItem.GetComponent<RectTransform>().anchorMax = new Vector2(itemMaxAnchorX, itemMaxAnchorY);
            MO_ScrollableContainer.resetUIRect(recordItem.transform);
        }
    }

    public void HomeButtonPressed()
    {
        // if the home stack is not the active stack, make it
        if (activeStack != homeStack)
        {
            // Take note of the page to park left
            GameObject vacatingPage = activeStack.Peek();

            // make the home stack the active stack
            activeStack = homeStack;

            // carry out the page transition
            vacatingPage.GetComponent<Animation>().Play("OutToLeft");
            activeStack.Peek().GetComponent<Animation>().Play("InFromRight");

            //UpdateOverviewPage();
        }
    }

    public void UpdateOverviewPage()
    {
        eNairaApiConnector.GetEnairaBalance(DataModel.Email);
    }

    public void TransactionsButtonPressed()
    {
        // if the transactions stack is not the active stack, make it
        if (activeStack != transactionsStack)
        {
            // Take note of the page to park left
            GameObject vacatingPage = activeStack.Peek();

            // make the transactions stack the active stack
            activeStack = transactionsStack;

            // carry out the page transition
            vacatingPage.GetComponent<Animation>().Play("OutToLeft");
            activeStack.Peek().GetComponent<Animation>().Play("InFromRight");
        }
    }

    public void SmartContractButtonPressed()
	{
        // if the transactions stack is not the active stack, make it
        if (activeStack != topUpStack)
        {
            // Take note of the page to park left
            GameObject vacatingPage = activeStack.Peek();

            // make the transactions stack the active stack
            activeStack = topUpStack;

            // carry out the page transition
            vacatingPage.GetComponent<Animation>().Play("OutToLeft");
            activeStack.Peek().GetComponent<Animation>().Play("InFromRight");
        }
    }

    public void CashOutButtonPressed()
    {
        // if the transactions stack is not the active stack, make it
        if (activeStack != cashOutStack)
        {
            // Take note of the page to park left
            GameObject vacatingPage = activeStack.Peek();

            // make the transactions stack the active stack
            activeStack = cashOutStack;

            // carry out the page transition
            vacatingPage.GetComponent<Animation>().Play("OutToLeft");
            activeStack.Peek().GetComponent<Animation>().Play("InFromRight");
        }
    }

    public void SlideOutLogInPage()
    {
        logInPage.GetComponent<Animation>().Play("OutToLeft");
        activeStack.Peek().GetComponent<Animation>().Play("InFromRight");
    }

    public void SlideInLogInPage()
    {
        logInPage.GetComponent<Animation>().Play("InFromLeft");
        activeStack.Peek().GetComponent<Animation>().Play("OutToRight");

        // reset the active and base stacks
        ResetBaseStacks();
    }

    public void SlideInFooter()
    {
        footer.GetComponent<Animation>().Play("InFromRight");
    }

    public void SlideOutFooter()
    {
        footer.GetComponent<Animation>().Play("OutToRight");
    }

    public void SlideInSignUpPage()
	{
        logInPage.GetComponent<Animation>().Play("OutToRight");
        signUpPage.GetComponent<Animation>().Play("InFromLeft");
	}

    public void SlideOutSignUpPage()
	{
        signUpPage.GetComponent<Animation>().Play("OutToLeft");
        logInPage.GetComponent<Animation>().Play("InFromRight");
    }

    public void BackButtonPressed()
    {
        if (activeStack.Count > 1)
        {
            activeStack.Pop().GetComponent<Animation>().Play("OutToRight");
            activeStack.Peek().GetComponent<Animation>().Play("InFromLeft");
        }
    }

    public void PushToActiveStack(GameObject thePage)
    {
        if (activeStack.Peek() != thePage)
        {
            activeStack.Peek().GetComponent<Animation>().Play("OutToLeft");
            thePage.GetComponent<Animation>().Play("InFromRight");
            activeStack.Push(thePage);
        }
    }

    public void LogInButtonPressed()
    {
        logInPage.LogInButtonPressed();
    }

    public void SignUpButtonPressed()
    {
        logInPage.SignUp();
    }

    public void ForgottenPasswordTapped()
    {
        logInPage.ForgottenPassword();
    }

    public void ChangePasswordButtonPressed()
    {
        changePasswordPage.ChangePassword();
        SlideOutChangePasswordPage();
    }

    public void SlideInChangePasswordPage()
    {
        // Roll up the settings page
        settingsGear.SpinGear();

        changePasswordPage.SetTokenLabel("Old Password:");

        // use the current page stack manager to push the change password page into view
        PushToActiveStack(changePasswordPage.gameObject);
        //changePasswordPage.gameObject.GetComponent<Animation>().Play("InFromLeft");
    }

    public void SlideInResetPasswordPage()
    {
        changePasswordPage.SetTokenLabel("Token:");

        // use the current page stack manager to push the change password page into view
        PushToActiveStack(changePasswordPage.gameObject);
        //changePasswordPage.gameObject.GetComponent<Animation>().Play("InFromLeft");
    }

    public void SlideOutChangePasswordPage()
    {
        // use the current page stack manager to pop the change password page out of view
        BackButtonPressed();
        //changePasswordPage.gameObject.GetComponent<Animation>().Play("OutToLeft");
    }

    public void UpdateSettingsPageStatus(String status, Color c)
    {
        //settingsPage.SetStatusTextColor(c);
        //settingsPage.SetLogInStatus(status);
    }

    public void LogOutButtonPressed()
    {
        logInPage.LogOut();
    }

    public void ShowChangePasswordButton()
    {
        //settingsPage.ShowChangePasswordButton();
    }

    public void HideChangePasswordButton()
    {
        settingsPage.HideChangePasswordButton();
    }

    public void UpdatePaymentsAwaitingActionPage()
    {
        transactionsPage.GetComponent<TransactionsPageController>().preparePaymentsAwaitingActionPage();
    }

}
