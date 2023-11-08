﻿using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Pieces
{
    public class Knight : Piece
    {
        public Knight(Cell cell, GameObject prefab, Transform root, Side side) : base(cell, prefab, root, side) {}

        public override List<Cell> GetAvailableMoves(Cell currentCell)
        {
            List<Cell> availableMoves = new List<Cell>();
            int currentColumn = currentCell.Coordinates.Columns;
            int currentRow = currentCell.Coordinates.Row;

            int[] columnsOffsets = { 1, 2,  2,  1, -1, -2, -2, -1 };
            int[] rowOffsets     = { 2, 1, -1, -2, -2, -1,  1,  2 };

            for (int i = 0; i < Matrix.BoardSize; i++)
            {
                int column = currentColumn + columnsOffsets[i];
                int row = currentRow + rowOffsets[i];

                Cell cell = Matrix.GetCell(column, row);
                
                ValidateCell(availableMoves, cell);
            }

            return availableMoves;
        }

        protected override bool ValidateCell(ICollection<Cell> availableMoves, Cell cell)
        {
            if (cell == null) return false; // Knight will check out-of-board cells, skipping them

            if (!cell.IsOccupied || (cell.IsOccupied && cell.Occupant.Side != Side))
            {
                availableMoves.Add(cell);
                return true;
            }

            return false;
        }
    }
}
