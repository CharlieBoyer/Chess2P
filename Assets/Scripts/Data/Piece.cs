using System;
using System.Collections.Generic;
using System.Linq;
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

        public string Type => GetType().Name;
        public bool IsTheKing => GetType() == typeof(King);

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
            Coordinates = copy.Coordinates;
            Side = copy.Side;
            HasMoved = copy.HasMoved;
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
        
        protected virtual void ValidateMoves(List<Coordinates> availableMoves)
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
            if (obj.GetType() != GetType()) return false;
            return Equals((Piece)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Heuristic);
        }

        #endregion
    }
}
