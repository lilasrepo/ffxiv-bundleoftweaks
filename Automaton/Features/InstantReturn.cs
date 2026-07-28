namespace Automaton.Features;

[Tweak(debug: true)]
public unsafe class InstantReturn : Tweak
{
    public override string Name => "Quick Return";
    public override string Description => "Calls the return function directly";

    private readonly Memory.AgentReturn Return = new();

    // B1(api13 / TC game v7.20): same shape as AutoSnipeQuests -- Memory.Signatures.AgentReturnReceiveEvent
    // did not resolve on the TC binary, EzSignatureHelper swallows the scan failure and leaves ReturnHook
    // null, and the bare Enable() then NREs on every Enable. RESOLVED 2026-07-29 with a rescanned value
    // from 9f3c68b (2025-11-01) -- the earlier "no candidate" claim was wrong because JP/ffxiv-bundleoftweaks
    // is checked out AT the walk-back pin, so "JP HEAD" was the pin rather than upstream.
    // Worth knowing for later: upstream has since dropped the signature entirely and hooks
    // AgentReturn.MemberFunctionPointers.Return via FFXIVClientStructs, which would remove this class of
    // breakage for good -- but that lives past the api14 wall. The guard stays as cheap insurance.
    private bool HookAvailable => Return.ReturnHook != null;

    public override void Enable()
    {
        if (!HookAvailable)
        {
            Warning("return-handler signature did not resolve on this game version -- feature inactive.");
            return;
        }
        Return.ReturnHook.Enable();
    }

    public override void Disable()
    {
        if (!HookAvailable) return;
        Return.ReturnHook.Disable();
    }
}
