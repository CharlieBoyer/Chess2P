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
        }
        
        private void GetAlignedCells(ICollection<Coordinates> availableMoves, int currentColumn, int currentRow)
        {
            for (int row = currentRow + 1; row < Matrix.BoardSize; row++) // Upward
            { 
                int result = ValidateSingleMove(new Coordinates(currentColumn, row));
                if (result == -1) break; // Show Stopper
                if (result == 0) {
                    availableMoves.Add(new Coordinates(currentColumn, row));
                    break;
                }
                availableMoves.Add(new Coordinates(currentColumn, row));
                
            }

            for (int row = currentRow - 1; row >= 0; row--) // Downward
            {
                int result = ValidateSingleMove(new Coordinates(currentColumn, row));
                if (result == -1) break; // Show Stopper
                if (result == 0) {
                    availableMoves.Add(new Coordinates(currentColumn, row));
                    break;
                }
                availableMoves.Add(new Coordinates(currentColumn, row));
            }

            for (int column = currentColumn + 1; column < Matrix.BoardSize; column++) // Rightward
            {
                int result = ValidateSingleMove(new Coordinates(column, currentRow));
                if (result == -1) break; // Show Stopper
                if (result == 0) {
                    availableMoves.Add(new Coordinates(column, currentRow));
                    break;
                }
                availableMoves.Add(new Coordinates(column, currentRow));
            }

            for (int column = currentColumn - 1; column >= 0; column--) // Leftward
            {
                int result = ValidateSingleMove(new Coordinates(column, currentRow));
                if (result == -1) break; // Show Stopper
                if (result == 0) {
                    availableMoves.Add(new Coordinates(column, currentRow));
                    break;
                }
                availableMoves.Add(new Coordinates(column, currentRow));
            }
        }
        
        protected int ValidateSingleMove(Coordinates coordinates)
        {
            Piece piece = Matrix.GetPiece(this.Reference, coordinates);

            if (piece == null || piece.Coordinates.Column is < 0 or > 7 || piece.Coordinates.Row is < 0 or > 7) return -1;
            if (piece.Side != Side) return 0; 
            
            return -1;
        }
    }
}
