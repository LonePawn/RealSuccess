using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class EnairaApiConnector : MonoBehaviour
{
    private static String eNairaApiServerUrl = "https://rgw.k8s.apis.ng/centric-platforms/uat/";
    private static String clientId = "96072d08827964d5f310d3b53b929906";


    private static String simulatedEnairaApiServerUrl = "http://localhost/RealSuccess/SmartKobo/";



    private HttpClient httpClient;

    //private String _email;          // This will hold the authorised user's email. This App is limited to 1 concurrent user email per device.
                                    // Meaning 2 or more people can not simultaneously login on the same device at the same time.

    public SK_NavBar navBar;

    public BackgroundProcess backgroundProcessIndicator;
    private static BackgroundProcess bpi;

    // Start is called before the first frame update
    void Start()
    {
        bpi = backgroundProcessIndicator;
        navBar = GameObject.FindGameObjectWithTag("NavBar").GetComponent<SK_NavBar>();
        httpClient = new HttpClient();
    }

    
    public void GetEnairaBalance(string email)
	{
        Debug.Log("GetEnairaBalance called");
        if (email.Equals("realsuccess@fakemail.com"))
        {
            Debug.Log("email match");
            //Task.Run(async () =>
            //{
                bpi.ShowRunningProcess("Fetching Balance");
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, eNairaApiServerUrl + "GetBalance");
            request.Headers.Add("ClientId", clientId);
            //request.Headers.Add("accept", "application/json");
                HttpContent content = new StringContent(
                    "{'user_email': 'test_user+access@bitt.com', 'user_token': 'eyJhbGciOiJSUI1NiIsInR5cCI6IkpXVCJ9.eyJhbGlhcyI6IkB0YWNoLjAxZiIsImV4cGlyeSI6IjIwMjEtMDktMjdUMTM6MjU6NDMuMDg2NDMxWiIsImlzcyI6ImdvbmRvciBjb21tZXJjZSIsIm9yZyI6ImNmYiIsInRva2VuX3R5cGUiOiJhY2Nlc3MiLCJ1c2VyaWQiOiIwMUZFUERXS1BHVjkxVzRCRE45MTczTjBWRyJ9.zMIxdoqkxY3MFnY7zRUeQdA88SrXD-KcukT2fePc3a4IMhtuAuBcNjSnVQ9J4AUNAz2BQGnhvjsJiOJGcV4M6w1n1tFJMr6xnDTChb2OZa2eSi6u3qjppOKXgQ_t0EOPpTC9Iqx3zgRt0C6Nl1z14ixmGmaAY0SKKgjHSI1ieiuRtzJuJi7qq7nHf_u4iypr4mMN1H6KCrIq86xEPp2bN2H3cEHQr2AaSjLamoPkT_oA0RHoNroZvTqmpZE80hvYHBSECUagiAazlb_ANMJgNF0zo_uSSMkyXHpASwqdaZnPLgINzLkIJrfNLwDf4P1VEh9VoaB1E9ElQanZVBrN51VDhBitTzGolSxk0_P37aVPrS9yeWceJTHs1GojMOGCosYRu_wi05n0bsG_iAEFRFxlV3pT-2YdS5YAbUdEJj65NO-6SkmPQBcMGCwHCztDn6h6lQZiUPgjnTqsSUq_kUC_X3ki4I7fVohm-Z9jplLOPHHk88CmlJnUI0AgYYfJQw8Kwi5010-1eTz6EFVI7_fjeV_OPI-emxJ4F1QaUy934XC69kWPDgfeLtBfYmee3M3N6MaD6eurltD23YknDhg-VWcBD_BycmUKMIQ5hXHL-0ix_e97zr1kw7UjcwdNwlCC6m9AkrEhevzvG4nQ3ZIVRhb_fklXykYITjulUmQ', 'user_type': 'USER', 'channel_code': 'APISNG'}",
                    System.Text.Encoding.UTF8,
                    "application/json");
                content.Headers.Add("content-type", "application/json");
                request.Content = content;
                Debug.Log("We are here");
            Task<HttpResponseMessage> response = httpClient.SendAsync(request);
            Task<string> jsonResponse = response.Result.Content.ReadAsStringAsync();
            //HttpResponseMessage response = httpClient.SendAsync(request);
                Debug.Log("We are back from eNaira API server");
                //string jsonResponse = await response.Content.ReadAsStringAsync();
                Debug.Log($"http response is: {jsonResponse.Result}");
                bpi.ShowProcessComplete("Done", jsonResponse.Result);
                
            //}).ConfigureAwait(false);
        }
        else
		{
            // fetch the balance from our simulated Enaira database
		}
    }

    public void GetSimulatedEnairaBalance(string email)
	{
        bpi.ShowRunningProcess("Fetching Balance");
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, simulatedEnairaApiServerUrl + "GetBalance");
        var data = new List<KeyValuePair<string, string>>();
        data.Add(new KeyValuePair<string, string>("email", email));
        var content = new FormUrlEncodedContent(data);
        var response = httpClient.SendAsync(request);
        var jsonResponse = response.Result.Content.ReadAsStringAsync();
        Debug.Log($"http response is: {jsonResponse.Result}");
        bpi.ShowProcessComplete("Done", jsonResponse.Result);
    }
    

    /*
    public IEnumerator GetEnairaBalance(string email)
    {
        Debug.Log("GetEnaira Balance called");
        if (email.Equals("realsuccess@fakemail.com"))
        {
            bpi.ShowRunningProcess("Fetching Balance");
            UnityWebRequest webRequest = new UnityWebRequest(eNairaApiServerUrl + "GetBalance", "post");
            webRequest.SetRequestHeader("ClientId", clientId);
            webRequest.SetRequestHeader("content-type", "application/json");
            webRequest.SetRequestHeader("accept", "application/json");
            string jsonData = "{user_email: 'test_user+access@bitt.com', user_type: 'USER', channel_code: 'APISNG'}";
            webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            
            Debug.Log("We are here");
            var response = webRequest.SendWebRequest();
            while (!response.isDone)
            {
                yield return response;
            }
            Debug.Log("We are back from eNaira API server");
            string jsonResponse = response.webRequest.downloadHandler.text;

            Debug.Log($"http response is: {jsonResponse}");
            bpi.ShowProcessComplete("Done", jsonResponse);
        }
        else
        {
            // fetch the balance from our simulated Enaira database
        }
    }*/

	

	public void OnFetchedEnairaBalance(bool success, float amount, string failReason)
	{
        if(success)
		{

		}
	}

}
