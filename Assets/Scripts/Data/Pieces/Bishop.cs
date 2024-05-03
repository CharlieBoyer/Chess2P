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
            {
                if (Reference[column, row] != null && Reference[column, row].Side == Side) // If allied, just stop this diagonal check
                    break;
                else if (Reference[column, row] != null && Reference[column, row].Side != Side) { // Do the same but add enemy piece before
                    availableMoves.Add(new Coordinates(column, row));
                    break;
                }
                    
                availableMoves.Add(new Coordinates(column, row)); // Cell is free;
            }
            
            for (int column = currentColumn - 1, row = currentRow + 1; row < Matrix.BoardSize && column >= 0; row++, column--) // Upward-left
            {
                if (Reference[column, row] != null && Reference[column, row].Side == Side)
                    break;
                else if (Reference[column, row] != null && Reference[column, row].Side != Side) {
                    availableMoves.Add(new Coordinates(column, row));
                    break;
                }
                    
                availableMoves.Add(new Coordinates(column, row));
            }

            for (int column = currentColumn + 1, row = currentRow - 1; row >= 0 && column < Matrix.BoardSize; row--, column++) // Downward-right
            {
                if (Reference[column, row] != null && Reference[column, row].Side == Side)
                    break;
                else if (Reference[column, row] != null && Reference[column, row].Side != Side) {
                    availableMoves.Add(new Coordinates(column, row));
                    break;
                }
                    
                availableMoves.Add(new Coordinates(column, row));
            }

            for (int column = currentColumn - 1, row = currentRow - 1; row >= 0 && column >= 0; row--, column--) // Downward-left
            {
                if (Reference[column, row] != null && Reference[column, row].Side == Side)
                    break;
                else if (Reference[column, row] != null && Reference[column, row].Side != Side) {
                    availableMoves.Add(new Coordinates(column, row));
                    break;
                }
                    
                availableMoves.Add(new Coordinates(column, row));
            }

            ValidateMoves(availableMoves);
            
            return availableMoves;
        }
    }
}
