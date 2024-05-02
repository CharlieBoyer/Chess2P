using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Data
{
    public struct Coordinates: IEquatable<Coordinates>
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Vector3 World => new Vector3(Column, 0f, Row);
    
        public Coordinates(int column, int row)
        {
            Row = row;
            Column = column;
        }
        
        [CanBeNull]
        public override string ToString()
        {
            string column = $"{(char)(Column + 65)}";
            string row = $"{Row + 1}";
            
            return column + row;
        }

        public bool Equals(Coordinates other)
        {
            return Row == other.Row && Column == other.Column;
        }

        public override bool Equals(object obj)
        {
            return obj is Coordinates other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }
    }
}
