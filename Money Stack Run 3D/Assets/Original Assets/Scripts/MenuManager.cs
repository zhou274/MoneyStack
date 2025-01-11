using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;


public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    //[SerializeField] private int immuneLevel;
    [SerializeField] private int valueLevel;
    [SerializeField] private int incomeLevel;

    //public Text immuneLevelText;
    public TextMeshProUGUI valueLevelText;
    public TextMeshProUGUI incomeLevelText;

    public string clickid;
    private StarkAdManager starkAdManager;


    [SerializeField] private List<int> immunePricePerLevel;
    [SerializeField] private List<int> valuePricePerLevel;
    [SerializeField] private List<int> incomePricePerLevel;

    //public Text immunePriceText;
    public TextMeshProUGUI valuePriceText;
    public TextMeshProUGUI incomePriceText;

    //public List<float> immunePerLevel;
    public List<float> valuePerLevel;
    public List<int> incomePerLevel;

    public int multiplierValue;

    public float incomePercentage;
    public float moneyStackMod;

    [SerializeField] private GameObject setting;
    private Animator settingAnim;
    public GameObject loseScreen;
    private void Awake()
    {
        instance = this;

        valueLevel = PlayerPrefs.GetInt("Value_Level", 1);
        incomeLevel = PlayerPrefs.GetInt("Income_Level", 1);

        moneyStackMod = valuePerLevel[valueLevel - 1];
        incomePercentage = incomePerLevel[incomeLevel - 1]; 
    }

    private void Start()
    {
        settingAnim = setting.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        valueLevelText.text = string.Format("等级 " + "{0:0}", valueLevel);
        incomeLevelText.text = string.Format("等级 " + "{0:0}", incomeLevel);

        valuePriceText.text = string.Format("{0:0}", valuePricePerLevel[valueLevel]);
        incomePriceText.text = string.Format("{0:0}", incomePricePerLevel[incomeLevel]);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BuyValueUpgrade()
    {
        if (GameManager.totalGemAmount >= valuePricePerLevel[valueLevel])
        {
            GameManager.totalGemAmount -= valuePricePerLevel[valueLevel];
            moneyStackMod = valuePerLevel[valueLevel];
            valueLevel++;
            PlayerPrefs.SetInt("Value_Level", valueLevel);
        }
    }

    public void BuyIncomeUpgrade()
    {
        if (GameManager.totalGemAmount >= incomePricePerLevel[incomeLevel])
        {
            GameManager.totalGemAmount -= incomePricePerLevel[incomeLevel];
            incomePercentage = incomePerLevel[incomeLevel];
            incomeLevel++;
            PlayerPrefs.SetInt("Income_Level", incomeLevel);
        }
    }

    public void ContinueGame()
    {
        ShowVideoAd("dih3de72fig3d169m2",
            (bol) => {
                if (bol)
                {

                    GameManager.instance.levelNo++;
                    GameManager.totalGemAmount += GameManager.instance.currentGemCollected * multiplierValue;
                    if (GameManager.instance.levelNo > GameManager.instance.dataLevels.Count - 1)
                    {
                        GameManager.instance.levelNo = 0;
                    }
                    PlayerPrefs.SetInt("Level_Number", GameManager.instance.levelNo);
                    PlayerPrefs.SetFloat("Total_Gem", GameManager.totalGemAmount);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);



                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
        //AdmobManager.instance.ShowAdReward(() =>
        //{
        //    //Xem ads complete
        //    GameManager.instance.levelNo++;
        //    GameManager.totalGemAmount += GameManager.instance.currentGemCollected * multiplierValue;
        //    if (GameManager.instance.levelNo > GameManager.instance.dataLevels.Count - 1)
        //    {
        //        GameManager.instance.levelNo = 0;
        //    }
        //    PlayerPrefs.SetInt("Level_Number", GameManager.instance.levelNo);
        //    PlayerPrefs.SetFloat("Total_Gem", GameManager.totalGemAmount);
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}, () =>
        //{
        //    GameManager.instance.levelNo++;
        //    GameManager.totalGemAmount += GameManager.instance.currentGemCollected;
        //    if (GameManager.instance.levelNo > GameManager.instance.dataLevels.Count - 1)
        //    {
        //        GameManager.instance.levelNo = 0;
        //    }
        //    PlayerPrefs.SetInt("Level_Number", GameManager.instance.levelNo);
        //    PlayerPrefs.SetFloat("Total_Gem", GameManager.totalGemAmount);
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //});
    }

    public void ReviveButton()
    {
        ShowVideoAd("dih3de72fig3d169m2",
            (bol) => {
                if (bol)
                {
                    loseScreen.SetActive(false);
                    ObstacleHit.Respawned();

                    //GameManager.totalGemAmount += GameManager.instance.currentGemCollected;
                    //PlayerPrefs.SetFloat("Total_Gem", GameManager.totalGemAmount);
                    //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    //SceneManager.LoadScene(SceneManager.GetActiveScene().name);


                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
        
        
    }

    public void SkipButton()
    {
        ShowVideoAd("dih3de72fig3d169m2",
            (bol) => {
                if (bol)
                {
                    GameManager.instance.levelNo++;
                    if (GameManager.instance.levelNo > GameManager.instance.dataLevels.Count - 1)
                    {
                        GameManager.instance.levelNo = 0;
                    }
                    PlayerPrefs.SetInt("Level_Number", GameManager.instance.levelNo);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);



                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
    }

    public void ContinueWithoutMultiplierButton()
    {
        //AdmobManager.instance.ShowAdInter();
        GameManager.instance.levelNo++;
        GameManager.totalGemAmount += GameManager.instance.currentGemCollected * multiplierValue;
        if (GameManager.instance.levelNo > GameManager.instance.dataLevels.Count - 1)
        {
            GameManager.instance.levelNo = 0;
        }
        PlayerPrefs.SetInt("Level_Number", GameManager.instance.levelNo);
        PlayerPrefs.SetFloat("Total_Gem", GameManager.totalGemAmount);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void settingButton()
    {
        if (settingAnim.GetBool("isOpen") == false)
        {
            settingAnim.SetBool("isOpen", true);
        }
        else
        {
            settingAnim.SetBool("isOpen", false);
        }
    }
    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
}
