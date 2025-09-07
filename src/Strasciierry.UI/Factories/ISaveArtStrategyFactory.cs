using Strasciierry.UI.Helpers.SaveArtStrategies;

namespace Strasciierry.UI.Factories;

public interface ISaveArtStrategyFactory
{
    SaveArtStrategy CreateStrategy(string fileName);
}
