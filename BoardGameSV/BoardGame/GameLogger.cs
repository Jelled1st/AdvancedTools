using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

class GameLogger
{
    GameBoard board;

    public GameLogger()
    {
    }

    public void SetBoard(GameBoard pBoard)
    {
        board = pBoard;
    }

    private string buildLogString()
    {
        string boardState = board.ToString();
        string boardInformation = "";
        if (board.MaxMovesLeft() == 0)
        {
            boardInformation += "Winner: " + board.CheckWinner();
        }
        else boardInformation += "Max Moves Left: " + board.MaxMovesLeft();

        string log = "";
        log += boardInformation + "\n==============================\nBoardsate\n" + boardState;
        return log;
    }

    private void SetAdditionalInformation(string pInformation)
    {

    }

    public void LogToConsole()
    {
        Console.WriteLine(buildLogString());
    }

    public void LogToFile(string pPath)
    {
        File.WriteAllText(pPath, buildLogString());
    }
}