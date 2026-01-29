using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ImagePair
{
    public Material a;
    public Material b;
}

public class ImageCube : MonoBehaviour
{
    // Six renderers for the cube's faces
    public Renderer right;
    public Renderer left;

    public Renderer top;
    public Renderer bottom;

    public Renderer front;
    public Renderer back;

    // All possible image pairs
    public ImagePair[] allPairs;

    // Material to use for the missing face
    public Material emptyMaterial;

    // Pairs of renderers for each axis
    private Renderer[][] planePairs;

    void Start()
    {
        // Initialize the plane pairs
        planePairs = new Renderer[][]
        {
            new Renderer[]{ right, left },
            new Renderer[]{ top, bottom },
            new Renderer[]{ front, back }
        };

        // Shuffle and select image pairs
        List<ImagePair> shuffledPairs = new List<ImagePair>(allPairs);
        for (int i = 0; i < shuffledPairs.Count; i++)
        {
            int rand = Random.Range(i, shuffledPairs.Count);
            (shuffledPairs[i], shuffledPairs[rand]) = (shuffledPairs[rand], shuffledPairs[i]);
        }

        ImagePair[] chosenPairs = new ImagePair[3];
        for (int i = 0; i < 3; i++)
            chosenPairs[i] = shuffledPairs[i];

        int brokenPairIndex = Random.Range(0, 3);
        int missingSide = Random.Range(0, 2); // 0 or 1

        for (int i = 0; i < 3; i++)
        {
            var planes = planePairs[i];
            var pair = chosenPairs[i];

            if (i == brokenPairIndex)
            {
                planes[missingSide].material = emptyMaterial;
                planes[1 - missingSide].material =
                    Random.value < 0.5f ? pair.a : pair.b;
            }
            else
            {
                planes[0].material = pair.a;
                planes[1].material = pair.b;
            }
        }
    }
}

