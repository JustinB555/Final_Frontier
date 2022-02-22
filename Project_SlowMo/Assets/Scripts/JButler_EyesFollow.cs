#define RAY

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_EyesFollow : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    [Space(5)]
    [Header("ADS_EndPoint")]
    [Tooltip("Put ADS_EndPoint here.")]
    [SerializeField] Transform endPoint = null;

    CharacterController player = null;
    Transform us = null;
    DebugScript ds = null;
    bool once = false;

    // Start is called before the first frame update
    void Start()
    {
        us = transform;
        ds = FindObjectOfType<DebugScript>();
        player = FindObjectOfType<CharacterController>();

        if (endPoint == null)
            throw new System.Exception(name + " does not know what to <color=red><b>look at</b></color>!\tMake sure to put an End Point.");

        if (ds == null)
            throw new System.Exception(name + " <b><color=red>could not find DebugScript</color></b>!\tMake sure that there is an active one in the scene at all times!");

        if (player == null)
            throw new System.Exception(name + " <b><color=red>could not find a CharacterController</color></b>!\tMake sure that there is an active one in the scene at all times!");
    }

    // Update is called once per frame
    void Update()
    {
        if (!ds.DebugActive() && Time.timeScale > 0)
        {
            FindCrosshair(us, endPoint);
            once = false;
        }
        else if (!once && Time.timeScale > 0)
        {
            ResetRotation();
            once = true;
        }
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void ResetRotation()
    {
        us.rotation = Quaternion.LookRotation(player.transform.forward);
    }

    private Vector3 RayDirection(Transform pointA, Transform pointB)
    {
        Vector3 ourPoint = pointA.position;
        Vector3 tarPoint = pointB.position;
        Vector3 aim = (tarPoint - ourPoint).normalized;
        return aim;
    }

    private void FindCrosshair(Transform pointA, Transform pointB)
    {
        Vector3 aim = RayDirection(pointA, pointB);

        float rotateY = Mathf.Atan2(aim.x, aim.z) * Mathf.Rad2Deg;
        float rotateX = Mathf.Atan(-aim.y) * Mathf.Rad2Deg;
        pointA.eulerAngles = new Vector3(rotateX, rotateY, 0f);

        #region DEBUGGING
#if RAY
        Debug.DrawLine(pointA.position, pointB.position, Color.yellow);
#endif
        #endregion
    }
}
