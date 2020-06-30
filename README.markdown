Brewed Beverages is a Caves of Qud mod that adds the automatic brewer, a machine that brews delicious (and not-so-delicious) beverages! Unfortunately, the manual has been lost to time.

Find one on every run in Dardi's kitchen!

See [the spoilers](Spoilers.markdown) for more information.

# Installing

See [the Official Caves of Qud Wiki](https://cavesofqud.gamepedia.com/Modding:Installing_a_mod#GitHub) for directions.

# Modding

Other mods can add more recipes for the automatic brewer! A recipe is an object blueprint that has the following tags:

- `helado_Brewed Beverages_Recipe`, which only needs to be present and whose value is not used;
- `helado_Brewed Beverages_Ingredients`, which is used as a comma-separated list of blueprint names;
- `helado_Brewed Beverages_Duration`, which must parse as a `byte`;
- and `helado_Brewed Beverages_Beverage`, which is used as a liquid identifier.

Here is the XML code for a sample recipe blueprint:

    <object Name="My Recipe" Inherits="DataBucket">
        <tag Name="helado_Brewed Beverages_Recipe" />
        <tag Name="helado_Brewed Beverages_Ingredients" Value="My Cool Item" />
        <tag Name="helado_Brewed Beverages_Duration" Value="10" />
        <tag Name="helado_Brewed Beverages_Beverage" Value="mycoolliquid" />
    </object>

To use this recipe, place at least one item whose blueprint name is `My Cool Item` into a brewer's intake, wait 10 turns, and it should dispense the liquid whose identifier is `mycoolliquid`.

# Contributing

Open an issue or send a pull request on [the project's Github repository](https://github.com/HeladoDeBrownie/Caves-of-Qud-Brewed-Beverages). If you contribute code, make sure your contribution runs in-game.

# Developing

The following steps are optional but useful:

- Install the `dotnet` command.
- Copy or symbolic link `Assembly-CSharp.dll` and `UnityEngine.CoreModule.dll` from the *Caves of Qud* installation into this mod's directory. (These are ignored by this project's git settings.)

Now you can `dotnet build` to error check the code and also use Visual Studio Code's code completion.
