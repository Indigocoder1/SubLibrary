using HarmonyLib;
using SubLibrary.SubFire;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CyclopsExternalDamageManager))]
internal class ExternalDamageManagerPatches
{
    [HarmonyPatch(nameof(CyclopsExternalDamageManager.UpdateOvershield)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> UpdateOvershield_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var match = new CodeMatch(m => m.opcode == OpCodes.Ldc_I4_0);

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .Advance(1);

        var brFalseLabel = matcher.Instruction.operand;

        matcher.SetInstruction(new CodeInstruction(OpCodes.Brfalse, brFalseLabel))
            .Advance(-2)
            .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
            .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .Insert(Transpilers.EmitDelegate(HasAFire));

        return matcher.InstructionEnumeration();
    }

    public static bool HasAFire(SubFire originalSubFire, CyclopsExternalDamageManager instance)
    {
        var moddedSubFire = instance.GetComponentInChildren<ModdedSubFire>(true);

        if ((moddedSubFire && moddedSubFire.fireCount > 0) || (originalSubFire && originalSubFire.GetFireCount() > 0))
        {
            return true;
        }

        return false;
    }
}
