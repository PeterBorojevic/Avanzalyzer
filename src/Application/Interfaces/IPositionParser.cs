using Core.Models.Dtos;

namespace Application.Interfaces;

public interface IPositionParser
{
    List<Position> ParsePositions(string filePath);
}
