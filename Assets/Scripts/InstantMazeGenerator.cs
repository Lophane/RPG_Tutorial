using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    private MazeCell[,,] _mazeGrid;

    

    void Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeHeight, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int y = 0; y < _mazeHeight; y++)
            {
                for (int z = 0; z < _mazeDepth; z++)
                {
                    Vector3 position = new Vector3(x, y, z);
                    _mazeGrid[x, y, z] = Instantiate(_mazeCellPrefab, position, Quaternion.identity);
                }
            }
        }

        int centerX = _mazeWidth / 2;
        int centerY = _mazeHeight / 2;

        GenerateMaze(null, _mazeGrid[centerX, centerY, _mazeHeight]);
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

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
        int x = (int)currentCell.transform.position.x;
        int y = (int)currentCell.transform.position.y;
        int z = (int)currentCell.transform.position.z;        

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

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }
        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }
        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }

        if (previousCell.transform.position.y < currentCell.transform.position.y)
        {
            previousCell.ClearCeiling();
            currentCell.ClearFloor();
            return;
        }
        if (previousCell.transform.position.y > currentCell.transform.position.y)
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
