using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaskInputManager : MonoBehaviour
{
    [SerializeField] private GameObject maskInputNode;
    [SerializeField] private List<GameObject> submitButtons;

    [SerializeField] private Material PurpleGlowMaterial;
    [SerializeField] private Material RedGlowMaterial;
    [SerializeField] private Material YellowGlowMaterial;

    [SerializeField] private List<Material> markingMaterials;

    [Header("Grid")]
    [SerializeField] private int gridSize;
    [SerializeField] private float gridScale;
    [SerializeField] private Transform gridInputNodeParent;
    public List<GameObject> gridInputNodes = new();
    public List<bool> gridAnswer = new();
    public List<bool> gridGuess = new();


    [Header("Hexagon")]
    public List<GameObject> hexagonInputNodes = new();
    public List<bool> hexagonAnswer = new();
    public List<bool> hexagonGuess = new();

    [Header("Markings")]
    public List<GameObject> markingInputNodes = new();
    public List<bool> markingAnswer = new();
    public List<bool> markingGuess = new();

    private void Awake() {
        SpawnGridNodes();
        SpawnHexagonLists();
        SpawnMarkingsLists();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = new();
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray.origin,ray.direction, out hit)) {
                if(hit.collider.TryGetComponent(out Identifier identifier))
                {
                    if(identifier.inputNode)
                    SelectNode(hit.collider.gameObject);
                    else if (identifier.submitDial)
                    StartCoroutine(SubmitGuess(submitButtons.IndexOf(hit.collider.gameObject)));
                }
				
			}
        }
    }

    private void SpawnHexagonLists()
    {
        for (int i = 0; i < 7; i++)
        {
            hexagonAnswer.Add(false);
            hexagonGuess.Add(false);
        }
    }
     private void SpawnMarkingsLists()
    {
        for (int i = 0; i < 16; i++)
        {
            markingAnswer.Add(false);
            markingGuess.Add(false);
        }
    }



    private void SpawnGridNodes()
    {
        for (int i = 0; i < Mathf.Pow(gridSize, 2); i++)
        {
            GameObject newMaskInputNode = Instantiate(maskInputNode, gridInputNodeParent);
            newMaskInputNode.transform.localPosition = new Vector3(i % gridSize, 0, Mathf.FloorToInt(i / gridSize));
            gridInputNodes.Add(newMaskInputNode);
            gridGuess.Add(false);
            gridAnswer.Add(false);
        }
        gridInputNodeParent.transform.localScale = new Vector3(gridScale, 1, gridScale) / (gridSize - 1);
        gridInputNodeParent.transform.position = gridInputNodeParent.parent.transform.position - new Vector3(gridScale / 2, 0, gridScale / 2);

    }

    private void SelectNode(GameObject node)
    {
        Debug.Log("node selected");
        bool thisNodeSelected = node.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy;
        switch (node.GetComponent<Identifier>().nodeType)
        {
            case Identifier.NodeType.grid:
            gridGuess[gridInputNodes.IndexOf(node)] = !thisNodeSelected;
            node.transform.GetChild(0).GetChild(0).gameObject.SetActive(!thisNodeSelected);
                break;
            case Identifier.NodeType.hexagon:
            hexagonGuess[hexagonInputNodes.IndexOf(node)] = !thisNodeSelected;
            node.transform.GetChild(0).GetChild(0).gameObject.SetActive(!thisNodeSelected);
                break;
            case Identifier.NodeType.marking:
            markingGuess[markingInputNodes.IndexOf(node)] = !thisNodeSelected;
            node.transform.GetChild(0).GetChild(0).gameObject.SetActive(!thisNodeSelected);
                break;
            /*
            int index = markingInputNodes.IndexOf(node);
            thisNodeSelected = node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.IsKeywordEnabled("_EMISSIVE");
            markingGuess[index] = !thisNodeSelected;
            Debug.Log("marking clicked");
            
            if(!thisNodeSelected)
            {
                node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSIVE");
                node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", PurpleGlowMaterial.GetColor("_EmissionColor"));
            }
            else
            {
                node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSIVE");
                node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
                break;*/
            /*
            if (!thisNodeSelected)
                {
                    Debug.Log("selecting now");
                    markingMaterials[index].EnableKeyword("_EMISSIVE");
                    markingMaterials[index].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                    node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = markingMaterials[index];
                }
            else
            markingMaterials[index].DisableKeyword("_EMISSIVE");*/
            
/*
            if (!thisNodeSelected)
            {
                Material changedMaterial = node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
                changedMaterial.EnableKeyword("_EMISSIVE");
                node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = changedMaterial;
            }
            else
            {
                Material changedMaterial = node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
                changedMaterial.DisableKeyword("_EMISSIVE");
                node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = changedMaterial;
            }*/
            /*if(thisNodeSelected)
            node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSIVE");
            else
            node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSIVE");
                break;*/
        }

        /*
        if (thisNodeSelected)
            {
                Material changedMaterial = node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
                changedMaterial.EnableKeyword("_EMISSIVE");
                node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = changedMaterial;
            }
            else
            {
                Material changedMaterial = node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
                changedMaterial.DisableKeyword("_EMISSIVE");
                node.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = changedMaterial;
            }
                break;
                */
       
    }

    private IEnumerator SubmitGuess(int dialIndex)
    {
        submitButtons[dialIndex].transform.GetChild(0).GetComponent<MeshRenderer>().material = PurpleGlowMaterial;
        yield return new WaitForSeconds(1.6f);

        bool correct = false;
        switch (dialIndex)
        {
            case 0:
                correct = SubmitGridGuess();
                break;
            case 1:
                correct = SubmitHexagonGuess(); //change these
                break;
            case 2:
                correct =  SubmitMarkingsGuess();
                break;
        }

        if(correct)
        {
            submitButtons[dialIndex].transform.GetChild(0).GetComponent<MeshRenderer>().material = YellowGlowMaterial;
        }
        else
        {
            submitButtons[dialIndex].transform.GetChild(0).GetComponent<MeshRenderer>().material = RedGlowMaterial;
        }
    }

    private bool SubmitGridGuess()
    {
        if(gridGuess.SequenceEqual(gridAnswer))
        {
            //correct answer
            for(int i = 0; i < gridInputNodes.Count; i++)
            {
                if(gridAnswer[i] == true)
                {
                    gridInputNodes[i].transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = YellowGlowMaterial;
                }
            }
            return true;
        }
        else
        {
            //wrong answer
            for(int i = 0; i < gridInputNodes.Count; i++)
            {
                gridInputNodes[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                gridInputNodes[i].transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = RedGlowMaterial;
            }
            return false;
        }
        //
    }


     private bool SubmitHexagonGuess()
    {
        if(hexagonGuess.SequenceEqual(hexagonAnswer))
        {
            //correct answer
            for(int i = 0; i < hexagonInputNodes.Count; i++)
            {
                if(hexagonAnswer[i] == true)
                {
                    hexagonInputNodes[i].transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = YellowGlowMaterial;
                }
            }
            return true;
        }
        else
        {
            //wrong answer
            for(int i = 0; i < hexagonInputNodes.Count; i++)
            {
                hexagonInputNodes[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                hexagonInputNodes[i].transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = RedGlowMaterial;
            }
            return false;
        }
        //
    }

     private bool SubmitMarkingsGuess()
    {
        if(markingGuess.SequenceEqual(markingAnswer))
        {
            Color emissionColour = YellowGlowMaterial.GetColor("_EmissionColor");
            //correct answer
            for(int i = 0; i < markingInputNodes.Count; i++)
            {
                markingInputNodes[i].transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", emissionColour);
            }
            return true;
        }
        else
        {
             Color emissionColour = RedGlowMaterial.GetColor("_EmissionColor");
            //wrong answer
            for(int i = 0; i < markingInputNodes.Count; i++)
            {
                markingInputNodes[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                markingInputNodes[i].transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", emissionColour);
                //markingInputNodes[i].transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSIVE");
            }
            return false;
        }
        //
    }
}
