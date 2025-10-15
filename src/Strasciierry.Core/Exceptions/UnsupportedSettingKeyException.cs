namespace Strasciierry.Core.Exceptions;

public class UnsupportedSettingKeyException : Exception
{
    public string Key { get; }

    public UnsupportedSettingKeyException(string key)
        : base($"Setting with key {key} is not supproted")
    {
        Key = key;
    }
}
