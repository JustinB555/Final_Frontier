
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtTimer : MonoBehaviour
{
    //public Text timerText;
    public TextMesh timerText;
    public float time;
    float msec;
    float sec;
    float min;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator BtClock()
    {
        while (true)
        {
            time += Time.deltaTime;
            msec = (int)((time - (int)time) * 100);
            sec = (int)(time % 60);

            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", min, sec, msec);

            yield return null;
        }
    }

    public void StartTimer()
    {
        StartCoroutine("BtClock");
    }

    public void StopTimer()
    {
        StopCoroutine("BtClock");
    }
}
