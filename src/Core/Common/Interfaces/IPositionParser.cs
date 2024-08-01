using Core.Models.Dtos;

namespace Core.Common.Interfaces;

public interface IPositionParser
{
    List<Position> ParsePositions(string filePath);
}
