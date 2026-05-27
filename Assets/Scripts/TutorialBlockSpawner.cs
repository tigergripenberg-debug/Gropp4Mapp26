using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBlockSpawner : MonoBehaviour
{
    public static TutorialBlockSpawner Instance;
    
    [Header("Settings")]
    public Transform[] spawnPoints; 
    public GameObject blockPrefab; 
    [SerializeField] public Color[] blockColors; 

    [Header("Runda 1 - Shapes (Exakta namn från ShapeLibrary)")]
    public string[] round1Shapes = { "5 Long Laying", "2 Long Standing", "Square" }; 

    [Header("Runda 2 - Shapes (Exakta namn från ShapeLibrary)")]
    public string[] round2Shapes = { "Square", "Square", "Square" }; // Byt ut dessa i Unity!

    [HideInInspector] public GameObject[] spawnedBlocks = new GameObject[3];
    private int blocksUsed = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Starta första rundan direkt när spelet börjar
        StartCoroutine(SpawnSequence(1));
    }

    // Denna anropas av TutorialManager när första rundan är klar!
    public void StartSecondRound()
    {
        StartCoroutine(SpawnSequence(2));
    }

    private IEnumerator SpawnSequence(int roundNumber)
    {
        yield return null;
        
        // Välj vilken lista med block vi ska använda beroende på runda
        string[] shapesToSpawn = roundNumber == 1 ? round1Shapes : round2Shapes;
        SpawnTutorialBlocks(shapesToSpawn);
    }

    private void SpawnTutorialBlocks(string[] shapeNames)
    {
        if (GridTimerScript.Instance != null) GridTimerScript.Instance.resetValue();
        
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (i >= shapeNames.Length) break;

            Shape shape = ShapeLibrary.GetByName(shapeNames[i]);
            if (shape == null) continue;

            GameObject blockGO = CreateShapeVisuals(shape, spawnPoints[i]);
            spawnedBlocks[i] = blockGO; 
        }

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnBlocksReady(spawnedBlocks);
        }
    }

    private GameObject CreateShapeVisuals(Shape shape, Transform spawnpoint)
    {
        GameObject shapeGO = new GameObject("TutorialShape_" + shape.Name);
        shapeGO.transform.SetParent(transform);
        ShapeBehaviour shapeBehaviour = shapeGO.AddComponent<ShapeBehaviour>();
        BoxCollider2D col = shapeGO.AddComponent<BoxCollider2D>();
        col.enabled = false;
        shapeGO.transform.position = spawnpoint.position;
        Vector2Int origin = shape.GetOriginCell();
        
        foreach (var cell in shape.cells)
        {
            GameObject block = Instantiate(blockPrefab, shapeGO.transform);
            Vector3 localPos = new Vector3(cell.x - origin.x, cell.y - origin.y, 0f);
            block.transform.localPosition = localPos;
        }

        if (shapeGO.transform.childCount > 0)
        {
            float minX = shapeGO.transform.GetChild(0).localPosition.x;
            float maxX = minX;
            float minY = shapeGO.transform.GetChild(0).localPosition.y;
            float maxY = minY;

            foreach (Transform child in shapeGO.transform)
            {
                Vector3 pos = child.localPosition;
                if (pos.x < minX) minX = pos.x;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.y > maxY) maxY = pos.y;
            }
            float centerX = (minX + maxX) / 2f;
            float centerY = (minY + maxY) / 2f;
            Vector3 offset = new Vector3(centerX * 0.6f, centerY * 0.6f, 0f);
            shapeGO.transform.position -= offset;
        }
        
        shapeBehaviour.Initialize(shape, blockColors, blockPrefab);
        shapeBehaviour.FitColliderToShape();
        col.enabled = true;
        return shapeGO;
    }

    public void BlockPlaced()
    {
        blocksUsed++;
        if (Score.Instance != null) Score.Instance.RegisterBlockPlaced();
        if (Timer.Instance != null) Timer.Instance.RegisterBlockPlaced();
        if (GridTimerScript.Instance != null) GridTimerScript.Instance.decreaseValue();
        
        if (blocksUsed >= 3)
        {
            blocksUsed = 0;
            GridManager.Instance.OnTurnFinished();
            // Tutorialen är klar! Vi stänger AV kollen för GameOver här, 
            // så att TutorialManager får sköta övergången ifred.
        }
        else
        {
            // Kolla bara om det går att lägga block så länge vi har block kvar i tutorialen
            GridManager.Instance.CheckIfPlayable();
        }
    }
}