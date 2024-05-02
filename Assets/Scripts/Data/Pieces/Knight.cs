using System.Collections.Generic;
using System.Linq;
using Enums;

namespace Data.Pieces
{
    public class Knight : Piece
    {
        public Knight(Side side, Coordinates coords, Piece[,] reference) : base(side, coords, reference) {}
        
        public Knight(Knight copy, Piece[,] reference) : base(copy, reference) {}
        
        public override float Heuristic
        {
            get
            {
                float baseValue = 3.20f;
                return baseValue;
            }
        }

        public override List<Coordinates> AvailableMoves()
        {
            List<Coordinates> availableMoves = new ();
            int currentColumn = this.Coordinates.Column;
            int currentRow = this.Coordinates.Row;

            int[] columnsOffsets = { 1, 2,  2,  1, -1, -2, -2, -1 };
            int[] rowOffsets     = { 2, 1, -1, -2, -2, -1,  1,  2 };

            for (int i = 0; i < Matrix.BoardSize; i++)
            {
                int column = currentColumn + columnsOffsets[i];
                int row = currentRow + rowOffsets[i];
                
                availableMoves.Add(new Coordinates(column, row));
            }

            ValidateMoves(availableMoves);
            return availableMoves;
        }

        protected override bool ValidateMoves(List<Coordinates> availableMoves)
        {
            foreach (Coordinates move in availableMoves.ToList())
            {
                if (move.Column is < 0 or > 7 || move.Row is < 0 or > 7) // Exclude out-of-bounds coordinates
                {
                    availableMoves.Remove(move);
                } 
                
                Piece destination = Matrix.GetPiece(Reference, move);
                
                if (destination != null && destination.Side == Side) // Exclude allied pieces
                {
                    availableMoves.Remove(move);
                }
            }

            return true;
        }
    }

}
