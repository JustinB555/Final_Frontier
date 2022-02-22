using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reload_UI : MonoBehaviour
{
    [SerializeField] GameObject reloadWhole = null;
    [SerializeField] Image cover;

    public float totalReload;

    public bool startReload = false;

    float timeAdjustment = 6.5f;
    void Start()
    {
        
    }

    void Update()
    {
        if (startReload)
        {
            reloadWhole.SetActive(true);
            if(Time.timeScale == 1)
            {
                cover.fillAmount -= totalReload * Time.deltaTime;
            }
            else
            {
                cover.fillAmount -= (totalReload * Time.deltaTime) * timeAdjustment;
            }
            if (cover.fillAmount <= 0)
            {
                startReload = false;
                cover.fillAmount = 1;
                reloadWhole.SetActive(false);
            }
        }
    }
}
