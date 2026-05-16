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

    // Przesuwa wszystkie bloki o jeden rząd w dół
    public void MoveBlocksDown()
    {
        // Znajdujemy wszystkie obiekty z komponentem Block w scenie
        Block[] allBlocks = FindObjectsOfType<Block>();

        foreach (Block block in allBlocks)
        {
            // Przesuwamy każdy blok o cellSize jednostek w dół
            block.transform.position += new Vector3(0, -cellSize, 0);
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