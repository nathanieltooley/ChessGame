using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Scripts.Helpers
{
    public static class MiscHelpers
    {
        public static ChessColor InvertColor(ChessColor color)
        {
            if (color == ChessColor.White)
            {
                return ChessColor.Black;
            }
            else
            {
                return ChessColor.White;
            }
        }
    }
}
