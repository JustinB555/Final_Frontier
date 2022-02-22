using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableTutorial : MonoBehaviour
{
    [SerializeField] GameObject TutorialText;
    [SerializeField] GameObject GrenadeMarker;
    [SerializeField] GameObject DestructableMarker;
    [SerializeField] GameObject Destructable;
    [SerializeField] GameObject Tutorial;
    [SerializeField] GameObject GrenadeTutorial;

    Player_Values pv;
    UITutorial uit = null;




    // Start is called before the first frame update
    void Start()
    {
        pv = GameObject.FindObjectOfType<Player_Values>();
        uit = GameObject.FindObjectOfType<UITutorial>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.grenadeCount > 0)
        {
            uit.GrenadeUI.SetActive(true);
            GrenadeMarker.SetActive(false);
            DestructableMarker.SetActive(true);
        }


        if (Destructable == null)
        {
            DestructableMarker.SetActive(false);
            Destroy(GrenadeTutorial);
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        TutorialText.SetActive(true);
        Invoke("GrenadeText", 4.0f);
    }


    void GrenadeText()
    {
        TutorialText.SetActive(false);
    }

}
