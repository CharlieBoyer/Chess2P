using System.Collections.Generic;
using Enums;

namespace Data.Pieces
{
    public class Queen : Piece
    {
        public Queen(Side side, Coordinates coords, Piece[,] reference) : base(side, coords, reference) {}
        
        public Queen(Queen copy, Piece[,] reference) : base(copy, reference) {}
        
        public override float Heuristic
        {
            get
            {
                float baseValue = 8.80f;
                return baseValue;
            }
        }

        public override List<Coordinates> AvailableMoves()
        {
            List<Coordinates> availableMoves = new ();
            int currentColumn = this.Coordinates.Column;
            int currentRow = this.Coordinates.Row;
            
            GetAlignedCells(availableMoves, currentColumn, currentRow);
            GetDiagonalCells(availableMoves, currentColumn, currentRow);

            ValidateMoves(availableMoves);
            return availableMoves;
        }
        
        private void GetDiagonalCells(ICollection<Coordinates> availableMoves, int currentColumn, int currentRow)
        {
            for (int column = currentColumn + 1, row = currentRow + 1; row < Matrix.BoardSize && column < Matrix.BoardSize; row++, column++) // Upward-right
                availableMoves.Add(new Coordinates(column, row));
            
            for (int column = currentColumn - 1, row = currentRow + 1; row < Matrix.BoardSize && column >= 0; row++, column--) // Upward-left
                availableMoves.Add(new Coordinates(column, row));

            for (int column = currentColumn + 1, row = currentRow - 1; row >= 0 && column < Matrix.BoardSize; row--, column++) // Downward-right
                availableMoves.Add(new Coordinates(column, row));

            for (int column = currentColumn - 1, row = currentRow - 1; row >= 0 && column >= 0; row--, column--) // Downward-left
                availableMoves.Add(new Coordinates(column, row));
        }
        
        private void GetAlignedCells(ICollection<Coordinates> availableMoves, int currentColumn, int currentRow)
        {
            for (int i = currentRow + 1; i < Matrix.BoardSize; i++) // Upward
                availableMoves.Add(new Coordinates(currentColumn, currentRow));
            
            for (int i = currentRow - 1; i >= 0; i--) // Downward
                availableMoves.Add(new Coordinates(currentColumn, currentRow));
            
            for (int i = currentColumn + 1; i < Matrix.BoardSize; i++) // Rightward
                availableMoves.Add(new Coordinates(currentColumn, currentRow));

            for (int i = currentColumn - 1; i >= 0; i--) // Leftward
                availableMoves.Add(new Coordinates(currentColumn, currentRow));
        }
    }
}
