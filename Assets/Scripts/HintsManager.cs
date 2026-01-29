using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Emit;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HintsManager : MonoBehaviour
{
    public static HintsManager inst;
    private Camera mainCamera;
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

    private List<int> mapEndOfPathDirections = new();
    

     private List<string> mapRegularLetters = new List<string>(){ "A", "B", "C", "F", "G", "H", "I", "J", "K", "M", "O", "P", "Q", "T", "V", "X", "Y", "Z"};
    private enum HintDirection
    {
        down, right, up, left
    }

    private List<LetterHintDirection> letterHintDirections = new List<LetterHintDirection>(){
        new() { label = "D",  direction = HintDirection.down},
        new() { label = "S",  direction = HintDirection.down},
        new() { label = "R",  direction = HintDirection.right},
        new() { label = "E",  direction = HintDirection.right},
        new() { label = "U",  direction = HintDirection.up},
        new() { label = "N",  direction = HintDirection.up},
        new() { label = "L",  direction = HintDirection.left},
        new() { label = "W",  direction = HintDirection.left},
        };

    private class LetterHintDirection
    {
        public string label;
        public HintDirection direction;
    }

    private HintDirection cubeDirection;
    private int cubeDirectionCount;
    private HintDirection hexagonDirection;
    private int hexagonDirectionCount;

    public List<TextMeshProUGUI> mapColumnLabels = new();


    [Header("Tablet")]
    public GameObject tabletFrontCanvas;
    public GameObject tabletBackCanvas;
    public List<int> tabletHexagonCode = new();

    private enum TabletPath
    {
        Map,
        Hexagon,
        Cube,
        none
    }
    private TabletPath currentTabletPath = TabletPath.none;
    public int pathProgress;

    private int[] currentTabletNumbers = new int[4];
    private List<int> availableTabletNumbers = new();

    private List<int> availableTabletNumbersPool = new(){1, 2, 3, 4, 5, 6, 7};

    private int currentTabletCenterNumber;

    private bool tabletFacingReverse = false;

    [SerializeField] private GameObject canvasRotationReference;
    

    void Awake()
    {
        mainCamera = Camera.main;
        inst = this;
        NewMap();
        GenerateTabletCodes();
    }

    
    private void Reset()
    {
        //lines
        foreach(GameObject line in mapLines)
        {
            line.SetActive(false);
        }
        NewMap();
    }


    public void StartRotateTablet(Vector3 direction, bool sideInput)
    {
        availableTabletNumbers = new(availableTabletNumbersPool);
        availableTabletNumbers.Remove(currentTabletCenterNumber);
        currentTabletCenterNumber = currentTabletNumbers[TabletDirection(direction) - 1];
        if(!tabletFacingReverse)
        {
            SpawnOuterNumbers(tabletBackCanvas.transform, direction);
        }
        else
        {
            SpawnOuterNumbers(tabletFrontCanvas.transform, direction);
        }
    }

    public void EndRotateTablet(Vector3 direction, bool sideInput)
    {
        if(tabletFacingReverse)
        {
            tabletFrontCanvas.transform.rotation = canvasRotationReference.transform.rotation;
        }
        else
        {
            tabletBackCanvas.transform.rotation = canvasRotationReference.transform.rotation;
        }
        tabletFacingReverse = !tabletFacingReverse;
        
    }

    private void SpawnOuterNumbers(Transform canvas, Vector3 direction)
    {
        SpawnCenterNumber(canvas);
        currentTabletNumbers = new int[4];

        switch(currentTabletPath)
        {
            case TabletPath.Map:
            pathProgress++;
            canvas.GetChild(TabletDirection(direction)).GetComponent<TextMeshProUGUI>().text = "" + 8 + pathProgress;
            currentTabletNumbers[TabletDirection(direction)] = 8 + pathProgress;
            for(int i = 0; i < 4; i++)
                {
                    if(currentTabletNumbers[i] == 0)
                    {
                        currentTabletNumbers[i] = SpawnTabletNumber(availableTabletNumbers,  canvas.GetChild(i + 1).GetComponent<TextMeshProUGUI>());
                        availableTabletNumbers.Remove(currentTabletNumbers[i]);
                    }
                }
                break;
            case TabletPath.none:
            pathProgress = 0;
            for(int i = 0; i < 4; i++)
                {
                    if(currentTabletNumbers[i] == 0)
                    {
                        currentTabletNumbers[i] = SpawnTabletNumber(availableTabletNumbers,  canvas.GetChild(i + 1).GetComponent<TextMeshProUGUI>());
                        availableTabletNumbers.Remove(currentTabletNumbers[i]);
                    }
                }
                break;

        }
    }

    private void SpawnCenterNumber(Transform canvas)
    {
        canvas.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentTabletCenterNumber + "";
    }

    private int SpawnTabletNumber(List<int> availableNumbers, TextMeshProUGUI textTarget)
    {
        int number = availableNumbers[Random.Range(0, availableNumbers.Count)];
        textTarget.text = "" + number;
        return number;
    }

    private void GenerateTabletCodes()
    {
        for(int i = 0; i < hexagonDirectionCount; i++)
        {
            int nextNumber = Random.Range(1, 8);
            tabletHexagonCode.Add(nextNumber);
            MaskInputManager.inst.hexagonAnswer[nextNumber - 1] = !MaskInputManager.inst.hexagonAnswer[nextNumber - 1];
        }
        for(int i = 0; i < cubeDirectionCount; i++)
        {
            int nextNumber = Random.Range(1, 5);
            tabletHexagonCode.Add(nextNumber);
            //MaskInputManager.inst.hexagonAnswer[nextNumber] = !MaskInputManager.inst.hexagonAnswer[nextNumber];
        }
    }

    //1, 2, 4, 2, 3, 1
    //1, 2, 1, 2, 6, 1
    //3, 4, 1, 2, 3, 1
    //3, 4, 6, 3, 4, 6

    private void NewMap()
    {
        GenerateFinalGridSteps();
        GenerateMap();
    }

    private void GenerateFinalGridSteps()
    {
        finalMapDirections = new();
        for(int i = 0; i < 3; i++)
        {
            int direction = Random.Range(0, mapDirections.Count);
            finalMapDirections.Add(mapDirections[direction]);
            mapEndOfPathDirections.Add(direction + 1);
        }
    }

    private void GenerateMap()
    {
        cubePosition = new Vector2(Random.Range(0, 7), Random.Range(0, 7));
        hexagonPosition = Vector2.zero;
        Vector2 randDistance = new Vector2(Random.Range(-6, 6), Random.Range(-6, 6));
        if(randDistance.x == 0 || randDistance.y == 0)
        {
            randDistance = new Vector2(Random.Range(2, 6), Random.Range(2, 6));
        }
        for(int i = 0; i < Mathf.Abs(randDistance.x) + 1; i++)
        {
            int index = GetCellFromIndex(new Vector2(mod((int)cubePosition.x + i * (int)Mathf.Sign(randDistance.x), 7), cubePosition.y));
            MaskInputManager.inst.gridAnswer[index] = !MaskInputManager.inst.gridAnswer[index];
        }
        for(int i = 1; i < Mathf.Abs(randDistance.y) + 1; i++)
        {
            int index = GetCellFromIndex(new Vector2(mod((int)(cubePosition.x + randDistance.x),  7), mod((int)(cubePosition.y + i * Mathf.Sign(randDistance.y)), 7)));
            MaskInputManager.inst.gridAnswer[index] = !MaskInputManager.inst.gridAnswer[index];
        }
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
        }

        if(randDistance.y > 0) //we go up so bottom line is shown
        mapLines[0].SetActive(true);
        else
        mapLines[2].SetActive(true);
        
        if(randDistance.x > 0) //we go right so left line is shown
        mapLines[1].SetActive(true);
        else
        mapLines[3].SetActive(true);

        //Spawn letters underneath the map
        List<LetterHintDirection> tempLetters = new(letterHintDirections);
        for(int i = 0; i < mapColumnLabels.Count; i++)
        {
            if(Mathf.RoundToInt(cubePosition.x) == i)
            {   
                Debug.Log("ya");
                int hintIndex = Random.Range(0, tempLetters.Count);
                LetterHintDirection chosenHintDirection = tempLetters[hintIndex];
                LetterHintDirection otherHintDirection = tempLetters[-(hintIndex % 2 * 2 - 1) + hintIndex];
                cubeDirection = chosenHintDirection.direction;
                cubeDirectionCount = Mathf.RoundToInt(cubePosition.y);
                mapColumnLabels[i].text = chosenHintDirection.label;
                tempLetters.Remove(chosenHintDirection);
                tempLetters.Remove(otherHintDirection);
            }
            else if (Mathf.RoundToInt(hexagonPosition.x) == i)
            {
                int hintIndex = Random.Range(0, tempLetters.Count);
                LetterHintDirection chosenHintDirection = tempLetters[hintIndex];
                LetterHintDirection otherHintDirection = tempLetters[-(hintIndex % 2 * 2 - 1) + hintIndex];
                hexagonDirection = chosenHintDirection.direction;
                hexagonDirectionCount = Mathf.RoundToInt(hexagonPosition.y);
                mapColumnLabels[i].text = chosenHintDirection.label;
                tempLetters.Remove(chosenHintDirection);
                tempLetters.Remove(otherHintDirection);
            }
            else
            {
                mapColumnLabels[i].text = mapRegularLetters[Random.Range(0, mapRegularLetters.Count)];
            }
        }

        if(cubePosition.x == hexagonPosition.y)
        {
            Reset();
            return;
        }

        cubePosition = mapCellOrigin + cubePosition * mapCellSize;
        cubeIcon.transform.localPosition = new Vector3(0, cubePosition.y, cubePosition.x);
        hexagonPosition = mapCellOrigin + hexagonPosition * mapCellSize;
        hexagonIcon.transform.localPosition =  new Vector3(0, hexagonPosition.y, hexagonPosition.x);
    }


    int mod(int x, int m) 
    {
    return (x%m + m)%m;
    }
    private int GetCellFromIndex(Vector2 cell)
    {
        return (int)cell.y * 7 + (int)cell.x;
    }

    private int TabletDirection(Vector3 direction)
    {
        //x - 1, abs, + 2
        //1, 0, 0, 2
        //-1, -2, 2, 4
        //z - 1, abs, + 1
        Debug.Log(direction);
        Debug.Log((int)Mathf.Abs((Mathf.Abs(direction.x - 1) + 2) * direction.x + (Mathf.Abs(direction.z - 1) + 1) * direction.z));
        return (int)Mathf.Abs((Mathf.Abs(direction.x - 1) + 2) * direction.x + (Mathf.Abs(direction.z - 1) + 1) * direction.z);
    }
}
