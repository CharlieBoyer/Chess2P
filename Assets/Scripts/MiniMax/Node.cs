using System.Collections.Generic;
using Data;
using Enums;
using Managers;
using UnityEngine;

namespace MiniMax
{
    public class Node
    { 
        public  Side OpponentTurn => (Owner == Side.Light ? Side.Dark : Side.Light);
        public Side Owner { get; private set; }
        public Piece[,] Matrix { get; private set; }
    
        private bool IsRoot { get; set; }

        public Coordinates Destination { get; private set; }
        public Coordinates Origin { get; private set; }


        public Node(Side owner, Piece[,] matrix, Coordinates origin, Coordinates destination, bool isRoot = false)
        {
            Owner = owner;
            IsRoot = isRoot;
            Matrix = matrix;
            Origin = origin;
            Destination = destination;
        }

        public float GetHeuristicValue()
        {
            float heuristicValue = 0f;
            foreach (Piece piece in Matrix)
            {
                if (piece == null) continue;
                if (piece.Side != GameManager.CurrentPlayerTurn) continue;

                //  heuristicValue = cell.Occupant.Side == _owner ? cell.Occupant.HeuristicScore : -cell.Occupant.HeuristicScore;
                if (piece.Side == Owner)
                    heuristicValue += piece.Heuristic;
                else
                    heuristicValue -= piece.Heuristic;
            }
        
            return heuristicValue;
        }

        public bool IsTerminal()
        {
            return GetChilds().Count == 0;
        }
    
        public List<Node> GetChilds()
        {
            List<Node> nodeList = new List<Node>();
            List<Piece> ownerPiece = Data.Matrix.GetAllPieces(Matrix, (IsRoot ? Owner : OpponentTurn));
        
            foreach (Piece piece in ownerPiece)
            {
                // Je dois récupérer le mouvement de chaque Occupant dont c'est le tour et créer un nouveau node basé sur
                // celui du node actuelle et jouer le coup sur celui ci
                // 1 - Récupérer la liste des muvement possible pour cette matrice
                List<Coordinates> moves = piece.AvailableMoves();
                Debug.Log(moves);
                foreach (Coordinates move in moves)
                {
                    // 2 - Copier la matrice actuel
                    Piece[,] newMatrix = Data.Matrix.DuplicateSnapshot(Matrix);
                
                    // 3 - Créer un Node contenant tout les information nécessaire (la nouvelle matrice, les sides)
                    Node childNode = new Node(IsRoot ? Owner : OpponentTurn, newMatrix, piece.Coordinates, move);

                    // 4 - Jouer le mouvement sur la matrice actuel
                    Data.Matrix.VirtualPerform(childNode);
                
                    // 5 - Ajouter le node a la liste des enfants à retourner
                    nodeList.Add(childNode);
                }
            }
            return nodeList;
        }
    
    
    }
}
