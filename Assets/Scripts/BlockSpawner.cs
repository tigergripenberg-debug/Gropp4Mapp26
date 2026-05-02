using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    private int BlocksUsed = 0;
    public static BlockSpawner Instance;

    [Header("Settings")]
    public GameObject[] blockPrefabs;
    public Transform[] spawnPoints;
    public GameObject blockPrefab;
    [SerializeField] Color[] blockColors;

    private Shape[] shapes = new Shape[]
    {
        ShapeLibrary.LShape,
        ShapeLibrary.IShape,
        ShapeLibrary.IShapeStanding,
        ShapeLibrary.Square,
        ShapeLibrary.SShape,
        ShapeLibrary.TShape,
        ShapeLibrary.ZShape
    };

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        SpawnShapes();
    }

    Shape RandomShape()
    {
        Shape randomShape = shapes[Random.Range(0, shapes.Length)];
        return randomShape;
    }

    private void SpawnShapes()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Shape shape = RandomShape();
            GameObject shapeGO = new GameObject("Shape");
            var SH = shapeGO.AddComponent<ShapeBehaviour>();
            shapeGO.AddComponent<BoxCollider2D>();
            shapeGO.transform.position = spawnPoints[i].position;
            Vector2 center = shape.GetCenter();
            foreach (var cell in shape.cells)
            {
                Vector3 localPos = new Vector3(cell.x - center.x, cell.y - center.y, 0f);
                GameObject block = Instantiate(blockPrefab, shapeGO.transform);
                block.transform.localPosition = localPos;
            }
            SH.Initialize(shape, blockColors);
            SH.FitColliderToShape();
        }
    }
    public void BlockPlaced()
    {
        BlocksUsed++;
        if (Score.Instance != null)
        {
            Score.Instance.RegisterBlockPlaced();
        }
        if (Timer.Instance != null)
        {
            Timer.Instance.RegisterBlockPlaced();
        }
        GridTimerScript.Instance.decreaseValue();
        if (BlocksUsed >= 3)
        {
            BlocksUsed = 0;
            GridManager.Instance.OnTurnFinished();
            SpawnShapes();
        }
        GridManager.Instance.CheckIfPlayable();
    }
}

/*public void SpawnNewBlock() GAMLA SPAWNSYSTEMET
{
    GridTimerScript.Instance.resetValue();
    foreach (var spawnpoint in spawnPoints)
    {
        int randomIndex = Random.Range(0, blockPrefabs.Length);
        int attempts = 0;
        while (GridManager.Instance.CanBlockFit(blockPrefabs[randomIndex]) == false)
        {
            randomIndex = Random.Range(0, blockPrefabs.Length);
            attempts++;
            if (attempts > 50)
            {
                Debug.Log("Inget block får plats! Här ska vi trigga Game Over senare.");
                break;
            }
        }
        GameObject spawnedBlock = Instantiate(blockPrefabs[randomIndex], spawnpoint.position, Quaternion.identity);
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        foreach (Transform child in spawnedBlock.transform)
        {
            if (child.localPosition.x < minX) minX = child.localPosition.x;
            if (child.localPosition.x > maxX) maxX = child.localPosition.x;
            if (child.localPosition.y < minY) minY = child.localPosition.y;
            if (child.localPosition.y > maxY) maxY = child.localPosition.y;
        }
        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;
        Vector3 offset = new Vector3(centerX * 0.6f, centerY * 0.6f, 0f);
        spawnedBlock.transform.position -= offset;
    }
}*/