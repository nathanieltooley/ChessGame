using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Scripts.Helpers
{
    public static class LogHelpers
    {
        public static void DebugLog(string message)
        {
            Debugger.Log(2, "inf", message);
        }
    }
}
