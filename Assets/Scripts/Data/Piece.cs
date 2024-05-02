using System;
using System.Collections.Generic;

using Enums;
using Data.Pieces;

namespace Data
{
    public abstract class Piece : IEquatable<Piece>, ICloneable
    {
        public Piece[,] Reference { get; private set; }
        public Coordinates Coordinates { get; set; }
        public Side Side { get; set; }
        public bool HasMoved { get; set; }
        

        public abstract float Heuristic { get; }

        public string Type => this.GetType().Name;
        public bool IsTheKing => this.GetType() == typeof(King);
        public bool IsNotTheKing => this.GetType() != typeof(King);
        
        protected Piece(Side side, Coordinates coordinates, Piece[,] reference)
        {
            Coordinates = coordinates;
            Reference = reference;
            Side = side;
            HasMoved = false;
        }

        protected Piece(Piece copy, Piece[,] reference)
        {
            Reference = reference;
            this.Coordinates = copy.Coordinates;
            this.Side = copy.Side;
            this.HasMoved = copy.HasMoved;
        }

        public static Piece Create(string prefabName, Coordinates coordinates)
        {
            return prefabName switch
            {
                "LightPawn"   => new Pawn   (Side.Light, coordinates, Matrix.Grid),
                "LightRook"   => new Rook   (Side.Light, coordinates, Matrix.Grid),
                "LightKnight" => new Knight (Side.Light, coordinates, Matrix.Grid),
                "LightBishop" => new Bishop (Side.Light, coordinates, Matrix.Grid),
                "LightQueen"  => new Queen  (Side.Light, coordinates, Matrix.Grid),
                "LightKing"   => new King   (Side.Light, coordinates, Matrix.Grid),
                "DarkPawn"    => new Pawn   (Side.Dark, coordinates, Matrix.Grid),
                "DarkRook"    => new Rook   (Side.Dark, coordinates, Matrix.Grid),
                "DarkKnight"  => new Knight (Side.Dark, coordinates, Matrix.Grid),
                "DarkBishop"  => new Bishop (Side.Dark, coordinates, Matrix.Grid),
                "DarkQueen"   => new Queen  (Side.Dark, coordinates, Matrix.Grid),
                "DarkKing"    => new King   (Side.Dark, coordinates, Matrix.Grid),
                
                _ => throw new ArgumentOutOfRangeException(prefabName, "Invalid piece name provided for Creation")
            };
        }
        
        public abstract List<Coordinates> AvailableMoves();
        
        protected virtual bool ValidateMoves(List<Coordinates> availableMoves)
        {
            int availableMoveSize = availableMoves.Count;
            
            for (int index = 0; index < availableMoveSize; index++) // Foreach valid moves detected, double-check the validity.
            {
                Coordinates move = availableMoves[index];
                
                if (move.Column is < 0 or > 7 || move.Row is < 0 or > 7) { // Filters-out falty moves outside the board and skip;
                    availableMoves.Remove(move);
                    availableMoveSize = availableMoves.Count;
                    continue;
                }
                
                Piece destinationPiece = Matrix.GetPiece(move); // Here move should be always valid (inside the board bounds).

                if (destinationPiece is not null) // If a piece exist at the provided coords...
                {
                    if (destinationPiece.Side == Side || destinationPiece.IsTheKing) {
                        availableMoves.Remove(move); // ...exclude it, if it's a allied piece.
                        availableMoveSize = availableMoves.Count;
                    }
                    continue; // Skip to the next move;
                }
            
                // At this point, the move should still be valid as the piece reference a empty cell (null)
            }

            return true;
        }

        protected virtual int ValidateSingleMove(Coordinates coordinates)
        {
            return 0;
        }
        

        #region Equality and Copy

        public object Clone()
        {
            return MemberwiseClone();
        }

        public bool Equals(Piece other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Coordinates.Equals(other.Coordinates) && Side == other.Side && HasMoved == other.HasMoved && Heuristic.Equals(other.Heuristic);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Piece)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Heuristic);
        }

        #endregion
    }
}
