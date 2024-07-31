using Core.Models;

namespace Application.Interfaces;

public interface IPositionParser
{
    List<Position> ParsePositions(string filePath);
}
