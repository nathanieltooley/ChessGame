using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Scripts.ChessBoard.Controllers
{
    public static class FEN
    {

        public static void Encrypt(BoardTile[,] pieces)
        {

        }

        public static BoardTile[,] Decrypt(string fenString)
        {
            BoardTile[,] board = new BoardTile[8,8];

            int rank = 0;
            int file = 0;

            int emptyCount = 0;
            bool checkNextFile = false;

            int cursor = 0;

            while (true) 
            {
                if (cursor >= fenString.Length)
                {
                    break;
                }

                char c = fenString[cursor];

                if (emptyCount > 0)
                {
                    board[rank, file] = BoardTile.BuildEmptyTile(new BoardPos(rank, file));
                    emptyCount--;

                    file++;

                    if (file >= 8)
                    {
                        file = 0;
                    }

                    continue;
                }

                if (c == '/')
                {
                    int tempFile = file;
                    for (int j = tempFile; j < 8; j++)
                    {
                        board[rank, tempFile] = BoardTile.BuildEmptyTile(new BoardPos(rank, tempFile));
                    }

                    rank++;
                    file = 0;
                    cursor++;
                    continue;
                }

                if (char.IsNumber(c))
                {
                    emptyCount = int.Parse($"{c}");
                    cursor++;
                    continue;
                }

                if (char.IsAscii(c))
                {
                    ChessColor color = GetColorFromChar(c);
                    ChessPieceId pieceId = GetPieceIdFromChar(c);

                    BoardTile newTile = BoardTile.BuildEmptyTile(new BoardPos(rank, file));

                    newTile.PieceId = pieceId;
                    newTile.PieceColor = color;

                    board[rank, file] = newTile;
                    cursor++;
                }

                file++;
            }

            return board;
        }

        private static ChessColor GetColorFromChar(char c)
        {
            if (char.IsLower(c))
            {
                return ChessColor.Black;
            } else
            {
                return ChessColor.White;
            }
        }

        private static ChessPieceId GetPieceIdFromChar(char c)
        {
            char lower = char.ToLower(c);
            Dictionary<char, ChessPieceId> charToPieceMap = new Dictionary<char, ChessPieceId>
            {
                { 'p', ChessPieceId.Pawn },
                { 'r', ChessPieceId.Rook },
                { 'b', ChessPieceId.Bishop },
                { 'n', ChessPieceId.Knight },
                { 'q', ChessPieceId.Queen },
                { 'k', ChessPieceId.King }
            };

            ChessPieceId pieceId; 
            charToPieceMap.TryGetValue(lower, out pieceId);

            return pieceId;
        }
    }
}
