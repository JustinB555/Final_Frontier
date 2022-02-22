using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UITutorial : MonoBehaviour
{

    [SerializeField] public GameObject WeaponUI;
    [SerializeField] public GameObject GrenadeUI;
    [SerializeField] public GameObject PainkillerUI;
    [SerializeField] public GameObject BulletTimePickupUI;
    [SerializeField] public GameObject HealthUI;
    [SerializeField] public GameObject BulletTimeUI;
    [SerializeField] public GameObject AOEAbilityUI;
    [SerializeField] public GameObject ShieldAbilityUI;

    MovementThirdPerson m3p = null;

    int currScene = 0;

    // Start is called before the first frame update
    void Start()
    {
        m3p = FindObjectOfType<MovementThirdPerson>();

        WeaponUI.SetActive(false);
        GrenadeUI.SetActive(false);
        PainkillerUI.SetActive(false);
        BulletTimePickupUI.SetActive(false);
        HealthUI.SetActive(false);
        BulletTimeUI.SetActive(false);
        AOEAbilityUI.SetActive(false);
        ShieldAbilityUI.SetActive(false);

        currScene = SceneManager.GetActiveScene().buildIndex;

        if( currScene == 3)
        {
            m3p.isNotTutorialBT = false;
            m3p.isNotTutorialCRS = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
