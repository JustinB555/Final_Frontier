using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DI_System : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DamageIndicater indicatoerPrefab = null;
    [SerializeField] private RectTransform holder = null;
    [SerializeField] private new Camera camera = null;
    [SerializeField] private Transform player = null;

    private Dictionary<Transform, DamageIndicater> Indicators = new Dictionary<Transform, DamageIndicater>();

    #region Delegates
    public static Action<Transform> CreateIndicator = delegate { };
    public static Func<Transform, bool> CheckIfObjectInSight = null;
    #endregion

    private void OnEnable()
    {
        CreateIndicator += Create;
        CheckIfObjectInSight += InSight;
    }
    private void OnDisable()
    {
        CreateIndicator -= Create;
        CheckIfObjectInSight -= InSight;
    }
    void Create(Transform target)
    {
        if (Indicators.ContainsKey(target))
        {
            Indicators[target].Reset();
            return;
        }
        DamageIndicater newIndicator = Instantiate(indicatoerPrefab, holder);
        newIndicator.Register(target, player, new Action(() => { Indicators.Remove(target); }));
        Indicators.Add(target, newIndicator);
    }
    bool InSight(Transform t)
    {
        Vector3 screenPoint = camera.WorldToViewportPoint(t.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }
}
