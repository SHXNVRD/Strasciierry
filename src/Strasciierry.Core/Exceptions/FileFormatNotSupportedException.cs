namespace Strasciierry.Core.Exceptions;

public class FileFormatNotSupportedException(string format) 
    : Exception($"{format} file format is not supported")
{ }