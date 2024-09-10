using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace CruncyMarshmallows;
public class CruncyMarshmallows : ModBehaviour
{
    public static CruncyMarshmallows Instance;

    public void Awake()
    {
        Instance = this;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        // You won't be able to access OWML's mod helper in Awake.
        // So you probably don't want to do anything here.
        // Use Start() instead.
    }

    public void Start()
    {
        // Starting here, you'll have access to OWML's mod helper.
        ModHelper.Console.WriteLine($"My mod {nameof(CruncyMarshmallows)} is loaded!", MessageType.Success);

        new Harmony("NineMeowz.CruncyMarshmallows").PatchAll(Assembly.GetExecutingAssembly());

        // Example of accessing game code.
        OnCompleteSceneLoad(OWScene.TitleScreen, OWScene.TitleScreen); // We start on title screen
        LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;
    }

    public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene)
    {
        if (newScene != OWScene.SolarSystem) return;
        ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
    }
}

[HarmonyPatch]
public static class Patches
{
    // Note to self: add DeathType.Dream as a death to see what happens for funsies
    // Static because in a static class
    // Etherpod was here
    static Dictionary<int, DeathType> deathRandomization = new Dictionary<int, DeathType>()
    {
        {0, DeathType.Crushed },
        {1, DeathType.Asphyxiation },
        {2, DeathType.Supernova },
        {3, DeathType.Meditation },
        {4, DeathType.Lava },
        {5, DeathType.Digestion },
        {6, DeathType.Energy },
        {7, DeathType.BigBang}
    };

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Marshmallow), nameof(Marshmallow.Eat))]
    public static void WellDoneMarshmallow(Marshmallow __instance)
    {
        if (__instance.IsBurned())
        {
            int rand = Random.Range(0, 7);
            //Note: last number is not included so it goes from 0 - 9 (above)
            Locator.GetDeathManager().KillPlayer(deathRandomization[rand]);
            //ctrl+shift+b = build. Do this after saving so that the manager gets updated.

            //int[] array = new int[] { 0, 2, 3 }; //This line with the brackets "[]" turns the integer into an array of numbers.
            //Dictionary<int, DeathType> coolerDictionary = new Dictionary<int, DeathType>();
            //coolerDictionary = new() { { 0, DeathType.Crushed }, { 1, DeathType.BlackHole }, { 2, DeathType.Asphyxiation } };
            //string text = coolerDictionary[2];
        }
    }
}

