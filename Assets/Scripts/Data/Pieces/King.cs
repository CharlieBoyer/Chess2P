using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

namespace Data.Pieces
{
    public class King : Piece
    {
        public King(Side side, Coordinates coords, Piece[,] reference) : base(side, coords, reference) {}

        public King(King copy, Piece[,] reference) : base(copy, reference) {}

        public override float Heuristic
        {
            get
            {
                float baseValue = Mathf.Infinity;
                return baseValue;
            }
        }

        public override List<Coordinates> AvailableMoves()
        {
            List<Coordinates> availableMoves = new ();
            int currentColumn = this.Coordinates.Column;
            int currentRow = this.Coordinates.Row;

            int[] rowOffsets = { -1, -1, -1,  0, 0,  1, 1, 1 };
            int[] colOffsets = { -1,  0,  1, -1, 1, -1, 0, 1 };

            for (int i = 0; i < Matrix.BoardSize; i++)
            {
                int row = currentRow + rowOffsets[i];
                int column = currentColumn + colOffsets[i];
                
                availableMoves.Add(new Coordinates(column, row));
            }

            ValidateMoves(availableMoves);
            return availableMoves;
        }
        
        protected override void ValidateMoves(List<Coordinates> availableMoves)
        {
            base.ValidateMoves(availableMoves);

            foreach (Coordinates remainingMove in availableMoves.ToList()) // Should avoid moves that self-check
            {
                Piece[,] testGrid = Matrix.DuplicateGrid(Reference);
                Matrix.VirtualPerform(testGrid, Side, this.Coordinates, remainingMove);
                List<Piece> piecesToConfront = Matrix.GetAllPieces(testGrid, Side == Side.Light ? Side.Dark : Side.Light);
                
                foreach (Piece piece in piecesToConfront)
                {
                    foreach (Coordinates move in piece.AvailableMoves())
                    {
                        if (move.Equals(this.Coordinates)) { // King is threaten by this piece
                            availableMoves.Remove(remainingMove);
                            
                            // goto du caca replaced by :
                            /*
                                $$$$$$$\  $$$$$$$$\ $$\      $$\  $$$$$$\   $$$$$$\  $$$$$$$\   $$$$$$\ $$$$$$$$\ $$\     $$\ 
                                $$  __$$\ $$  _____|$$$\    $$$ |$$  __$$\ $$  __$$\ $$  __$$\ $$  __$$\\__$$  __|\$$\   $$  |
                                $$ |  $$ |$$ |      $$$$\  $$$$ |$$ /  $$ |$$ /  \__|$$ |  $$ |$$ /  $$ |  $$ |    \$$\ $$  / 
                                $$ |  $$ |$$$$$\    $$\$$\$$ $$ |$$ |  $$ |$$ |      $$$$$$$  |$$$$$$$$ |  $$ |     \$$$$  /  
                                $$ |  $$ |$$  __|   $$ \$$$  $$ |$$ |  $$ |$$ |      $$  __$$< $$  __$$ |  $$ |      \$$  /   
                                $$ |  $$ |$$ |      $$ |\$  /$$ |$$ |  $$ |$$ |  $$\ $$ |  $$ |$$ |  $$ |  $$ |       $$ |    
                                $$$$$$$  |$$$$$$$$\ $$ | \_/ $$ | $$$$$$  |\$$$$$$  |$$ |  $$ |$$ |  $$ |  $$ |       $$ |    
                                \_______/ \________|\__|     \__| \______/  \______/ \__|  \__|\__|  \__|  \__|       \__|
                             */
                        }
                    }
                }
            }
        }
    }
}
