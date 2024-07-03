using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;

public class InstantMazeGenerator : MonoBehaviour
{
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;

    [SerializeField]
    private int _mazeHeight;

    [SerializeField]
    private int _seed;

    [SerializeField]
    private bool _useSeed;

    private MazeCell[,,] _mazeGrid;

    private List<MazeCell> _visitedCells;

    private List<MazeCell> _endPoints;

    void Start()
    {
        if (_useSeed)
        {
            Random.InitState(_seed);
        }
        else
        {
            int randomSeed = Random.Range(1, 1000000);
            Random.InitState(randomSeed);

            Debug.Log(randomSeed);
        }

        _mazeGrid = new MazeCell[_mazeWidth, _mazeHeight, _mazeDepth];
        _visitedCells = new List<MazeCell>();
        _endPoints = new List<MazeCell>();

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int y = 0; y < _mazeHeight; y++)
            {
                for (int z = 0; z < _mazeDepth; z++)
                {
                    Vector3 localPosition = new Vector3(x, y, z);
                    _mazeGrid[x, y, z] = Instantiate(_mazeCellPrefab, localPosition, Quaternion.identity, transform);
                    _mazeGrid[x, y, z].transform.localPosition = localPosition;
                }
            }
        }

        int centerX = _mazeWidth / 2;
        int centerY = _mazeHeight / 2;

        GenerateMaze(null, _mazeGrid[centerX, centerY, _mazeHeight], 0);

        GetComponent<NavMeshSurface>().BuildNavMesh();

        Debug.Log("cells: " + _visitedCells.Count);
        Debug.Log("end points: " + _endPoints.Count);
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell, int generation)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        _visitedCells.Add(currentCell);

        if (GetUnvisitedCells(currentCell, generation).Count() == 0)
        {
            _endPoints.Add(currentCell);
        }

        new WaitForSeconds(.05f);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell, generation);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell, generation + 1);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell, int generation)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell, generation);

        return unvisitedCells
            .OrderBy(cell =>
                Random.Range(1, 10) *
                (Mathf.Abs(cell.transform.localPosition.y - currentCell.transform.localPosition.y) > 0 ? 3 : 1))
            .FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell, int generation)
    {
        int x = (int)currentCell.transform.localPosition.x;
        int y = (int)currentCell.transform.localPosition.y;
        int z = (int)currentCell.transform.localPosition.z;

        List<MazeCell> cells = new List<MazeCell>();

        if (x + 1 < _mazeWidth && !_mazeGrid[x + 1, y, z].IsVisited)
            cells.Add(_mazeGrid[x + 1, y, z]);

        if (x - 1 >= 0 && !_mazeGrid[x - 1, y, z].IsVisited)
            cells.Add(_mazeGrid[x - 1, y, z]);

        if (z + 1 < _mazeDepth && !_mazeGrid[x, y, z + 1].IsVisited)
            cells.Add(_mazeGrid[x, y, z + 1]);

        if (z - 1 >= 0 && !_mazeGrid[x, y, z - 1].IsVisited)
            cells.Add(_mazeGrid[x, y, z - 1]);

        if (generation >= 5)
        {
            if (y + 1 < _mazeHeight && !_mazeGrid[x, y + 1, z].IsVisited)
                cells.Add(_mazeGrid[x, y + 1, z]);

            if (y - 1 >= 0 && !_mazeGrid[x, y - 1, z].IsVisited)
                cells.Add(_mazeGrid[x, y - 1, z]);
        }

        // Shuffle cells to introduce randomness, but bias against y-axis movement
        return cells.OrderBy(cell =>
            Random.Range(1, 10) *
            (Mathf.Abs(cell.transform.localPosition.y - currentCell.transform.localPosition.y) > 0 ? 3 : 1));
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null) return;

        if (previousCell.transform.localPosition.x < currentCell.transform.localPosition.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }
        if (previousCell.transform.localPosition.x > currentCell.transform.localPosition.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.localPosition.z < currentCell.transform.localPosition.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }
        if (previousCell.transform.localPosition.z > currentCell.transform.localPosition.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }

        if (previousCell.transform.localPosition.y < currentCell.transform.localPosition.y)
        {
            previousCell.ClearCeiling();
            currentCell.ClearFloor();
            return;
        }
        if (previousCell.transform.localPosition.y > currentCell.transform.localPosition.y)
        {
            previousCell.ClearFloor();
            currentCell.ClearCeiling();
            return;
        }
    }

    void Update() { }
}


/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;

public class InstantMazeGenerator : MonoBehaviour
{
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;

    [SerializeField]
    private int _mazeHeight;

    [SerializeField]
    private int _seed;

    [SerializeField]
    private bool _useSeed;

    private MazeCell[,,] _mazeGrid;

    private List<MazeCell> _visitedCells;

    private List<MazeCell> _endPoints;



    void Start()
    {

        if (_useSeed)
        {
            Random.InitState(_seed);
        }
        else
        {
            int randomSeed = Random.Range(1, 1000000);
            Random.InitState(randomSeed);

            Debug.Log(randomSeed);
        }
        

        _mazeGrid = new MazeCell[_mazeWidth, _mazeHeight, _mazeDepth];
        _visitedCells = new List<MazeCell>();
        _endPoints = new List<MazeCell>();

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int y = 0; y < _mazeHeight; y++)
            {
                for (int z = 0; z < _mazeDepth; z++)
                {
                    Vector3 localPosition = new Vector3(x, y, z);
                    _mazeGrid[x, y, z] = Instantiate(_mazeCellPrefab, localPosition, Quaternion.identity, transform);
                    _mazeGrid[x, y, z].transform.localPosition = localPosition;
                }
            }
        }

        int centerX = _mazeWidth / 2;
        int centerY = _mazeHeight / 2;

        GenerateMaze(null, _mazeGrid[centerX, centerY, _mazeHeight]);

        GetComponent<NavMeshSurface>().BuildNavMesh();

        Debug.Log("cells: " + _visitedCells.Count);
        Debug.Log("end points: " + _endPoints.Count);
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        _visitedCells.Add(currentCell);

        if (GetUnvisitedCells(currentCell).Count() == 0)
        {
            _endPoints.Add(currentCell);
        }

        new WaitForSeconds(.05f);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);

        

    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();

    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.localPosition.x;
        int y = (int)currentCell.transform.localPosition.y;
        int z = (int)currentCell.transform.localPosition.z;        

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, y, z];

            if (cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, y, z];

            if (cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }

        if (y + 1 < _mazeHeight)
        {
            var cellToAbove = _mazeGrid[x, y + 1, z];

            if (cellToAbove.IsVisited == false)
            {
                yield return cellToAbove;
            }
        }

        if (y - 1 >= 0)
        {
            var cellToBelow = _mazeGrid[x, y - 1, z];

            if (cellToBelow.IsVisited == false)
            {
                yield return cellToBelow;
            }
        }

        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, y, z + 1];

            if (cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }

        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, y, z - 1];

            if (cellToBack.IsVisited == false)
            {
                yield return cellToBack;
            }
        }


    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if(previousCell == null)
        {
            return;
        }

        if (previousCell.transform.localPosition.x < currentCell.transform.localPosition.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }
        if (previousCell.transform.localPosition.x > currentCell.transform.localPosition.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.localPosition.z < currentCell.transform.localPosition.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }
        if (previousCell.transform.localPosition.z > currentCell.transform.localPosition.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }

        if (previousCell.transform.localPosition.y < currentCell.transform.localPosition.y)
        {
            previousCell.ClearCeiling();
            currentCell.ClearFloor();
            return;
        }
        if (previousCell.transform.localPosition.y > currentCell.transform.localPosition.y)
        {
            previousCell.ClearFloor();
            currentCell.ClearCeiling();
            return;
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
*/