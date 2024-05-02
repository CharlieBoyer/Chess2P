using System.Collections.Generic;
using Enums;

namespace Data.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(Side side, Coordinates coords, Piece[,] reference) : base(side, coords, reference) {}

        public Bishop(Bishop copy, Piece[,] reference) : base(copy, reference) {}

        public override float Heuristic
        {
            get
            {
                float baseValue = 3.30f;
                return baseValue;
            }
        }

        public override List<Coordinates> AvailableMoves()
        {
            List<Coordinates> availableMoves = new ();
            int currentColumn = this.Coordinates.Column;
            int currentRow = this.Coordinates.Row;
            
            for (int column = currentColumn + 1, row = currentRow + 1; row < Matrix.BoardSize && column < Matrix.BoardSize; row++, column++) // Upward-right
                availableMoves.Add(new Coordinates(column, row));
            
            for (int column = currentColumn - 1, row = currentRow + 1; row < Matrix.BoardSize && column >= 0; row++, column--) // Upward-left
                availableMoves.Add(new Coordinates(column, row));

            for (int column = currentColumn + 1, row = currentRow - 1; row >= 0 && column < Matrix.BoardSize; row--, column++) // Downward-right
                availableMoves.Add(new Coordinates(column, row));

            for (int column = currentColumn - 1, row = currentRow - 1; row >= 0 && column >= 0; row--, column--) // Downward-left
                availableMoves.Add(new Coordinates(column, row));

            ValidateMoves(availableMoves);
            
            return availableMoves;
        }
    }
}
