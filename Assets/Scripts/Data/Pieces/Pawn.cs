using System.Collections.Generic;
using Enums;

namespace Data.Pieces
{
    public class Pawn : Piece
    {
        public Pawn(Side side, Coordinates coords, Piece[,] reference) : base(side, coords, reference) {}
        
        public Pawn(Pawn copy, Piece[,] reference) : base(copy, reference) {}

        public override float Heuristic
        {
            get
            {
                float baseValue = 1.0f;
                return baseValue;
            }
        }

        public override List<Coordinates> AvailableMoves()
        {
            List<Coordinates> moves = new ();
            int currentColumn = this.Coordinates.Column;
            int currentRow = this.Coordinates.Row;
            int offset = (Side == Side.Light) ? 1 : -1;
            
            Piece forward, forwardLeft, forwardRight, forwardPush;
            
            forward = Matrix.GetPiece(Reference,currentColumn, currentRow + offset);
            forwardLeft = Matrix.GetPiece(Reference,currentColumn - 1, currentRow + offset);
            forwardRight = Matrix.GetPiece(Reference,currentColumn + 1, currentRow + offset);
            forwardPush = Matrix.GetPiece(Reference,currentColumn, currentRow + offset * 2);
            
            if (forwardLeft is not null && forwardLeft.Side != Side)
                moves.Add(forwardLeft.Coordinates);

            if (forwardRight is not null && forwardRight.Side != Side)
                moves.Add(forwardRight.Coordinates);

            if (forward is null && (currentColumn is >= 0 and <= 7 && currentRow is >= 0 and <= 7)) 
                moves.Add(new Coordinates(currentColumn, currentRow + offset));
            else
                return moves;
            
            if (forwardPush is null && !HasMoved)
                moves.Add(new Coordinates(currentColumn, currentRow + offset * 2));

            ValidateMoves(moves);
            return moves;
        }
    }
}
