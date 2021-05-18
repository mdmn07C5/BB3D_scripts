using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The purpsose of this scriptable object is to hold the mappings
/// of colors and gameobjects for the platform spawner.
/// There should be only one game object in the project
/// </summary>
public class ColorMappingSO : ScriptableObject
{
    static ColorMappingSO instance = null;

    public List<Color32> colors;
    public List<GameObject> bricks;

    public static ColorMappingSO Instance
    {
        get
        {
            if (instance == null)
            {
                #if UNITY_EDITOR
                instance = AssetDatabase.LoadAssetAtPath<ColorMappingSO>("Assets/ScriptableObjects/ColorMappings.asset");
                #endif
            }
            return instance;
        }
    }

    bool CompareColor32(Color32 c1, Color32 c2) {
        if (c1.r == c2.r && c1.b == c2.b && c1.g == c2.g)
        {
            return true;
        }
        return false;
    }

    public bool IsInColorList(Color32 c) {
        foreach (Color32 color in colors) {
            if (CompareColor32(c, color)) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Find and returns the index of the color
    /// </summary>
    /// <param name="c">Color chosen to find its index</param>
    /// <returns>The index of the color. It is -1 if not found</returns>
    public int IndexOfColor(Color32 c) {
        int index = 0;
        foreach (Color32 color in colors)
        {
            if (CompareColor32(c, color)){
                return index;
            }
            index++;
        }
        index = -1;
        return index;
    }

}
