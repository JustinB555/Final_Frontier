using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will allow use to just create this script from the asset menu. Similar the adding stuff to the component menu, if you add the order it will put it at that position on the menu.
// You can NOT add a ScriptableObject to a GameObject since it doesn't derive from Monobehavior. Instead you use this to store large amounts of data.
//               Default file name when created      What shows in menu          Where it shows in menu
[CreateAssetMenu(fileName = "Data", menuName = "Data Managers/Holds Liquid Data", order = 1)]

// Suppose to help with data. Not my field of understanding right now.
[PreferBinarySerialization]

[AddComponentMenu("Project SlowMo Scripts/ScriptableObject")]
public class JButler_AttributeExamples2 : ScriptableObject //MonoBehaviour
{
    // Data goes here.
}
