using UnityEngine;

public class UpdateTimes : MonoBehaviour
{
    private double lastTime = 0f;
    private int numUpdates = 0;
    private int numFixedUpdates = 0;
    private void Start()
    {
        //No need to have this printing from the start
        //ActivateDebugToggle();
    }
    private void Update()
    {
        ++numUpdates;
        //****************************************
        //You can remove this, and just make a call to the method inside the if check from a debug menu
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            ActivateDebugToggle();
        }
        //******************************************
    }
    private void FixedUpdate()
    {
        ++numFixedUpdates;
    }
    private void PrintInfo()
    {
        double timeSinceLastCall = Time.time - lastTime;
        Debug.LogError("Updates " + numUpdates / timeSinceLastCall + " Fixed " + numFixedUpdates / timeSinceLastCall);
        numUpdates = numFixedUpdates = 0;
        lastTime = Time.time;
    }
    public void ActivateDebugToggle()
    {
        if (!IsInvoking("PrintInfo"))
        {
            lastTime = Time.time;
            numUpdates = numFixedUpdates = 0;
            InvokeRepeating("PrintInfo", 1f, 1f);
        }
        else
        {
            CancelInvoke("PrintInfo");
        }
    }
}

