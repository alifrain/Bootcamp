using System;
using System.Dynamic;
using Microsoft.VisualBasic;
namespace OthelloGameProject;

// Enums dan Records
public enum ColorType
{
    None,
    White,
    Black
}
public record struct Position(int Row, int Col);
