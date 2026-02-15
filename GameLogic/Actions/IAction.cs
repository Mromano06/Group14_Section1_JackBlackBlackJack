using GameLogic.Core;
using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameLogic.Actions
{
    public interface IAction
    {
        bool IsExecutable(Game game);

        ActionResult Execute(Game game);

        string Description { get; }
    }

    public class ActionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static ActionResult Failed(string message)
        {
            return new ActionResult { Success = false, Message = message};
        }

        public static ActionResult Successful(string message)
        {
            return new ActionResult { Success = true, Message = message };
        }
    }
}
