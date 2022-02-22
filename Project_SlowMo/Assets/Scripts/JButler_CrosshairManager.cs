using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JButler_CrosshairManager : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    [Space(5)]
    [Header("Crosshairs")]
    [Tooltip("Put the default crosshair here.")]
    [SerializeField] JButler_Crosshair defaultCrosshair;
    [Tooltip("Put the hit crosshair here.")]
    [SerializeField] JButler_CrosshairHit hitCrosshair;

    [Space(5)]
    [Header("Numbers")]
    [Tooltip("This is to show how much damage you are doing.")]
    [SerializeField] Text damageNum;

    bool toggle;
    Canvas ourCanvas;
    JButler_ChangeCamera cams = null;

    // Start is called before the first frame update
    void Start()
    {
        ourCanvas = GetComponent<Canvas>();
        cams = FindObjectOfType<JButler_ChangeCamera>();

        CheckRenderCamera();
        CheckFields();
    }

    // Update is called once per frame
    void Update()
    {
        HideCrosshair();
    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public JButler_Crosshair Crosshair()
    {
        return defaultCrosshair;
    }

    public void ActiveDeactivate()
    {
        toggle = !toggle;

        if (toggle)
        {
            defaultCrosshair.NotActive();
            hitCrosshair.Active();
        }
        else
        {
            defaultCrosshair.Active();
            hitCrosshair.NotActive();
        }
    }

    public void ShowDamage(int amount)
    {
        damageNum.gameObject.SetActive(true);
        damageNum.text = amount.ToString();
        Invoke("MuteNums", 0.2f);
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void MuteNums()
    {
        damageNum.gameObject.SetActive(false);
    }

    void CheckRenderCamera()
    {
        if (ourCanvas.worldCamera == null)
            throw new System.Exception(name + " is <b><color=red>missing its Render Camera</color></b>!\tMake sure to set the Main Camera as the Render Camera.");
    }

    void HideCrosshair()
    {
        if (cams.ActiveNeutral())
        {
            defaultCrosshair.ColorWhite();
            defaultCrosshair.NotActive();
        }
        else
            defaultCrosshair.Active();
    }

    void CheckFields()
    {
        if (defaultCrosshair == null)
            throw new System.Exception(name + "'s field <b><color=red>Default Crosshair</color></b> is null!\t\tDid you forget to add the image to Default Crosshair?");

        if (hitCrosshair == null)
            throw new System.Exception(name + "'s field <b><color=red>Hit Crosshair</color></b> is null!\t\tDid you forget to add the image to Hit Crosshair?");

        if (damageNum == null)
            throw new System.Exception(name + "'s field <b><color=red>Damage Num</color></b> is null!\t\tDid you forget to add the text Damage Num?");

        if (cams == null)
            throw new System.Exception(name + " <b><color=red>could not find JButler_ChangeCamera</color></b>!\tMake sure that there is an active one in the scene at all times!");
    }
}
