/*
    Add a 5% chance that an automatic brewer will spawn alongside any
    stationary space-time vortex.
*/

using static UnityEngine.Debug;
using XRL;

[HasModSensitiveStaticCache]
public class helado_BrewedBeverages_BrewerPopulator
{
    const string Blueprint = "helado_Brewed Beverages_Automatic Brewer";

    [ModSensitiveCacheInit]
    public static void Reset()
    {
        PopulationInfo population;

        PopulationManager.Populations.TryGetValue("CommonOddEncounters",
            out population);

        if (population != null)
        {
            var outerGroup = population.Items[0] as PopulationGroup;

            if (outerGroup != null)
            {
                foreach (var outerGroupItem in outerGroup.Items)
                {
                    var innerGroup = outerGroupItem as PopulationGroup;

                    if (innerGroup != null)
                    {
                        foreach (var innerGroupItem in innerGroup.Items)
                        {
                            var @object = innerGroupItem as PopulationObject;

                            if (
                                @object != null &&
                                @object.Blueprint == "Space-Time Rift"
                            )
                            {
                                innerGroup.Items.Add(new PopulationObject
                                {
                                    Blueprint = Blueprint,
                                    Chance = "5",
                                    Number = "1",
                                });
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
