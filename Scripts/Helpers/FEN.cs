﻿using ChessGame.Scripts.DataTypes;
using System.Collections.Generic;

namespace ChessGame.Scripts.Helpers
{
    public static class FEN
    {

        private static Dictionary<ChessPieceId, char> pieceIdChars = new Dictionary<ChessPieceId, char>
        {
            { ChessPieceId.Pawn, 'p' },
            { ChessPieceId.Rook, 'r' },
            { ChessPieceId.Bishop, 'b' },
            { ChessPieceId.Knight, 'n' },
            { ChessPieceId.Queen, 'q' },
            { ChessPieceId.King, 'k' }
        };

        private static Dictionary<char, ChessPieceId> charToPieceMap = new Dictionary<char, ChessPieceId>
        {
            { 'p', ChessPieceId.Pawn },
            { 'r', ChessPieceId.Rook },
            { 'b', ChessPieceId.Bishop },
            { 'n', ChessPieceId.Knight },
            { 'q', ChessPieceId.Queen },
            { 'k', ChessPieceId.King }
        };

        public static string Encrypt(PieceInfo[,] board)
        {
            string fenString = "";
            int emptyCount = 0;

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    PieceInfo currentTile = board[rank, file];
                    char pieceChar = 'z'; // can't have a null char variable nor can it be empty so z is our 'empty' char

                    if (currentTile.PieceId == ChessPieceId.Empty)
                    {
                        emptyCount++;
                    }

                    if (currentTile.PieceId != ChessPieceId.Empty)
                    {
                        if (emptyCount > 0)
                        {
                            fenString += emptyCount.ToString();
                            emptyCount = 0;
                        }

                        pieceChar = GetCharFromPieceId(currentTile.PieceId);

                        if (currentTile.Color == ChessColor.White)
                        {
                            pieceChar = char.ToUpper(pieceChar);
                        }
                    }

                    if (pieceChar != 'z')
                    {
                        fenString += pieceChar;
                    }
                }

                if (emptyCount > 0)
                {
                    fenString += emptyCount.ToString();
                    emptyCount = 0;
                }

                if (rank != 7)
                {
                    fenString += "/";
                }
            }

            return fenString;
        }

        public static PieceInfo[,] Decrypt(string fenString, List<PieceInfo> whitePieceOut, List<PieceInfo> blackPieceOut)
        {
            PieceInfo[,] board = new PieceInfo[8, 8];

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
                    board[rank, file] = PieceInfo.GetEmptyPiece();
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
                        board[rank, tempFile] = PieceInfo.GetEmptyPiece();
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

                    var piece = new PieceInfo { Color = color, PieceId = pieceId };

                    board[rank, file] = piece;

                    if (color == ChessColor.White)
                    {
                        whitePieceOut.Add(piece);
                    }
                    else
                    {
                        blackPieceOut.Add(piece);
                    }

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
            }
            else
            {
                return ChessColor.White;
            }
        }

        private static ChessPieceId GetPieceIdFromChar(char c)
        {
            char lower = char.ToLower(c);

            ChessPieceId pieceId;
            charToPieceMap.TryGetValue(lower, out pieceId);

            return pieceId;
        }

        private static char GetCharFromPieceId(ChessPieceId pieceId)
        {
            char lower;

            pieceIdChars.TryGetValue(pieceId, out lower);

            return lower;
        }
    }
}
