using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.Utilities
{
    public struct Cell
    {
        public Vector3 position; // local position
        public int rawIndex ;
        public int columnIndex;
        public bool open;
    }

    public class LevelGrid : MonoBehaviour
    {
        #region private  variables
        public int rows; // number of raws inside the cell
        public int columns; // number of columns inside the cell
        public float cellWidth; //  width of the cell
        public float cellHeight;//  height of the cell
        #endregion

        #region public  variables
        private Dictionary<int, Cell> _cellsDict; // cells;
        #endregion

        protected void Start()
        {
            _cellsDict = new Dictionary<int, Cell>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Cell cell = new Cell();
                    cell.position = new Vector3(i + cellWidth / 2.0f, 0, j + cellHeight / 2.0f);
                    cell.rawIndex = i;
                    cell.columnIndex = j;
                    cell.open = true;
                    _cellsDict.Add(i * rows + j, cell);
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 1, 1, 0.75F);
            float gridTotalwidth = columns * cellWidth;
            float gridTotalHeight = rows * cellHeight;

            Vector3 origin = transform.position;
            // [] pointA, pointB, pointC, pointD are corners of rectangle
            Vector3 pointA = origin;
            Vector3 pointB = new Vector3(pointA.x + gridTotalwidth, origin.y, origin.z);
            Vector3 pointC = new Vector3(pointB.x, origin.y, pointB.z + gridTotalHeight);
            Vector3 pointD = new Vector3(pointA.x, origin.y, pointA.z + gridTotalHeight);

            Gizmos.DrawLine(pointA, pointB);
            Gizmos.DrawLine(pointB, pointC);
            Gizmos.DrawLine(pointC, pointD);
            Gizmos.DrawLine(pointA, pointD);

            Vector3 pointBottom = pointA;
            Vector3 pointTop = pointD;

            for (int i = 0; i < columns; i++)
            {
                pointBottom = new Vector3(pointBottom.x + cellWidth, pointBottom.y, pointBottom.z);
                pointTop = new Vector3(pointTop.x + cellWidth, pointTop.y, pointTop.z);

                Gizmos.DrawLine(pointBottom, pointTop);
            }

            Vector3 pointLeft = pointA;
            Vector3 pointRight = pointB;
            for (int i = 0; i < rows; i++)
            {
                pointLeft = new Vector3(pointLeft.x, pointLeft.y, pointLeft.z + cellHeight);
                pointRight = new Vector3(pointRight.x, pointRight.y, pointRight.z + cellHeight);
                Gizmos.DrawLine(pointLeft, pointRight);
            }
        }

        public bool IsCellOpenAtLocation(Vector3 position)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            int rowIndex = (int)Mathf.Floor(localPosition.x);
            int columnIndex = (int)Mathf.Floor(localPosition.z);
            int dictIonaryKey = rowIndex * rows + columnIndex;

            if (_cellsDict.ContainsKey(dictIonaryKey))
            {
                Cell cell = _cellsDict[dictIonaryKey];
                if (cell.open) return true;
            }
            return false;
        }

        public Vector3 WorldToCellWorldPos(Vector3 position) // retuns cells centre position in world coordinates
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            int rowIndex = (int)Mathf.Floor(localPosition.x);
            int columnIndex = (int)Mathf.Floor(localPosition.z);
            int dictIonaryKey = rowIndex * rows + columnIndex;

            Vector3 cellWoldPos = new Vector3();
            if (_cellsDict.ContainsKey(dictIonaryKey))
            {
                cellWoldPos = transform.TransformPoint(_cellsDict[dictIonaryKey].position);
            }
            return cellWoldPos;
        }

        public void OpenCellAthisPosition(Vector3 position)
        {
            OpenCloseCell(position, true);
        }

        public void CloseCellAthisPosition(Vector3 position)
        {
            OpenCloseCell(position, false);
        }

        public Vector3 GetClosetOpenPositionAtTarget(Vector3 inputPosition, Vector3 targetPosition) // aipos player pos  
        {
            Cell inputCell = (Cell)WorldPositionToCell(inputPosition);
            Cell targetCell = (Cell)WorldPositionToCell(targetPosition);

            Vector3 inputCellPos = inputCell.position;
            Vector3 targetCellPos = targetCell.position;

            Vector3 direction = inputCellPos - targetCellPos;
            direction.Normalize();
            Cell? expectedCell = null;

            float roteVector = 0.0f;
            Vector3 worlPsitionOFCell = new Vector3();

            while (expectedCell == null) // search unit we find closet  open postion
            {
                direction = Quaternion.AngleAxis(roteVector, Vector3.up) * direction;
                Vector3 expectedCellPos = targetCellPos + direction;
                worlPsitionOFCell = transform.TransformPoint(expectedCellPos);
                expectedCell = WorldPositionToCell(worlPsitionOFCell);

                if (expectedCell != null && !((Cell)(expectedCell)).open) expectedCell = null;
                roteVector = 45.0f;
            }

             return WorldToCellWorldPos(worlPsitionOFCell);
        }

        public bool IsTwoPositionsCloseByOnGrid(Vector3 positinA, Vector3 positinB)
        {
            Vector3 localPosition = transform.InverseTransformPoint(positinA);
            Vector2 cellIndexA = new Vector2((int)Mathf.Floor(localPosition.x), (int)Mathf.Floor(localPosition.z));
            localPosition = transform.InverseTransformPoint(positinB);
            Vector2 cellIndexB = new Vector2((int)Mathf.Floor(localPosition.x), (int)Mathf.Floor(localPosition.z));
            float length = (cellIndexA - cellIndexB).magnitude;

            if(length < 1.5f) return true; // not generic, TODO: li,k wiht cell size
            return false;
        }

        private void OpenCloseCell(Vector3 position, bool open)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            int rowIndex = (int)Mathf.Floor(localPosition.x);
            int columnIndex = (int)Mathf.Floor(localPosition.z);
            int dictIonaryKey = rowIndex * rows + columnIndex;

            if (_cellsDict.ContainsKey(dictIonaryKey))
            {
                Cell cell = _cellsDict[dictIonaryKey];
                cell.open = open;
                _cellsDict[dictIonaryKey] = cell;
            }
            Cell cells = _cellsDict[dictIonaryKey];
        }

        private Cell? WorldPositionToCell(Vector3 Position)
        {
            Vector3 localPosition = transform.InverseTransformPoint(Position);
            int rowIndex = (int)Mathf.Floor(localPosition.x);
            int columnIndex = (int)Mathf.Floor(localPosition.z);
            int dictIonaryKey = rowIndex * rows + columnIndex;

            Cell? cell = null;

            if (_cellsDict.ContainsKey(dictIonaryKey))
            {
                cell = _cellsDict[dictIonaryKey]; 
            }
            return cell;
        }
    }
}
