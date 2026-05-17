using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Liczba kolumn w siatce (poziomo)
    public int columns = 7;

    // Liczba wierszy w siatce (pionowo)
    public int rows = 9;

    // Rozmiar jednego kwadratu siatki w jednostkach Unity
    public float cellSize = 1f;

    // Referencja do prefaba bloku – przeciągniemy go w Inspectorze
    public GameObject blockPrefab;

    // Numer aktualnej tury – zaczyna od 1
    public int currentTurn = 1;

    // Pozycja lewego dolnego rogu siatki
    private Vector2 gridOrigin;

    // Reference to the pickup prefab
    public GameObject pickupPrefab;

    // Chance for each optional pickup to spawn (0-1)
    public float scatterSpawnChance = 0.3f;
    public float horizontalStrikeSpawnChance = 0.3f;
    public float verticalStrikeSpawnChance = 0.3f;

    void Awake()
    {
        // Obliczamy pozycję lewego dolnego rogu siatki
        float gridWidth = columns * cellSize;
        float gridHeight = rows * cellSize;
        gridOrigin = new Vector2(-gridWidth / 2f, -gridHeight / 2f);
    }

    // Zwraca pozycję środka danego kwadratu w siatce
    public Vector2 GetCellCenter(int column, int row)
    {
        float x = gridOrigin.x + (column * cellSize) + (cellSize / 2f);
        float y = gridOrigin.y + (row * cellSize) + (cellSize / 2f);
        return new Vector2(x, y);
    }

    // Spawnuje nowy rząd bloków na górze planszy
    public void SpawnNewRow()
    {
        // Górny rząd to rows-1 (bo liczymy od 0)
        // np. dla 9 wierszy: 0,1,2,3,4,5,6,7,8 – górny to 8
        int spawnRow = rows - 1;

        // Tworzymy listę kolumn które będą miały bloki
        // Co najmniej jedna kolumna musi pozostać pusta (zgodnie z dokumentacją)
        // Losujemy ile bloków spawnujemy: od 1 do columns-1
        int blockCount = Random.Range(1, columns);

        // Tworzymy tablicę wszystkich kolumn i tasujemy ją losowo
        int[] columnIndices = new int[columns];
        for (int i = 0; i < columns; i++)
        {
            columnIndices[i] = i;
        }

        // Algorytm tasowania Fisher-Yates
        for (int i = columns - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = columnIndices[i];
            columnIndices[i] = columnIndices[randomIndex];
            columnIndices[randomIndex] = temp;
        }

        // Spawnujemy bloki w pierwszych blockCount kolumnach z potasowanej tablicy
        for (int i = 0; i < blockCount; i++)
        {
            int column = columnIndices[i];
            Vector2 position = GetCellCenter(column, spawnRow);

            // Tworzymy blok w danej pozycji
            GameObject newBlock = Instantiate(blockPrefab, position, Quaternion.identity);

            // Ustawiamy HP bloku równe numerowi aktualnej tury
            newBlock.GetComponent<Block>().SetHealth(currentTurn);
        }
    }

    // Spawn pickups in the top row
    public void SpawnPickups()
    {
        int spawnRow = rows - 1;

        // Get all occupied columns in the top row
        Block[] allBlocks = FindObjectsOfType<Block>();
        bool[] occupiedColumns = new bool[columns];

        // Mark columns that already have blocks in the top row
        foreach (Block block in allBlocks)
        {
            if (block.transform.position.y >= GetCellCenter(0, spawnRow).y - 0.1f)
            {
                int col = Mathf.RoundToInt((block.transform.position.x - gridOrigin.x) / cellSize - 0.5f);
                if (col >= 0 && col < columns)
                {
                    occupiedColumns[col] = true;
                }
            }
        }

        // Find free columns
        System.Collections.Generic.List<int> freeColumns = new System.Collections.Generic.List<int>();
        for (int i = 0; i < columns; i++)
        {
            if (!occupiedColumns[i])
            {
                freeColumns.Add(i);
            }
        }

        // Spawn one Plus pickup in a random free column
        if (freeColumns.Count > 0)
        {
            int randomIndex = Random.Range(0, freeColumns.Count);
            int column = freeColumns[randomIndex];
            Vector2 position = GetCellCenter(column, spawnRow);
            GameObject plusPickup = Instantiate(pickupPrefab, position, Quaternion.identity);
            plusPickup.GetComponent<Pickup>().Initialize(Pickup.PickupType.Plus);
            freeColumns.RemoveAt(randomIndex);
        }

        // Try to spawn optional pickups in remaining free columns
        TrySpawnOptionalPickup(freeColumns, Pickup.PickupType.Scatter, scatterSpawnChance, spawnRow);
        TrySpawnOptionalPickup(freeColumns, Pickup.PickupType.HorizontalStrike, horizontalStrikeSpawnChance, spawnRow);
        TrySpawnOptionalPickup(freeColumns, Pickup.PickupType.VerticalStrike, verticalStrikeSpawnChance, spawnRow);
    }

    // Try to spawn an optional pickup in a random free column
    void TrySpawnOptionalPickup(System.Collections.Generic.List<int> freeColumns, Pickup.PickupType type, float chance, int spawnRow)
    {
        // Check if there are free columns and if pickup should spawn
        if (freeColumns.Count > 0 && Random.value < chance)
        {
            int randomIndex = Random.Range(0, freeColumns.Count);
            int column = freeColumns[randomIndex];
            Vector2 position = GetCellCenter(column, spawnRow);
            GameObject pickup = Instantiate(pickupPrefab, position, Quaternion.identity);
            pickup.GetComponent<Pickup>().Initialize(type);
            freeColumns.RemoveAt(randomIndex);
        }
    }

    public void MoveBlocksDown()
    {
        // Move all blocks down
        Block[] allBlocks = FindObjectsOfType<Block>();
        foreach (Block block in allBlocks)
        {
            block.transform.position += new Vector3(0, -cellSize, 0);
        }

        // Move all pickups down
        Pickup[] allPickups = FindObjectsOfType<Pickup>();
        foreach (Pickup pickup in allPickups)
        {
            pickup.transform.position += new Vector3(0, -cellSize, 0);
        }
    }

    // Check if any block has reached the bottom row
    public bool CheckGameOver()
    {
        Block[] allBlocks = FindObjectsOfType<Block>();

        foreach (Block block in allBlocks)
        {
            if (block.transform.position.y <= gridOrigin.y + cellSize)
            {
                return true;
            }
        }

        return false;
    }

    // Rysuje siatkę w edytorze Unity
    void OnDrawGizmos()
    {
        float gridWidth = columns * cellSize;
        float gridHeight = rows * cellSize;
        Vector2 origin = new Vector2(-gridWidth / 2f, -gridHeight / 2f);

        Gizmos.color = Color.red;

        for (int x = 0; x <= columns; x++)
        {
            Vector3 start = new Vector3(origin.x + x * cellSize, origin.y, 0);
            Vector3 end = new Vector3(origin.x + x * cellSize, origin.y + gridHeight, 0);
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= rows; y++)
        {
            Vector3 start = new Vector3(origin.x, origin.y + y * cellSize, 0);
            Vector3 end = new Vector3(origin.x + gridWidth, origin.y + y * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}