namespace Strasciierry.Core.Commands;

public interface ICommand
{
    Task Do();
    Task Undo();
}