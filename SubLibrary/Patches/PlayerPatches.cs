using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SubLibrary.Monobehaviors;
using UnityEngine;

namespace SubLibrary.Patches;

[HarmonyPatch(typeof(Player))]
internal static class PlayerPatches
{
    [HarmonyPatch(nameof(Player.ValidateCurrentSub)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ValidateCurrentSub_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var match = new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && Mathf.Approximately((float)i.operand, 35f));

        var matcher = new CodeMatcher(instructions);
        matcher.MatchForward(true, match)
            .Advance(1)
            .InsertAndAdvance(Transpilers.EmitDelegate(GetCustomValidationRange));
        
        return matcher.InstructionEnumeration();
    }

    private static float GetCustomValidationRange(float previousValue)
    {
        var currentSub = Player.main.currentSub;
        if (!currentSub || !currentSub.TryGetComponent(out CustomValidationRange customRange)) return previousValue;

        return customRange.validationRange;
    }
}