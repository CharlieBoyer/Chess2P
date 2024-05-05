using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using UnityEngine;
using Debug = System.Diagnostics.Debug;

using Data;
using Enums;
using Managers;
using ThreadPriority = System.Threading.ThreadPriority;

namespace AI
{
    public class Node
    {
        public int Depth;
        public Side? MaxingSide;
        public Node Parent;
        public List<Node> Children;
        public Piece[,] Grid;
        public float HeuristicValue;

        public Coordinates? Origin;
        public Coordinates? Destination;

        public Node(Piece[,] grid, int depth, Coordinates? origin, Coordinates? destination, Node parent = null, Side? side = null)
        {
            Grid = Matrix.DuplicateGrid(grid);
            Depth = depth;
            MaxingSide = side;
            Origin = origin;
            Destination = destination;
            Parent = parent;
            Children = new List<Node>();

            if (Origin != null && Destination != null)
                PerformMove();
            
            // HeuristicValue = EvaluateHeuristics();
        }

        public void GenerateChildren()
        {
            Side sideToEvaluate = MaxingSide ?? Side.Light;
            List<Piece> sidePieces = Matrix.GetAllPieces(Grid, sideToEvaluate);
            Children = new List<Node>();

            foreach (Piece piece in sidePieces)
            {
                foreach (Coordinates move in piece.AvailableMoves())
                {
                    Node child = new(Grid, Depth + 1, piece.Coordinates, move, this, sideToEvaluate);
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
            Grid[Destination.Value.Column, Destination.Value.Row].Coordinates = Destination.Value;
            Grid[Origin.Value.Column, Origin.Value.Row] = null;
        }

        public float EvaluateHeuristics()
        {
            switch (GameManager.HeuristicMode)
            {
                case HeuristicMode.PiecesValueSum:
                    return HeuristicValueSum();
                default:
                    throw new Exception("Unexpected HeuristicMode provided");
            }
        }

        public GameState IsTerminal()
        {
            Side currentPlayer = MaxingSide ?? Side.Light;
            List<Piece> currentPlayerPieces = Matrix.GetAllPieces(Grid, currentPlayer);
            List<Coordinates> allMoves = new List<Coordinates>();
            
            bool isInCheck = Matrix.IsInCheck(Grid, currentPlayer);

            foreach (Piece piece in currentPlayerPieces) {
                allMoves.AddRange(piece.AvailableMoves());
            }

            if (isInCheck && allMoves.Count == 0)
                return GameState.Checkmate;
            else if (!isInCheck && allMoves.Count == 0)
                return GameState.Stalemate;
            else
                return GameState.Playing;
        }

        #region Heuristics Calculations

        private float HeuristicValueSum()
        {
            float lightSideScore = 0;
            float darkSideScore = 0;

            for (int column = 0; column < Matrix.BoardSize; column++)
            {
                for (int row = 0; row < Matrix.BoardSize; row++)
                {
                    Piece piece = Grid[column, row];

                    if (piece == null) continue;
                    
                    float pieceValue = piece.Heuristic;

                    if (piece.Side == Side.Light)
                        lightSideScore += pieceValue;
                    else
                        darkSideScore += pieceValue;
                }
            }

            // If it's the light side's turn to maximize, return the difference in favor of the light side
            // Otherwise, return the difference in favor of the dark side
            return (MaxingSide == Side.Light ? lightSideScore - darkSideScore : darkSideScore - lightSideScore);
        }

        #endregion

        #region Test

        public static IEnumerator RunNodeGenerationCoroutine(Piece[,] grid, int depth)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            // Start the node generation on a separate thread
            var thread = new Thread(() =>
            {
                Node rootNode = new Node(grid, 0, null, null);
                GenerateNodesRecursively(rootNode, depth, 0);
            });

            thread.Priority = ThreadPriority.Highest;
            thread.Start();

            while (thread.IsAlive)
                yield return null;
            
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Process completed in: {stopwatch.Elapsed.Minutes}m {stopwatch.Elapsed.Seconds}s {stopwatch.Elapsed.Milliseconds}ms");
        }

        private static void GenerateNodesRecursively(Node node, int maxDepth, int currentDepth)
        {
            string indent = new string(' ', 4 * currentDepth);
            if (currentDepth <= maxDepth)
            {
                // Debug.WriteLine(currentDepth == 0 ? "0 Root node" : $"Depth {currentDepth}: {node.Origin} -> {node.Destination}");
                Debug.WriteLine(currentDepth == 0 ? "0 Root node" : $"{indent}Depth {currentDepth}: {node.Origin} -> {node.Destination}");
                node.GenerateChildren();

                foreach (Node child in node.Children)
                    GenerateNodesRecursively(child, maxDepth, currentDepth + 1);
            }
        }

        #endregion
    }
}
