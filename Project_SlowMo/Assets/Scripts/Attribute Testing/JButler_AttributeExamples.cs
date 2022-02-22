using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////////////////
// Things need to be said above class.
//////////////////////////////////////////////////

/// <summary>
/// AddComponentMenu
/// </summary>

// You can create new components that will show up in the component menu. If you include a "/", it will put the next item as a child. So here is a Project SlowMo folder for all our scripts TeeHee :P.
[AddComponentMenu("Project SlowMo Scripts/JButler_AttributeExamples")]

/// <summary>
/// DisallowMultipleComponent
/// </summary>

// This prevents you from adding muliptle of the same script to the same GameObject.
[DisallowMultipleComponent]

/// <summary>
/// ExcludeFromPreset
/// </summary>

// Prevents you from creating presets of this class.
[ExcludeFromPreset]

/// <summary>
/// ExecuteAlways
/// </summary>

// Makes this code active even when you are are in the editor. Make sure if you add this to an object that you SEPERATE PLAY LOGIC from EDITOR LOGIC!!!
[ExecuteAlways]

/// <summary>
/// HelpURL
/// </summary>

// Adds an url link that can you can go too. This replaces the MonoBehavior page (? in the top right of the component).
[HelpURL("https://docs.unity3d.com/ScriptReference/HelpURLAttribute.html")]

/// <summary>
/// RequireComponent
/// </summary>

// Adds this component automatically as a dependency.
[RequireComponent(typeof(JButler_Hello))]
public class JButler_AttributeExamples : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    /// <summary>
    /// ColorUsage
    /// </summary>

    [Space(10)]
    [Header("ColorUsage Attribute")]
    [Tooltip("ColorUsage requires that you choose if to show alpha and if to show HDR colors.\nThis example has both on.")]
    [ColorUsage(true, true)] [SerializeField] private Color hdrWithAlpha;
    [Tooltip("This example has HDR off, but alpha on.")]
    [ColorUsage(true, false)] [SerializeField] private Color alphaOnly;
    [Tooltip("This example has the alpha off, but HDR on.")]
    [ColorUsage(false, true)] [SerializeField] private Color hdrOnly;
    [Tooltip("This example has both off.")]
    [ColorUsage(false, false)] [SerializeField] private Color colorOnly;

    /// <summary>
    /// ContextMenu
    /// </summary>

    // This appears under the menu options. Click the menu options/right click component (you know the place you reset values and stuff) and at the bootom you should see this.
    [ContextMenu("Change Value")]
    void AddToValue()
    {
        Debug.Log("You successfully changed the value!!!");
        changeValue++;
    }
    [Space(10)]
    [Header("ContextMenu")]
    [Tooltip("Change this value without touching it yourself.")]
    [SerializeField] private int changeValue = 0;

    /// <summary>
    /// ContextMenuItem
    /// </summary>

    // This is similar to above, but you give it a different name that shows up in the menu. As a note, you must attach this to a field (in this case resetValue). Another note, It shows up reversed order, bottom is top and top is bottom.
    [Space(10)]
    [Header("ContextMenuItem")]
    [Tooltip("Adds a menu to the field itself.\nRight click the field to see menu.")]
    [ContextMenuItem("Reset Value", "ResetBackToZero")]
    [ContextMenuItem("Add 100", "IncreaseBy100")]
    [SerializeField] private int resetValue = 100;
    void IncreaseBy100()
    {
        Debug.Log("Look! It is going up!!!");
        resetValue += 100;
    }
    void ResetBackToZero()
    {
        Debug.Log("Oh, we are back to zero.");
        resetValue = 0;
    }

    /// <summary>
    /// GradientUsage
    /// </summary>

    [Space(10)]
    [Header("GradientUsage")]
    [Tooltip("This allows us to select HDR colors.\nWhen false, it doesn't act differently if it wasn't there.")]
    [GradientUsage(true)]
    [SerializeField] private Gradient hdrGradient;

    /// <summary>
    /// Header
    /// </summary>

    // You know about headers.

    /// <summary>
    /// HideInInspector
    /// </summary>

    // Makes a variable not show up in the inspector, but makes it serialized.
    [HideInInspector] public string mySecrets;
    [Space(10)]
    [Header("HideInInspector")]
    [Tooltip("Something is hidden.\nSee if you can find it...")]
    [SerializeField] private string SomethingHidden;

    /// <summary>
    /// InspectorName
    /// </summary>

    // Change how the inspector shows what the enum's name is.
    private enum ModelImporterIndexFormat
    {
        Auto = 0,
        [InspectorName("16 bits")]
        UInt16 = 1,
        [InspectorName("32 bits")]
        UInt32 = 2,
    }
    [Space(10)]
    [Header("InspectorName")]
    [Tooltip("The name is different from what it is called in code.")]
    [SerializeField] private ModelImporterIndexFormat ourEnum = ModelImporterIndexFormat.Auto;

    /// <summary>
    /// Min
    /// </summary>

    // You can set the minimum value of either a float or int. I think in the script you can still get it under the min, but you can't in the inspector.
    [Space(10)]
    [Header("Min")]
    [Tooltip("I don't believe in negative numbers.")]
    [Min(0)]
    [SerializeField] private float negativeNumber = 0.0f;

    /// <summary>
    /// Multiline
    /// </summary>

    // Gives you a larger space to edit strings. The optional value determines the size of the box.
    [Space(10)]
    [Header("Multiline")]
    [Tooltip("I like long Monologs")]
    [Multiline(5)]
    [SerializeField] private string myMonolog;

    /// <summary>
    /// Range
    /// </summary>

    // You can use either a float or an int.
    [Space(10)]
    [Header("Range")]
    [Tooltip("Makes a slider within the two values.")]
    [Range(0f, 10f)]
    [SerializeField] private float slideMe = 5.0f;

    /// <summary>
    /// TextArea
    /// </summary>

    // Unlike the other option, this puts the area right underneath you. The first number determine the size of the area while the second determines how many lines you can see while scrolling.
    [Space(10)]
    [Header("TextArea")]
    [Tooltip("Another option for writing large amounts of text.")]
    [TextArea(5, 10)]
    [SerializeField] private string myLetterToMe;

    /// <summary>
    /// TagField
    /// </summary>

    // This only works if you are using Cinemachine inside your project. Comment this out if you are not.
    [Space(10)]
    [Header("TagField")]
    [Tooltip("This only works with Cinemachine, but you can use it with other code.")]
    [Cinemachine.TagField]
    [SerializeField] string test;

    //////////////////////////////////////////////////
    // Part of ExecuteAlways
    //////////////////////////////////////////////////

    [Space(10)]
    private float timer = 0.0f;
    public bool testExcuteAlways = true;

    private void Update()
    {
        if (!Application.IsPlaying(gameObject) && testExcuteAlways)
        {
            timer += Time.deltaTime;
            Debug.Log("Hello");
        }
    }
}
