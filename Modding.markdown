This text assumes basic familiarity with *Caves of Qud* XML modding. You can find out more in [the modding section of the Official *Caves of Qud* Wiki](https://cavesofqud.gamepedia.com/Modding:Overview).

# Specifying recipes

A recipe is represented by an object blueprint that has the following required tags:

- `helado_Brewed Beverages_Recipe`, which is only used to locate the recipe;
- `helado_Brewed Beverages_Ingredients`, which is used as a comma-separated list of blueprint names and represents the required ingredients for the recipe;
- `helado_Brewed Beverages_Duration`, which must parse as a `byte` and represents the number of turns it takes to brew the recipe;
- and `helado_Brewed Beverages_Beverage`, which is used as either a liquid identifier or the name of a population table (in order of preference) and represents the liquid produced by the recipe.

The following optional tags also have meaning:

- `helado_Brewed Beverages_Tricky`, which cosmetically alters the brewing success message;
- and `helado_Brewed Beverages_Mistake`, which causes the recipe to give the brewing failure message and also annoy the brewer.

## Example

Here is the XML code for a sample recipe blueprint:

    <object Name="My Cool Recipe" Inherits="DataBucket">
        <tag Name="helado_Brewed Beverages_Recipe" />
        <tag Name="helado_Brewed Beverages_Ingredients" Value="Vinewafer,Starapple,Urberry" />
        <tag Name="helado_Brewed Beverages_Duration" Value="23" />
        <tag Name="helado_Brewed Beverages_Beverage" Value="cloning" />
    </object>

The blueprint may inherit from anything, but to avoid accidentally spawning it as an object, you should either include `<tag Name="ExcludeFromDynamicEncounters" />` or inherit from `DataBucket` (as done above), which includes this for you. Since no particular inheritance scheme is required to specify recipes, your mod should be able to work even when Brewed Beverages is not installed, as long as it has no other explicit dependencies on it.

To use this example recipe, place at least one each of a vinewafer, a starapple, and an urberry into an automatic brewer (and no other objects), wait 23 turns, and observe that it dispenses cloning draught. Feel free to experiment with making your own recipes using both base-game objects and liquids and ones provided by your mod or another mod.
