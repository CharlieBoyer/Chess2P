using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Managers;
using Data;
using Enums;
using Debug = UnityEngine.Debug;

namespace AI
{
    public class Node
    {
        public int Depth;
        public Node Parent;
        public List<Node> Children;
        public Piece[,] Grid;
        public float HeuristicValue;

        public Coordinates? Origin;
        public Coordinates? Destination;

        public Node(Piece[,] grid, int depth, Coordinates? origin, Coordinates? destination, Node parent = null)
        {
            Grid = Matrix.DuplicateGrid(grid);
            Depth = depth;
            Origin = origin;
            Destination = destination;
            Parent = parent;
            Children = new List<Node>();

            if (Origin != null && Destination != null)
                PerformMove();
        }

        public void GenerateChildren(Side turn)
        {
            Children = new List<Node>();
            List<Piece> sidePieces = Matrix.GetAllPieces(Grid, turn);

            if (Depth >= GameManager.Depth || IsTerminal())
            { 
                return;
            }

            foreach (Piece piece in sidePieces)
            {
                foreach (Coordinates move in piece.AvailableMoves())
                {
                    Node child = new(Grid, Depth + 1, piece.Coordinates, move, this);
                    Children.Add(child);
                }
            }
        }

        private void PerformMove()
        {
            if (!Origin.HasValue || !Destination.HasValue)
                throw new ArgumentException("Trying to PerformMove() with null Coordinate(s) : " + (!Origin.HasValue ? "Origin" : "") + (!Destination.HasValue ? ", Destination" : ""));

            Piece pieceToMove = Grid[Origin.Value.Column, Origin.Value.Row];
            // Piece destination = Grid[Destination.Value.Column, Destination.Value.Row];

            Grid[Destination.Value.Column, Destination.Value.Row] = pieceToMove;
            Grid[Origin.Value.Column, Origin.Value.Row] = null;
        }

        public void EvaluateHeuristics()
        {
            return;
        }

        public bool IsTerminal()
        {
            return false;
        }

        #region Debug

        public static void GenerateNodeTree(Piece[,] grid, int depth, Side startingTurn)
        {
            Debug.Log("> Root node instantiated !");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Node rootNode = new Node(grid, 0, null, null);

            StringBuilder logBuilder = new StringBuilder();
            GenerateNodeTreeRecursive(rootNode, depth, startingTurn, 0, logBuilder);
            
            stopwatch.Stop();
            TimeSpan elapsedTime = stopwatch.Elapsed;
            string formattedElapsedTime = $"{elapsedTime.Minutes:00}m {elapsedTime.Seconds:00}s {elapsedTime.Milliseconds:000}ms";
            Debug.Log($"\n> Test completed in : {formattedElapsedTime}");
            
            Debug.Log(logBuilder.ToString());
        }

        private static void GenerateNodeTreeRecursive(Node node, int maxDepth, Side turn, int currentDepth, StringBuilder logBuilder)
        {
            string indent = new string(' ', currentDepth * 2);
            if (node.Origin == null && node.Destination == null)
                logBuilder.AppendLine($"{indent}{currentDepth}: Root");
            else
                logBuilder.AppendLine($"{indent}{currentDepth}: {node.Origin} -> {node.Destination}");

            if (currentDepth >= maxDepth || node.IsTerminal()) return;
            
            node.GenerateChildren(turn);

            foreach (Node child in node.Children) {
                GenerateNodeTreeRecursive(child, maxDepth, turn == Side.Light ? Side.Dark : Side.Light, currentDepth + 1, logBuilder);
            }
        }

        #endregion
    }

    #region Terminal Checks

    /*
    public bool IsTerminalNode(Node node)
    {
        // Check for terminal game state based on the node's Grid field
        // ...
    }

    private bool IsCheckmate(Node node, Side side)
    {
        Coordinates kingPosition = Matrix.GetKing(Grid, side).Coordinates;

        if (!IsKingInCheck(kingPosition, side)) {
            return false;
        }

        List<Piece> pieces = Matrix.GetAllPieces(Grid, side);

        foreach (Piece piece in pieces) {
            foreach (Coordinates unused in piece.AvailableMoves()) {
                return false; // If there's at least one legal move, it's not checkmate.
            }
        }

        return true; // If the king is in check and there are no legal moves, it's checkmate.
    }

    private bool IsStalemate(Side side)
    {
        List<Piece> pieces = Matrix.GetAllPieces(Grid, side);

        foreach (Piece piece in pieces)
        {
            foreach (Coordinates unused in piece.AvailableMoves())
            {
                // If there's at least one legal move, it's not stalemate.
                return false;
            }
        }

        Coordinates kingPosition = Matrix.GetKing(Grid, side).Coordinates;

        if (IsKingInCheck(kingPosition, side))
        {
            return false;
        }

        // If the king is not in check and there are no legal moves, it's stalemate.
        return true;
    }

    private bool IsKingInCheck(Coordinates kingPosition, Side side)
    {
        List<Piece> opponentPieces = Matrix.GetAllPieces(Grid, side);

        foreach (Piece piece in opponentPieces)
        {
            if (piece.AvailableMoves().Contains(kingPosition))
            {
                return true;
            }
        }

        return false;
    }
    */

    #endregion
}
