using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintsManager : MonoBehaviour
{

    [Header("Map")]
    [SerializeField] private List<GameObject> mapLines = new();

    //place the two icons!
    [SerializeField] private GameObject hexagonIcon;
    [SerializeField] private GameObject cubeIcon;

    private Vector2 hexagonPosition;
    private Vector2 cubePosition;
    [SerializeField] private Vector2 mapCellOrigin;
    [SerializeField] private Vector2 mapCellSize;

    [SerializeField] private List<Vector2> mapDirections = new();
    private List<Vector2> finalMapDirections = new();


    void Awake()
    {
        RandomiseMap();
    }
    private void RandomiseMap()
    {
        GenerateFinalGridSteps();
        PlaceMapIcons();
    }

    private void GenerateFinalGridSteps()
    {
        for(int i = 0; i < 3; i++)
        {
            finalMapDirections.Add(mapDirections[Random.Range(0, mapDirections.Count)]);
        }
    }

    private void PlaceMapIcons()
    {
        cubePosition = new Vector2(Random.Range(0, 7), Random.Range(0, 7));
        hexagonPosition = Vector2.zero;
        Vector2 randDistance = new Vector2(Random.Range(-6, 6), Random.Range(-6, 6));
        if(randDistance.x == 0 || randDistance.y == 0)
        {
            randDistance = new Vector2(Random.Range(2, 6), Random.Range(2, 6));
        }
        Debug.Log(randDistance.x);
        for(int i = 0; i < Mathf.Abs(randDistance.x) + 1; i++)
        {
            int index = GetCellFromIndex(new Vector2(mod((int)cubePosition.x + i * (int)Mathf.Sign(randDistance.x), 7), cubePosition.y));
            MaskInputManager.inst.gridAnswer[index] = !MaskInputManager.inst.gridAnswer[index];
            Debug.Log(new Vector2((cubePosition.x + i * Mathf.Sign(randDistance.x)) % 7, cubePosition.y));
        }
        Debug.Log(randDistance.y);
        for(int i = 1; i < Mathf.Abs(randDistance.y) + 1; i++)
        {
            int index = GetCellFromIndex(new Vector2(mod((int)(cubePosition.x + randDistance.x),  7), mod((int)(cubePosition.y + i * Mathf.Sign(randDistance.y)), 7)));
            MaskInputManager.inst.gridAnswer[index] = !MaskInputManager.inst.gridAnswer[index];
            Debug.Log(new Vector2((cubePosition.x + randDistance.x) % 7, (cubePosition.y + i * Mathf.Sign(randDistance.y)) % 7));
        }
        Debug.Log(finalMapDirections.Count);
        for(int i = 0; i < finalMapDirections.Count; i++)
        {
            Vector2 addedVector = cubePosition + randDistance;
            for(int j = 0; j <= i; j++)
            {
                addedVector += finalMapDirections[j];
            }
            addedVector = new Vector2(mod((int)addedVector.x, 7), mod((int)addedVector.y, 7));
            int index = GetCellFromIndex(addedVector);
            MaskInputManager.inst.gridAnswer[index] = !MaskInputManager.inst.gridAnswer[index];

            if(i == finalMapDirections.Count - 1)
            {
                hexagonPosition = addedVector;
            }

            Debug.Log(addedVector);
        }

        if(randDistance.y > 0) //we go up so bottom line is shown
        mapLines[0].SetActive(true);
        else
        mapLines[2].SetActive(true);
        
        if(randDistance.x > 0) //we go right so left line is shown
        mapLines[1].SetActive(true);
        else
        mapLines[3].SetActive(true);

        cubePosition = mapCellOrigin + cubePosition * mapCellSize;
        cubeIcon.transform.localPosition = new Vector3(0, cubePosition.y, cubePosition.x);
        hexagonPosition = mapCellOrigin + hexagonPosition * mapCellSize;
        hexagonIcon.transform.localPosition =  new Vector3(0, hexagonPosition.y, hexagonPosition.x);

        //Either direction version
        /*
            if(Random.value > 0.5f) //Horizontal first
        {
            for(int i = 0; i < Mathf.Sign(randDistance.x); i++)
            {
                int index = GetCellFromIndex(cubePosition + new Vector2(i * Mathf.Sign(randDistance.x) % 7, 0));
                MaskInputManager.inst.gridAnswer[index] = !MaskInputManager.inst.gridAnswer[index];
            }
            for(int i = 0; i < Mathf.Sign(randDistance.y); i++)
            {
                int index = GetCellFromIndex(new Vector2((cubePosition.x + randDistance.x) % 7, (cubePosition.y + i * Mathf.Sign(randDistance.y)) % 7));
                MaskInputManager.inst.gridAnswer[index] = !MaskInputManager.inst.gridAnswer[index];
            }
        }
        else //vertical first
        {
            for(int i = 0; i < Mathf.Sign(randDistance.y); i++)
            {
                int index = GetCellFromIndex(cubePosition + new Vector2(0, i * Mathf.Sign(randDistance.y) % 7));
                MaskInputManager.inst.gridAnswer[index] = !MaskInputManager.inst.gridAnswer[index];
            }
            for(int i = 0; i < Mathf.Sign(randDistance.y); i++)
            {
                int index = GetCellFromIndex(new Vector2((cubePosition.x + i * Mathf.Sign(randDistance.x)) % 7, (cubePosition.y + randDistance.y) % 7));
                MaskInputManager.inst.gridAnswer[index] = !MaskInputManager.inst.gridAnswer[index];
            }
        }
        */
    }


    int mod(int x, int m) {
    return (x%m + m)%m;
}
    private int GetCellFromIndex(Vector2 cell)
    {
        Debug.Log("index is" + ((int)cell.y * 7 + (int)cell.x));
        return (int)cell.y * 7 + (int)cell.x;
    }
}
