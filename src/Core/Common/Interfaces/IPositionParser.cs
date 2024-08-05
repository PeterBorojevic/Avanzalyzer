using Core.Models.Dtos.Csv;

namespace Core.Common.Interfaces;

public interface IPositionParser
{
    List<Position> ParsePositions(string filePath);
}
