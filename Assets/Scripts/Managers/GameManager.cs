using UnityEngine;

using View;
using Data;
using Enums;
using AI;

namespace Managers
{
    public class GameManager: MonoBehaviour
    {
        #region Fields

        #region Inspector

            [Header("Settings")]
            public Board Board;
            [Range(1,8)] public int AIDepth;
            [Range(0.001f, 3f)] public float AutoPlayInterval = 2;
            public bool EnableStepByStep;
            
        #endregion
        
        public static int Depth { get; private set; }
        public static Side CurrentPlayerTurn { get; private set; }
        public static Side OpponentTurn => (CurrentPlayerTurn == Side.Light ? Side.Dark : Side.Light);
        public static int TurnCounter { get; private set; }
        
        public bool Checkmate { get; set; }
        public bool Stalemate { get; set; }
        public bool Draw { get; set; }

        private bool _updateOnce;
        private bool _pauseAutoPlay;
        private bool _confirmEscape;
        private float _escapeTimer = 3f;

        #endregion
        
        private void Awake()
        {
            Depth = AIDepth;
            CurrentPlayerTurn = Side.Light;
            TurnCounter = 1;
            Checkmate = false;
            Stalemate = false;
            Draw = false;
        }
        private void Start()
        {
            Node.GenerateNodeTree(Matrix.Grid, AIDepth, Side.Light);
            // StartCoroutine(StartGameLoop());
        }
        
        private void Update()
        {
            /* CheckExitEscape(Input.GetKeyDown(KeyCode.Escape));
            
            if (_updateOnce) {
                UIManager.UpdateTurn(CurrentPlayerTurn);
                _updateOnce = false;
            } */
        }
 
        #region Gameplay
        
        public void PerformMovement(Coordinates origin, Coordinates destination)
        {
            Matrix.Perform(CurrentPlayerTurn, origin, destination);
            Board.UpdateView();
        }

        private void ChangeTurn()
        {
            CurrentPlayerTurn = OpponentTurn;
            TurnCounter++;
        }

        public void PauseAutoPlay()
        {
            _pauseAutoPlay = !_pauseAutoPlay;
        }

        #endregion

        #region AI

        private void AIThink(Side playerSide)
        {
            Node root = new Node(Matrix.Grid, 0, null, null);
            Node bestMoveNode = MinMax(root, Depth, playerSide == CurrentPlayerTurn);
            
            if (bestMoveNode.Origin.HasValue && bestMoveNode.Destination.HasValue)
            {
                Coordinates origin = bestMoveNode.Origin.Value;
                Coordinates destination = bestMoveNode.Destination.Value;

                // Perform the move on the board using the origin and destination coordinates
                // ...
            }
            else
            {
                // Handle the case where the Origin or Destination is null
                // This could indicate an error or a terminal game state (e.g., checkmate or stalemate)
            }
        }

        public Node MinMax(Node node, int depth, bool isMaximizingPlayer)
        {
            if (depth == 0 || node.IsTerminal())
            {
                node.EvaluateHeuristics();
                return node;
            }

            if (isMaximizingPlayer)
            {
                float maxEval = float.MinValue;
                Node bestNode = null;

                foreach (Node child in node.Children)
                {
                    float eval = MinMax(child, depth - 1, false).HeuristicValue;
                    if (eval > maxEval)
                    {
                        maxEval = eval;
                        bestNode = child;
                    }
                }

                return bestNode;
            }
            else
            {
                float minEval = float.MaxValue;
                Node bestNode = null;

                foreach (Node child in node.Children)
                {
                    float eval = MinMax(child, depth - 1, true).HeuristicValue;
                    if (eval < minEval)
                    {
                        minEval = eval;
                        bestNode = child;
                    }
                }

                return bestNode;
            }
        }

        #endregion
        
        #region Utils

        private void CheckExitEscape(bool triggered)
        {
            if (triggered && _confirmEscape)
            {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.ExitPlaymode();
                #endif
                Application.Quit();
            }
                
            if (triggered) {
                _confirmEscape = true;
            }

            if (_escapeTimer <= 0f)
                _escapeTimer = 3f;

            _escapeTimer -= Time.deltaTime;
        }

        #endregion
    }
}
