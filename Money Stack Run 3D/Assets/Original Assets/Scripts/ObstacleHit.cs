using DG.Tweening;
using Dreamteck.Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using StarkSDKSpace;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;

public class ObstacleHit : MonoBehaviour
{
    [SerializeField] private GameObject invisibleTrack;
    [SerializeField] private GameObject playerBlock;
    [SerializeField] private GameObject player;

    [SerializeField] private float knockBackDistance;

    [SerializeField] private GameObject stackPos;
    private float stackCheckpoint = 10400f;

    private Animator characterAnim;

    [SerializeField] private GameObject moneyIndicator;
    [SerializeField] private GameObject moneyIndicatorPos;
    public static Action Respawned;
    private StarkAdManager starkAdManager;

    public string clickid;

    private void Awake()
    {
        Respawned += Respawn;
    }
    private void OnDestroy()
    {
        Respawned -= Respawn;
    }
    void Start()
    {
        characterAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        for (int i = stackPos.GetComponent<StackPosController>().moneyStack.Length - 1; i >= 0; i--)
        {
            if (gameObject.GetComponent<PlayerPowerController>().moneyAmount <= 0)
            {
                stackPos.GetComponent<StackPosController>().moneyStack[0].gameObject.SetActive(false);
            }
            if (gameObject.GetComponent<PlayerPowerController>().moneyAmount < stackCheckpoint)
            {
                stackPos.GetComponent<StackPosController>().moneyStack[i].gameObject.SetActive(false);
                stackCheckpoint -= 100f;
            }
            else
            {
                stackCheckpoint = 10400f;
                return;
            }
        }
    }

    //When hit an obstacle
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacles"))
        {
            //minus some Money on Player
            gameObject.GetComponent<PlayerPowerController>().moneyAmount -= collision.gameObject.GetComponent<ObstacleValue>().obstacleValue;

            invisibleTrack.GetComponent<SplineFollower>().follow = false;
            player.GetComponent<control>().enabled = false;

            GameObject newIndicator = Instantiate(moneyIndicator, moneyIndicatorPos.transform.position, Quaternion.identity);
            newIndicator.GetComponent<MoneyIndicatorValue>().impactValue.text = string.Format("-" + "{0:0}", collision.gameObject.GetComponent<ObstacleValue>().obstacleValue);
            newIndicator.transform.SetParent(playerBlock.transform);
            StartCoroutine("DelayIndicatorDisable", newIndicator);

            //if player have no money left ... else...
            if (gameObject.GetComponent<PlayerPowerController>().moneyAmount < 0)
            {
                gameObject.GetComponent<PlayerPowerController>().moneyAmount += 2;
                Destroy(collision.gameObject);
                characterAnim.SetTrigger("Dead");
                gameObject.GetComponent<Collider>().enabled = false;
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
                PlayerBlockMovement.instance.playerBlockSpeed = 0;
                ShowInterstitialAd("e4ikn6jn985kbb05k4",
            () => {
                Debug.LogError("--插屏广告完成--");

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });
            }
            else
            {
                characterAnim.SetBool("Hit", true);
                playerBlock.transform.DOMoveX(transform.position.x + knockBackDistance, 1f);
                StartCoroutine("PlayerMovingToPosition");
            }
        }
    }
    public void Respawn()
    {
        characterAnim.SetTrigger("isContinue");
        gameObject.GetComponent<Collider>().enabled = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        PlayerBlockMovement.instance.playerBlockSpeed = 2.5f;
        GameManager.instance.gameOver = false;
        player.GetComponent<control>().enabled = true;
        StartCoroutine("PlayerMovingToPosition");
        gameObject.GetComponent<PlayerPowerController>().moneyAmount = 2;
        GameManager.instance.startLosing = true;
        Time.timeScale = 1;
    }
    IEnumerator PlayerMovingToPosition()
    {
        yield return new WaitForSeconds(1.2f);
        characterAnim.SetBool("Hit", false);
        PlayerBlockMovement.instance.playerBlockSpeed = 2.5f;
        player.GetComponent<control>().enabled = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    IEnumerator DelayIndicatorDisable(GameObject newIndicator)
    {
        yield return new WaitForSeconds(0.36f);
        Destroy(newIndicator);
    }
    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }
}
