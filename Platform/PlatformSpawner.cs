using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: Make class generic to accept any game object.
// TODO: Make the platform spawner take in an image and create the platform
public enum PlatformType { Rectangle, /*Circle,*/ ColorMapping };
public class PlatformSpawner : MonoBehaviour
{
    public static PlatformManager previousPlatform;
    public static Vector3 platformRootSavedPosition;

    [Tooltip("The shape the platform will be in")]
    public PlatformType platformType = PlatformType.Rectangle;

    [Tooltip("The amount of space between each object")]
    public float spacing = 0;

    [SerializeField]
    [Tooltip("The game objects the plafrom will be composed of")]
    [DrawIf("platformType", PlatformType.Rectangle)]
    //[DrawIf("platformType", PlatformType.Circle)]
    GameObject platformPiecePrefab = null;

    [SerializeField]
    [Tooltip("The Image used to generate the platform")]
    [DrawIf("platformType", PlatformType.ColorMapping)]
    Texture2D platformImage = null;

    [SerializeField]
    [DrawIf("platformType", PlatformType.Rectangle)]
    int platformLength = 0;
    [SerializeField]
    [DrawIf("platformType", PlatformType.Rectangle)]
    int platformWidth = 0;

    [SerializeField]
    private ColorMappingSO colorMapping;

    [SerializeField]
    private GameObject platformParent; 

    private Transform startingPoint;

    [HideInInspector] public GameObject platformGO;
    
    [HideInInspector]
    public GameObject platformRoot;

    /*[SerializeField]
    [DrawIf("platformType", PlatformType.Circle)]
    int radius = 0;*/

    // TODO: Make this function callable 

    /// <summary>
    /// This will create platform made of up of small gameobjects with the given 
    /// </summary>
    /// <param name="pType">The type of platform that wants to be made</param>
    /// <returns>The platform GameObject</returns>
    /// 

    private void Start()
    {
        CreateRoot();
    }

    public void ClearPlatforms()
    {
        Destroy(platformRoot);
        platformGO = null;
        CreateRoot();
    }

    public void CreateRoot()
    {
        platformRoot = new GameObject("PlatformRoot");
    }

    public void CallGeneratePlatform()
    {
        GeneratePlatform(PlatformType.ColorMapping);
    }

    public GameObject GeneratePlatform(PlatformType pType) {

        if (platformGO != null)
        {
            previousPlatform = platformGO.GetComponent<PlatformManager>();
        }

        platformGO = Instantiate(platformParent);

        switch (pType) {
            case PlatformType.Rectangle:
                GenerateSquare(platformGO);
                break;
            /*case PlatformType.Circle:
                platformGO = GenerateCircle();
                break;*/
            case PlatformType.ColorMapping:
                GeneratePlatformWithTexture(platformGO, platformImage);
                break;
        }

        if (previousPlatform != null)
        {
            TranslatePlatformToAnchor(platformGO.GetComponent<PlatformManager>(), previousPlatform);

        }
        else {
            TranslatePlatformToUnderPlayer(platformGO.transform);
        }
        platformGO.transform.parent = platformRoot.transform;
        return platformGO;
    }

    public void SaveRootPosition() {
        platformRootSavedPosition = platformRoot.transform.position;
    }

    public void ResetRoot()
    {
        platformRoot.transform.position = platformRootSavedPosition;
    }

    void TranslatePlatformToAnchor(PlatformManager plaform, PlatformManager previousPlatform) {
        Transform startingAnchor = plaform.startingAnchor;
        Transform endingAnchor = previousPlatform.endAnchor;
        Vector3 offset = new Vector3(startingAnchor.position.x - endingAnchor.position.x, 0, startingAnchor.position.z - endingAnchor.position.z);
        print(offset);
        plaform.transform.Translate(-offset);
    }

    void TranslatePlatformToUnderPlayer(Transform trans) {
        Vector3 offset = new Vector3(trans.position.x - startingPoint.position.x, trans.position.y, trans.position.z - startingPoint.position.z);
        print(offset);
        // print(startingPoint.position);
        trans.Translate(offset);
    }

    GameObject Apply2DArrayToPlatformComponent(GameObject platformGO, GameObject[,] arrayOfBricks) {
        
        return platformGO;
    }
        
    GameObject GeneratePlatformWithTexture(GameObject platformGO, Texture2D texture)
    {
        //Move the Z position to the starting position over to the 3rd quadrant of the graph
        float startingHeight = -(texture.height+spacing * (texture.height -2)) / 2;
        //Move the X position to the starting position over to the 3rd quadrant of the graph
        float startingWidth = -(texture.width + spacing * (texture.width -2)) / 2;
        //Starting position of the shape
        Vector3 pos = new Vector3(startingWidth, -100, startingHeight);
        //Place the reference of the bricks in a 2d array
        GameObject[,] arrayOfBricks2D = new GameObject[texture.height, texture.width];
        //Pixels of the image mapped
        Color32[] pixels = texture.GetPixels32(0);
        int indexOfPixel = 0;
        PlatformManager platformComponent = platformGO.GetComponent<PlatformManager>();
        for (int i = 0; i < texture.height; ++i)
        {
            for (int j = 0; j < texture.width; ++j)
            {
                //If the brick is in the ColorMapping, Instantiate a brick and set its parent to the platform GameObject
                int indexOfColorInMap = colorMapping.IndexOfColor(pixels[indexOfPixel]);
                if (indexOfColorInMap != -1)
                {   
                    GameObject b = colorMapping.bricks[indexOfColorInMap];
                    if (b != null)
                    {
                        arrayOfBricks2D[i, j] = Instantiate(b, pos, Quaternion.identity);

                        if (arrayOfBricks2D[i, j].GetComponent<WinLevel>())
                        {
                            platformComponent.victoryPlatforms.Add(arrayOfBricks2D[i, j]);
                        }
                        else
                        {
                            platformComponent.normalPlatforms.Add(arrayOfBricks2D[i, j]);
                        }

                        arrayOfBricks2D[i, j].transform.SetParent(platformGO.transform);
                    }
                    if (indexOfColorInMap == 0)
                    {
                        startingPoint = arrayOfBricks2D[i, j].transform;
                    }
                    else if (indexOfColorInMap == 1)
                    {
                        GameObject temp = new GameObject();
                        temp.transform.position = pos;
                        temp.transform.SetParent(platformGO.transform);
                        platformComponent.startingAnchor = temp.transform;
                    }
                    else if (indexOfColorInMap == 2) {
                        GameObject temp = new GameObject();
                        temp.transform.position = pos;
                        temp.transform.SetParent(platformGO.transform);
                        platformComponent.endAnchor = temp.transform;
                    }
                    else
                    {
                        Debug.LogWarning("The color has no mapping: " + pixels[indexOfPixel]);
                    }
                }
                pos.x += 1f + spacing;
                indexOfPixel++;
            }
            pos.x = startingWidth;
            pos.z += 1f + spacing;
        }
        Apply2DArrayToPlatformComponent(platformGO, arrayOfBricks2D);
        return platformGO;
    }

    GameObject GenerateSquare(GameObject platformGO) {
        //Move the Z position to the starting position over to the 3rd quadrant of the graph
        float startingHeight = -(platformLength + spacing * platformLength) / 2;
        //Move the X position to the starting position over to the 3rd quadrant of the graph
        float startingWidth = -(platformWidth + spacing * platformWidth) / 2;
        //Starting position of the shape
        Vector3 pos = new Vector3(startingWidth-1, 0, startingHeight-1);
        //Place the reference of the bricks in a 2d array
        GameObject[,] arrayOfBricks2D = new GameObject[platformLength, platformWidth];

        for (int i = 0; i < platformLength; ++i) {
            pos.x = startingWidth-1;
            pos.z += 1f + spacing;
            for (int j = 0; j < platformWidth; ++j) {
                pos.x += 1f + spacing;
                arrayOfBricks2D[i,j] = Instantiate(platformPiecePrefab, pos, Quaternion.identity);
                arrayOfBricks2D[i, j].transform.SetParent(platformGO.transform);
            }
        }
        return Apply2DArrayToPlatformComponent(platformGO, arrayOfBricks2D);
    }

    public void SetPlatformImage(Texture2D level)
    {
        platformImage = level;
    }

    /*GameObject GenerateCircle()
    {
        GameObject platformGO = new GameObject("Platform");
        SemiCircle(platformGO, false);
        SemiCircle(platformGO, true);
        return platformGO;
    }*/

    // TODO: Consider changing breaking up the semicircle into quarter circles or giving degrees
    /*void SemiCircle(GameObject platformGO, bool isFlipped) {
        //lamda function to flip the semicircle
        Func<float, float> op;
        float initialZ = -1.001f;
        if (isFlipped)
        {
            op = (float x) => { x -= 1.001f; return x; };
            initialZ = 0.001f;
        }
        else {
            op = (float x) => { x += 1.001f; return x; };
            initialZ = -1.001f;
        }
        Vector3 pos = new Vector3(0, 0, 0);
        int cSquared = radius * radius;
        //Top Quarter of the circle

        for (int x = -radius; x * x <= cSquared; ++x)
        {
            pos.x += 1.001f;
            pos.z = initialZ;
            for (int y = 0; (y * y + x * x <= cSquared); ++y)
            {
                pos.z = op(pos.z);
                Instantiate(platformPiecePrefab, pos, Quaternion.identity, platformGO.transform);
            }
        }
    }*/

}