using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using MonoBehaviours;
using Helpers;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static Side CurrentPlayerTurn { get; private set; }
        public static Side OpponentTurn => (CurrentPlayerTurn == Side.Light ? Side.Dark : Side.Light);
        public static bool Checkmate { get; set; }
        public static bool Check { get; set; }
        
        private static Cell _origin;
        private static Cell _destination;
        private static List<Cell> _moves;
        private static bool _showedOnce;

        private static bool _inCheck = false;
        private static List<Cell> _checkResponsibles = new ();
        private static List<Cell> _invalidCellsForKing = new ();
        private static List<Cell> _validEscapeMoves = new();
        private static List<Cell> _validEscapeDestinations = new ();

        private bool _confirmEscape;
        private float _escapeTimer = 3f;

        #region Interactions
        
            private void Awake()
            {
                CurrentPlayerTurn = Side.Light;
                Checkmate = false;
            }

            private void Update()
            {      
                if (!_showedOnce) {
                    Debug.Log($"Player Turn: {CurrentPlayerTurn.ToString()}");
                    _showedOnce = true;
                }
                
                if (Input.GetButtonDown("Fire2"))
                    Unselect();

                EmergencyEscape(Input.GetKeyDown(KeyCode.Escape));
            }

            private void EmergencyEscape(bool triggered)
            {
                if (triggered && _confirmEscape)
                {
                    EditorApplication.ExitPlaymode();
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

        #region Game Logic
        
            #region Cell Selection
            
                public static void SelectCell(Cell cell)
                {
                    if (Check)
                        CheckSelection(cell);
                    else
                        RegularSelection(cell);
                }

                private static void CheckSelection(Cell cell)
                {
                    if (_origin == null && _validEscapeMoves.Contains(cell)) // If no piece have been selected, checks if the current selected can be played;
                        SetOrigin(cell);
                    else if (_origin != null && _validEscapeDestinations.Contains(cell)) // If a piece have been selected to be played, verify if the destination is in the list that covers the check;
                        SetDestination(cell);
                    else
                    {
                        Debug.Log($"{CurrentPlayerTurn.ToString()} King is in Check. No moves from this piece can escape it.");
                        Unselect();
                    }
                }

                private static void RegularSelection(Cell cell)
                {
                    if (_origin == null)
                    {
                        SetOrigin(cell);
                    }
                    else if (_origin != null && IsDifferentPieceSelected(cell))
                    {
                        Unselect();
                        SelectCell(cell); // Tail Recursion
                    }
                    else
                    {
                        SetDestination(cell);
                    }
                }
                
                private static void Unselect()
                {
                    if (_origin == null) return;
                    
                    _origin.Occupant.Behaviour.Highlight(false);
                    _origin.Occupant.Behaviour.HighlightError(false);
                    _origin = null;

                    if (_moves == null) return;

                    foreach (Cell cell in _moves)
                        cell.Behaviour.Highlight(false, CellBehaviour.defaultColor);
                    _moves = null;
                }
            
            #endregion

            private static void ResolveMovement()
            {
                _origin.Occupant.Behaviour.Highlight(false);
                
                if (_destination is { IsOccupied: true } && _destination.Occupant.Side == OpponentTurn)
                    Destroy(_destination.Occupant.Behaviour.gameObject);

                _destination.Occupant = _origin.Occupant;
                _destination.Occupant.HasMoved = true;
                _origin.Occupant = null;
                
                Board.UpdateView();
                ChangeTurn();
            }

            private static void ChangeTurn()
            {
                _showedOnce = false;
                _destination = null;
                _origin = null;

                foreach (Cell cell in _moves) // Reset Cells highlighting
                    cell.Behaviour.Highlight(false, CellBehaviour.defaultColor);
                _moves = null;
                
                CurrentPlayerTurn = OpponentTurn;
                Check = CurrentPlayerIsInCheck();
            }

        #endregion

        #region Targets

            private static void SetOrigin(Cell cell)
            {
                if (!cell.IsOccupied) return;
                if (cell.Occupant.Side != CurrentPlayerTurn) {
                    Debug.Log("You can't play an Opponent piece !");
                    return;
                }
                    
                _origin = cell;
                _origin.Occupant.Behaviour.Highlight(true);
                    
                _moves = Matrix.GetMoves(_origin);
                Board.EnableCellsTargets(_moves);
            }
            
            private static void SetDestination(Cell cell)
            {
                if (!_moves.Contains(cell)) return;
                    
                _destination = cell;
                ResolveMovement();
            }

        #endregion
        
        #region Checks handling

            private static bool CurrentPlayerIsInCheck()
            {
                Cell king = Matrix.GetKing(CurrentPlayerTurn);
                List<Cell> opponentPieces = Matrix.GetPieceCells(OpponentTurn);

                ClearChecks();

                foreach (Cell opponentCell in opponentPieces) // For each opponent pieces
                {
                    List<Cell> possibleMoves = opponentCell.Occupant.GetAvailableMoves(opponentCell);

                    foreach (Cell target in possibleMoves) // For each possibility
                    {
                        if (target == king) // If one of them threaten the King
                        {
                            Debug.Log($">>> >>> {opponentCell.Occupant.Side} {opponentCell.Occupant.GetType()} make the {king.Occupant.Side} {king.Occupant.GetType()} in check, tracking down...");
                            _checkResponsibles.Add(opponentCell); // Track the piece
                            break;
                        }
                    }

                    if (_checkResponsibles.Count > 0) // If any opponent piece threaten the King
                    {
                        Debug.Log($"Found {_checkResponsibles.Count} possible checks");
                        foreach (Cell responsible in _checkResponsibles) // For each King's threats
                        {
                            foreach (Cell cell in responsible.Occupant.GetAvailableMoves(responsible))  
                            {
                                cell.Behaviour.Highlight(true, Utility.PieceCheckWarning); // Highlight the threat line(s) 
                                _invalidCellsForKing.Add(cell); // and track cells as invalid to play. Use later to extract possible moves for the King
                            }
                        }

                        return true;
                    }
                }

                Debug.Log("No checks found...");
                return false;
            }
            
            private static void ClearChecks()
            {
                foreach (Cell cell in _checkResponsibles)
                {
                    cell.Behaviour.Highlight(false, CellBehaviour.defaultColor);
                }
                
                _checkResponsibles.Clear();
                _invalidCellsForKing.Clear();
            }
            
            private static List<Cell> GetValidEscapeCells()
            {
                throw new NotImplementedException();
            }

        #endregion

        #region Utils

            private static bool IsDifferentPieceSelected(Cell cell) {
                return cell.IsOccupied && cell.Occupant.Side == CurrentPlayerTurn;
            }

            public static void ShowNoMovesAvailable() {
                _origin.Occupant.Behaviour.HighlightError(true);
            }

        #endregion
    }
}
