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
    [Header("Planes (opposites)")]
    public Renderer right;
    public Renderer left;

    public Renderer top;
    public Renderer bottom;

    public Renderer front;
    public Renderer back;

    [Header("All fixed image pairs (16 images = 8 pairs)")]
    public ImagePair[] allPairs;

    [Header("Empty material")]
    public Material emptyMaterial;

    private Renderer[][] planePairs;

    void Start()
    {
        if (allPairs.Length < 3)
        {
            Debug.LogError("You need at least 3 image pairs.");
            return;
        }

        planePairs = new Renderer[][]
        {
            new Renderer[]{ right, left },
            new Renderer[]{ top, bottom },
            new Renderer[]{ front, back }
        };

        // Shuffle all available pairs
        List<ImagePair> shuffledPairs = new List<ImagePair>(allPairs);
        for (int i = 0; i < shuffledPairs.Count; i++)
        {
            int rand = Random.Range(i, shuffledPairs.Count);
            (shuffledPairs[i], shuffledPairs[rand]) = (shuffledPairs[rand], shuffledPairs[i]);
        }

        // Take only the first 3 pairs for this cube
        ImagePair[] chosenPairs = new ImagePair[3];
        for (int i = 0; i < 3; i++)
            chosenPairs[i] = shuffledPairs[i];

        // Choose which pair is broken
        int brokenPairIndex = Random.Range(0, 3);
        int missingSide = Random.Range(0, 2); // 0 or 1

        for (int i = 0; i < 3; i++)
        {
            var planes = planePairs[i];
            var pair = chosenPairs[i];

            if (i == brokenPairIndex)
            {
                // One side empty, the other side gets ONE image from the pair
                planes[missingSide].material = emptyMaterial;
                planes[1 - missingSide].material =
                    Random.value < 0.5f ? pair.a : pair.b;
            }
            else
            {
                // Normal pair: A on one side, B on the opposite
                planes[0].material = pair.a;
                planes[1].material = pair.b;
            }
        }
    }
}

