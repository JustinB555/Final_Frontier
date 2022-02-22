using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_Target : MonoBehaviour
{
    enum Points : int { one = 1, two = 2, three = 3, four = 4, five = 5, NONE = 0 }

    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public
    [Space(5)]
    [Header("Points Gained")]
    [Tooltip("How many points do you gain if you hit this part?")]
    [SerializeField] Points points = Points.NONE;
    [Tooltip("This bool allows you to shoot it with or without Bullet Dodge.")]
    [SerializeField] bool needsBD = true;

    [Space(5)]
    [Header("References")]
    [Tooltip("This is the broken target.")]
    [SerializeField] GameObject broken = null;
    [Tooltip("This is the good target.")]
    [SerializeField] GameObject good = null;
    [Tooltip("This is the parent")]
    [SerializeField] JButler_Target parent = null;
    [Tooltip("Put our scoreboard for this test here.")]
    public JButler_TargetScore scoreboard = null;

    [Space(5)]
    [Header("Who's Hit")]
    [Tooltip("This is a visual representation of which point was hit.")]
    public bool point1 = false;
    public bool point2 = false;
    public bool point3 = false;
    public bool point4 = false;
    public bool point5 = false;
    [Tooltip("Only check once.")]
    public bool checkedHit = false;

    [Space(5)]
    [Header("Clean Up")]
    [Tooltip("Time to despawn after destroyed.\n USE ONLY WITH PARENT TARGET!")]
    [SerializeField] float despawn = 2.0f;

    // private
    float timer = 0.0f;
    Sound_Manager sm = null;

    // Start is called before the first frame update
    void Start()
    {
        sm = FindObjectOfType<Sound_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        Cleanup();
    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public int PointsGained()
    {
        int value = (int)points;

        return value;
    }

    public void Break()
    {
        good.SetActive(false);
        broken.SetActive(true);
    }

    public bool NeedsBD()
    {
        if (parent.needsBD)
            return true;
        else
            return false;
    }

    public void WhosHit()
    {
        switch (points)
        {
            case Points.one:
                parent.point1 = true;
                sm.Play("TargetHit");
                break;

            case Points.two:
                parent.point2 = true;
                sm.Play("TargetHit");
                break;

            case Points.three:
                parent.point3 = true;
                sm.Play("TargetHit");
                break;

            case Points.four:
                parent.point4 = true;
                sm.Play("TargetHit");
                break;

            case Points.five:
                parent.point5 = true;
                sm.Play("BullseyeAgainAgain");
                break;

            default:
                parent.point1 = false;
                parent.point2 = false;
                parent.point3 = false;
                parent.point4 = false;
                parent.point5 = false;
                break;
        }
    }

    public int PointsValue()
    {
        if (point1)
            return 1;
        else if (point2)
            return 2;
        else if (point3)
            return 3;
        else if (point4)
            return 4;
        else if (point5)
            return 5;
        else
            return 0;
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void Cleanup()
    {
        if (broken.activeInHierarchy)
        {
            timer += Time.deltaTime;
            if (timer >= despawn)
                broken.SetActive(false);
        }
    }
}
