using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Liczba kolumn w siatce (poziomo)
    public int columns = 7;

    // Liczba wierszy w siatce (pionowo)
    public int rows = 9;

    // Rozmiar jednego kwadratu siatki w jednostkach Unity
    public float cellSize = 1f;

    // Pozycja lewego dolnego rogu siatki
    // Będziemy ją obliczać automatycznie żeby siatka była wyśrodkowana
    private Vector2 gridOrigin;

    void Start()
    {
        // Obliczamy pozycję lewego dolnego rogu siatki
        // Szerokość całej siatki to columns * cellSize (7 * 1 = 7 jednostek)
        // Dzielimy przez 2 żeby wyśrodkować poziomo
        // Wysokość całej siatki to rows * cellSize (9 * 1 = 9 jednostek)
        // Dzielimy przez 2 żeby wyśrodkować pionowo
        float gridWidth = columns * cellSize;
        float gridHeight = rows * cellSize;

        gridOrigin = new Vector2(-gridWidth / 2f, -gridHeight / 2f);

        Debug.Log("GridManager uruchomiony! Siatka: " + columns + "x" + rows);
        Debug.Log("Lewy dolny róg siatki: " + gridOrigin);
    }

    // Ta metoda zwraca pozycję środka danego kwadratu w siatce
    // Podajesz numer kolumny i wiersza (od 0), a ona zwraca pozycję w świecie gry
    public Vector2 GetCellCenter(int column, int row)
    {
        float x = gridOrigin.x + (column * cellSize) + (cellSize / 2f);
        float y = gridOrigin.y + (row * cellSize) + (cellSize / 2f);
        return new Vector2(x, y);
    }

    // Rysuje siatkę w edytorze Unity (tylko widoczne w Scene view, nie w grze)
    // Pomaga nam zobaczyć gdzie jest nasza plansza
    void OnDrawGizmos()
    {
        float gridWidth = columns * cellSize;
        float gridHeight = rows * cellSize;
        Vector2 origin = new Vector2(-gridWidth / 2f, -gridHeight / 2f);

        Gizmos.color = Color.red;

        // Rysujemy linie pionowe
        for (int x = 0; x <= columns; x++)
        {
            Vector3 start = new Vector3(origin.x + x * cellSize, origin.y, 0);
            Vector3 end = new Vector3(origin.x + x * cellSize, origin.y + gridHeight, 0);
            Gizmos.DrawLine(start, end);
        }

        // Rysujemy linie poziome
        for (int y = 0; y <= rows; y++)
        {
            Vector3 start = new Vector3(origin.x, origin.y + y * cellSize, 0);
            Vector3 end = new Vector3(origin.x + gridWidth, origin.y + y * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}