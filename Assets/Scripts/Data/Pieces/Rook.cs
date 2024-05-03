using System.Collections.Generic;
using Enums;

namespace Data.Pieces
{
    public class Rook : Piece
    {
        public Rook(Side side, Coordinates coords, Piece[,] reference) : base(side, coords, reference) {}
        
        public Rook(Rook copy, Piece[,] reference) : base(copy, reference) {}
        
        public override float Heuristic
        {
            get
            {
                float baseValue = 5.10f;
                return baseValue;
            }
        }

        public override List<Coordinates> AvailableMoves()
        {
            List<Coordinates> availableMoves = new ();
            int currentColumn = this.Coordinates.Column;
            int currentRow = this.Coordinates.Row;

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
            return availableMoves;
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
