using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorRange : MonoBehaviour
{
    GameObject player;

    [Tooltip("Assign the Icon's Canvas here")]
    [SerializeField] GameObject icon;

    [Tooltip("Is the Icon going to flash?")]
    [SerializeField] bool flashing = false;

    [Tooltip("How fast should the icon flash if enabled? Higher number equals slower flash")]
    [SerializeField] float interval = 0.5f;

    float timer;
    bool onOff;
    bool detection;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        transform.LookAt(player.transform);

        timer += Time.deltaTime;
        if(timer >= interval)
        {
            timer = 0;
            onOff = !onOff;
        }

        if (detection)
        {
            //Debug.Log("Detected Player");
            if (flashing)
            {
                if (onOff)
                {
                    icon.SetActive(true);
                }
                else
                {
                    icon.SetActive(false);
                }
            }
            else
            {
                icon.SetActive(true);
            }
        }
        else
        {
            icon.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            detection = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            detection = false;
        }
    }
}
