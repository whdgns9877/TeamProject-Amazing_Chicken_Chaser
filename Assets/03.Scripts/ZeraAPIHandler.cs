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
	public Res_GetSessionID resGetSessionID = null;
	public Res_BalanceInfo resBalanceInfo = null;
	public Res_Settings resSettings = null;

	public bool ConnectOdin = false;

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
				ConnectOdin = true;
			}
			else
				ConnectOdin = false;

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
			}
		});
	}

	//-----------------------------------------------------------------------------------------------------



	/// To get user¡¯s information.This is also used to authenticate if session-id is valid or not.
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
}
