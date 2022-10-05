using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ZeraAPIHandler : MonoBehaviour
{
	static ZeraAPIHandler instance = null;

	public static ZeraAPIHandler Inst
    {
        get
        {
			if(instance == null)
            {
				instance = FindObjectOfType<ZeraAPIHandler>();

				if(instance == null)
                {
					instance = new GameObject("ZeraAPIHandler").AddComponent<ZeraAPIHandler>();
                }
            }
			DontDestroyOnLoad(instance);
			return instance;
        }
    }

	[Header("[Our Project API-KEY]")]
    [SerializeField] string API_KEY = "";

    [Header("[Betting Backend Base URL")]
    [SerializeField] string FullAppsStagingURL = "https://odin-api-sat.browseosiris.com";

	// BaseURL - Staging
	string getBaseURL()
	{
		return FullAppsStagingURL;
	}

	public Res_GetUserProfile resGetUserProfile = null;
	public Res_GetSessionID	  resGetSessionID   = null;
	public Res_BalanceInfo    resBalanceInfo    = null;
	public Res_Settings       resSettings       = null;

	public string selectedBettingID = null;

	public bool getUserProfile = false;
	public bool getSessionID = false;

	//-----------------------------------------------------------------------------------------------------
	//
	// Get UserInfo
	public void GetUserProfile()
	{
		StartCoroutine(processRequestGetUserInfo());
	}

	IEnumerator processRequestGetUserInfo()
	{
		// UserInfo
		yield return requestGetUserInfo((response) =>
		{
			if (response != null)
			{
				resGetUserProfile = response;
				getUserProfile = true;
				Debug.Log("유저 정보 받아옴!");
			}
			else
				getUserProfile = false;

		});
	}

	// Get Session ID
	public void GetSessionID()
	{
		StartCoroutine(processRequestGetSessionID());
	}

	IEnumerator processRequestGetSessionID()
	{
		// UserInfo
		yield return requestGetSessionID((response) =>
		{
			if (response != null)
			{
				resGetSessionID = response;
				Debug.Log("세션 아이디 받아옴!");
				getSessionID = true;
            }
            else
            {
				getSessionID = false;
            }
		});
	}

	// GetMyZeraBalance
	public void GetMyZeraBalance()
	{
		StartCoroutine(processRequestZeraBalance());
	}

	IEnumerator processRequestZeraBalance()
	{
		yield return requestZeraBalance(resGetSessionID.sessionId, (response) =>
		{
			if (response != null)
			{
				resBalanceInfo = response;
				Debug.Log("내 제라 정보 받아옴!");
			}
		});
	}

	// Get Betting Setting Information
	public void GetBettingSettings()
	{
		StartCoroutine(processRequestSettings());
	}
	IEnumerator processRequestSettings()
	{
		yield return requestSettings((response) =>
		{
			if (response != null)
			{
				resSettings = response;
				Debug.Log("베팅 세팅 정보 받아옴!");
			}
		});
	}

    // ZERA Betting
    public void BettingZera()
    {
        StartCoroutine(processRequestBetting_Zera());
    }
    IEnumerator processRequestBetting_Zera()
    {
        ResBettingPlaceBet resBettingPlaceBet = null;
        ReqBettingPlaceBet reqBettingPlaceBet = new ReqBettingPlaceBet();
        reqBettingPlaceBet.players_session_id = new string[] { resGetSessionID.sessionId };
        reqBettingPlaceBet.bet_id = selectedBettingID;// resSettings.data.bets[0]._id;
        yield return requestCoinPlaceBet(reqBettingPlaceBet, (response) =>
        {
            if (response != null)
            {
                resBettingPlaceBet = response;
				Debug.Log("베팅 완료!");
            }
        });
    }


    //-----------------------------------------------------------------------------------------------------



    /// To get user’s information.This is also used to authenticate if session-id is valid or not.
    /// This can determine if the Odin is currently running or not. 
    ///	If Odin is not running, the API  is not accesible as well.
    ///	Inform the User to run the Osiris and Connect to Odin via Meta wallet.
    delegate void resCallback_GetUserInfo(Res_GetUserProfile response);
	IEnumerator requestGetUserInfo(resCallback_GetUserInfo callback)
	{
		// get user profile
		UnityWebRequest www = UnityWebRequest.Get("http://localhost:8546/api/getuserprofile");
		yield return www.SendWebRequest();
		Res_GetUserProfile res_getUserProfile = JsonUtility.FromJson<Res_GetUserProfile>(www.downloadHandler.text);
		callback(res_getUserProfile);
	}


	// Request User's Session ID
	delegate void resCallback_GetSessionID(Res_GetSessionID response);
	IEnumerator requestGetSessionID(resCallback_GetSessionID callback)
	{
		// get session id
		UnityWebRequest www = UnityWebRequest.Get("http://localhost:8546/api/getsessionid");
		yield return www.SendWebRequest();
		Res_GetSessionID res_getSessionID = JsonUtility.FromJson<Res_GetSessionID>(www.downloadHandler.text);
		callback(res_getSessionID);
	}

	// GetMyZera from API
	delegate void resCallback_BalanceInfo(Res_BalanceInfo response);
	IEnumerator requestZeraBalance(string sessionID, resCallback_BalanceInfo callback)
	{
		string url = getBaseURL() + ("/v1/betting/" + "zera" + "/balance/" + sessionID);

		UnityWebRequest www = UnityWebRequest.Get(url);
		www.SetRequestHeader("api-key", API_KEY);
		yield return www.SendWebRequest();
		Res_BalanceInfo res = JsonUtility.FromJson<Res_BalanceInfo>(www.downloadHandler.text);
		callback(res);
	}

	// To get game’s general and bet settings
	delegate void resCallback_Settings(Res_Settings response);
	IEnumerator requestSettings(resCallback_Settings callback)
	{
		string url = getBaseURL() + "/v1/betting/settings";

		UnityWebRequest www = UnityWebRequest.Get(url);
		www.SetRequestHeader("api-key", API_KEY);
		yield return www.SendWebRequest();
		Res_Settings res = JsonUtility.FromJson<Res_Settings>(www.downloadHandler.text);
		callback(res);
	}

	// Request Method : POST 
	// Body Type : json
	delegate void resCallback_BettingPlaceBet(ResBettingPlaceBet response);
	IEnumerator requestCoinPlaceBet(ReqBettingPlaceBet req, resCallback_BettingPlaceBet callback)
	{
		string url = getBaseURL() + "/v1/betting/" + "zera" + "/place-bet";

		string reqJsonData = JsonUtility.ToJson(req);
		Debug.Log(reqJsonData);


		UnityWebRequest www = UnityWebRequest.Post(url, reqJsonData);
		byte[] buff = System.Text.Encoding.UTF8.GetBytes(reqJsonData);
		www.uploadHandler = new UploadHandlerRaw(buff);
		www.SetRequestHeader("api-key", API_KEY);
		www.SetRequestHeader("Content-Type", "application/json");
		yield return www.SendWebRequest();

		ResBettingPlaceBet res = JsonUtility.FromJson<ResBettingPlaceBet>(www.downloadHandler.text);
		callback(res);
	}
}
