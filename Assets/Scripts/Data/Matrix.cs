using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using View;
using AI;
using Enums;
using Node = MiniMax.Node;

namespace Data
{
    public static class Matrix
    {
        public const int BoardSize = 8;
        public static readonly Piece[,] Grid = new Piece[BoardSize, BoardSize];

        public static void Init()
        {
            string[] pieceOrder = { "Rook", "Knight", "Bishop", "Queen", "King", "Bishop", "Knight", "Rook" };

            for (int column = 0; column < Matrix.BoardSize; column++)
            {
                Grid[column, 0] = Piece.Create("Light" + pieceOrder[column], new Coordinates(column, 0));
                Grid[column, 1] = Piece.Create("LightPawn", new Coordinates(column, 1));
                Grid[column, 7] = Piece.Create("Dark" + pieceOrder[column], new Coordinates(column, 7));
                Grid[column, 6] = Piece.Create("DarkPawn", new Coordinates(column, 6));
            }
        }

        #region GetPiece()

        public static Piece GetPiece(Coordinates coordinates)
        {
            return GetPiece(coordinates.Column, coordinates.Row);
        }
        
        public static Piece GetPiece(Piece[,] grid, Coordinates coordinates)
        {
            return GetPiece(grid, coordinates.Column, coordinates.Row);
        }
    
        /// <summary>
        /// Request a unique Piece from the original Grid array tied to the chess board
        /// </summary>
        /// <param name="column">Coordinates component between 0 and 7</param>
        /// <param name="row">Coordinates component between 0 and 7</param>
        /// <returns></returns>
        public static Piece GetPiece(int column, int row)
        {
            if (column is < 0 or > 7) throw new IndexOutOfRangeException("Try to access a piece outside the board");
            if (row is < 0 or > 7) throw new IndexOutOfRangeException("Try to access a piece outside the board");
        
            return Grid[column, row];
        }

        /// <summary>
        /// Request a unique Piece from the supplied "snapshot" matrix
        /// </summary>
        /// <param name="grid">The grid to request</param>
        /// <param name="column">Coordinates component between 0 and 7</param>
        /// <param name="row">Coordinates component between 0 and 7</param>
        /// <returns></returns>
        public static Piece GetPiece(Piece[,] grid, int column, int row)
        {
            if (column is < 0 or > 7) return null; //throw new IndexOutOfRangeException("Try to access a piece outside the board");
            if (row is < 0 or > 7)
                return null;//throw new IndexOutOfRangeException("Try to access a piece outside the board");
        
            return grid[column, row];
        }
        
        #endregion

        #region GetAllPieces()

        /// <summary>
        /// Return all Pieces objects from the original Grid as a continous list.<br/>
        /// <b>Notice:</b> Doesn't filter empty Pieces
        /// </summary>
        /// <returns></returns>
        public static List<Piece> GetAllPieces()
        {
            List<Piece> allPieces = new();
        
            for (int column = 0; column < BoardSize; column++)
            {
                for (int row = 0; row < BoardSize; row++)
                {
                    allPieces.Add(Grid[column, row]);
                }
            }

            return allPieces;
        }
    
        /// <summary>
        /// Return all Pieces objects from the "snapshot" grid as a continous list.<br/>
        /// <b>Notice:</b> Doesn't filter empty Pieces
        /// </summary>
        /// <returns></returns>
        public static List<Piece> GetAllPieces(Piece[,] grid)
        {
            List<Piece> allPieces = new();
        
            for (int column = 0; column < BoardSize; column++)
            {
                for (int row = 0; row < BoardSize; row++)
                {
                    allPieces.Add(Grid[column, row]);
                }
            }

            return allPieces;
        }
        
        /// <summary>
        /// Request all Pieces from the original Grid and filters in Pieces from a specified "side".
        /// </summary>
        /// <returns></returns>
        public static List<Piece> GetAllPieces(Side side)
        {
            List<Piece> pieces = new();
        
            for (int column = 0; column < BoardSize; column++)
            {
                for (int row = 0; row < BoardSize; row++)
                {
                    Piece piece = Grid[column, row];
                    if (piece != null && piece.Side == side)
                        pieces.Add(piece);
                }
            }

            return pieces;
        }
    
        /// <summary>
        /// Request all Pieces from the specified "snapshot" grid and filter in Pieces from a specified "side".
        /// </summary>
        /// <returns></returns>
        public static List<Piece> GetAllPieces(Piece[,] grid, Side side)
        {
            List<Piece> pieces = new();
        
            for (int column = 0; column < BoardSize; column++)
            {
                for (int row = 0; row < BoardSize; row++)
                {
                    Piece piece = grid[column, row];
                    
                    if (piece != null && piece.Side == side)
                        pieces.Add(piece);
                }
            }

            return pieces;
        }


        #endregion

        #region GetKing()

        /// <summary>
        /// Utitlity method to request only the King from the supplied "side" inside the original Grid 
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static Piece GetKing(Side side)
        {
            Piece piece;
        
            for (int column = 0; column < BoardSize; column++)
            {
                for (int row = 0; row < BoardSize; row++)
                {
                    piece = Grid[column, row];
                    if (piece is { IsTheKing: true } && piece.Side == side)
                        return piece;
                }
            }

            throw new NullReferenceException($"Error: Unable to get the {side.ToString()} King. A piece may have bypassed all safeguard and took the King out");
        }

        /// <summary>
        /// Utitlity method to request only the King from the supplied "side" inside a specific "snapshot" grid 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static Piece GetKing(Piece[,] grid, Side side)
        {
            List<Piece> pieces = GetAllPieces(grid, side);

            try
            {
                return pieces.First(piece => piece is not null && piece.IsTheKing);
            }
            catch
            {
                UnityEngine.Debug.LogError($"Error: Unable to get the {side.ToString()} King. A piece may have bypassed all safeguard and took the King out");
                throw;
            }
        }

        #endregion

        public static Coordinates GetCoordsByName(string cellName)
        {
            char columnLetter = cellName[0];
            int row = int.Parse(cellName[1..]) - 1;
            int column = columnLetter - 'A';

            if (column is < 0 or > 7) throw new Exception($"Cell {cellName} doesn't exist.");
            if (row is < 0 or > 7) throw new Exception($"Cell {cellName} doesn't exist.");

            return new Coordinates(column, row);
        }

        public static List<Coordinates> GetMoves(Piece piece)
        {
            return piece.AvailableMoves();
        }

        public static void Perform(Side player, Coordinates originCoords, Coordinates destinationCoords)
        {
            Piece origin = Grid[originCoords.Column, originCoords.Row];
            Piece destination = Grid[destinationCoords.Column, destinationCoords.Row];
            
            if (origin == null || origin.Side != player)
                throw new ArgumentException("Unexpected origin while Perfom(): origin can't be empty or from the opponent side");
            if (destination is not null && destination.Equals(origin))
                throw new ArgumentException("Unexpected destination while Perform(): destination can't be equals to origin.");
            if (destination is not null && destination.Side == origin.Side)
                throw new ArgumentException("Unexpected destination while Perform(): destination can't be an allied piece.");
            
            Grid[destinationCoords.Column, destinationCoords.Row] = origin;
            Grid[destinationCoords.Column, destinationCoords.Row].Coordinates = destinationCoords;
            Grid[originCoords.Column, originCoords.Row] = null;
        }
        
        public static void VirtualPerform(Piece[,] matrixCopy, Side player, Coordinates originCoords, Coordinates destinationCoords)
        {
            Piece origin = matrixCopy[originCoords.Column, originCoords.Row];
            Piece destination = matrixCopy[destinationCoords.Column, destinationCoords.Row];
            
            if (origin == null || origin.Side != player)
                throw new ArgumentException("Unexpected origin while Perfom(): origin can't be empty or from the opponent side");
            if (destination is not null && destination.Equals(origin))
                throw new ArgumentException("Unexpected destination while Perform(): destination can't be equals to origin.");
            if (destination is not null && destination.Side == origin.Side)
                throw new ArgumentException("Unexpected destination while Perform(): destination can't be an allied piece.");
            
            matrixCopy[destinationCoords.Column, destinationCoords.Row] = origin;
            matrixCopy[destinationCoords.Column, destinationCoords.Row].Coordinates = destinationCoords;
            matrixCopy[originCoords.Column, originCoords.Row] = null;
        }

        public static bool IsInCheck(Piece[,] grid, Side sideToCheck)
        {
            Coordinates kingCoords = GetKing(grid, sideToCheck).Coordinates;
            Side opponent = sideToCheck == Side.Light ? Side.Dark : Side.Light;
            List<Piece> opponentPieces = GetAllPieces(grid, opponent);
            
            foreach (Piece opponentPiece in opponentPieces)
            {
                // Check if the opponent's piece can move to the king's position
                if (opponentPiece.AvailableMoves().Contains(kingCoords))
                {
                    return true;
                }
            }

            return false;
        }
        
        public static Piece[,] GetCurrentGridSnapshot() // Deep Copy
        {
            Piece[,] snapshot = new Piece[BoardSize, BoardSize];

            for (int column = 0; column < BoardSize; column++)
            {
                for (int row = 0; row < BoardSize; row++)
                {
                    if (Grid[column, row] == null) {
                        snapshot[column, row] = null;
                        continue;
                    }
                    
                    Type originalPieceType = Grid[column, row].GetType();
                    ConstructorInfo copyCtor = originalPieceType.GetConstructor(new [] { originalPieceType, typeof(Piece[,])});
    
                    if (copyCtor != null)
                        snapshot[column, row] = (Piece)copyCtor.Invoke(new object[] { Grid[column, row], snapshot });
                    else
                        throw new NullReferenceException($"Unable to copy Grid[${column},${row}] of type ${originalPieceType}");
                }
            }

            return snapshot;
        }

        public static Piece[,] DuplicateGrid(Piece[,] snapshot)
        {
            Piece[,] duplicate = new Piece[BoardSize, BoardSize];

            for (int column = 0; column < BoardSize; column++)
            {
                for (int row = 0; row < BoardSize; row++)
                {
                    if (snapshot[column, row] == null) {
                        duplicate[column, row] = null;
                        continue;
                    }
                    
                    Type originalPieceType = snapshot[column, row].GetType();
                    ConstructorInfo copyCtor = originalPieceType.GetConstructor(new [] { originalPieceType, typeof(Piece[,])});
    
                    if (copyCtor != null)
                        duplicate[column, row] = (Piece)copyCtor.Invoke(new object[] { snapshot[column, row], duplicate });
                    else
                        throw new NullReferenceException($"Unable to copy Grid[${column},${row}] of type ${originalPieceType}");
                }
            }

            return duplicate;
        }
        
        #region Debug

        /// <summary>
        /// 
        /// </summary>
        public static void Debug()
        {
            string debug = "Debug Matrix :\n\n";
            for (int column = 0; column < BoardSize; column++)
            {
                string rowPieces = "";
                for (int row = 0; row < BoardSize; row++)
                {
                    rowPieces += Board.GetUnitBehaviour(GetPiece(column, row).Coordinates).name + " ";
                }
                
                debug += rowPieces + "\n";
            }
            UnityEngine.Debug.Log(debug);
        }

        #endregion
        
    }
}
