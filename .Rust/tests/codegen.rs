use kataru::*;
use kataru_ffi::build_codegen_consts;

#[test]
fn test_build_codegen_consts() {
    let story = Story::load("tests/data/story").unwrap();
    let source = build_codegen_consts(&story).unwrap();
    let expected = r###"/// DO NOT EDIT.
/// This file was autogenerated by Kataru based on your scripts.

namespace Kataru
{
    /// <summary>
    /// Autogenerated class containing all namespace strings.
    /// </summary>
    public static class Namespaces
    {
        public const string Global = "global",
            Room1 = "Room1",
            Room2 = "Room2";

        /// <summary>
        /// Get all namespace names.
        /// </summary>
        public static string[] All() => all;

        private static readonly string[] all = {
            Global,
            Room1,
            Room2
        };
    }

    /// <summary>
    /// Autogenerated class containing all passage strings.
    /// </summary>
    public static class Passages
    {
        public const string None = "None",
            EndDialogue = "EndDialogue",
            Room1_RedSlimeTalk = "Room1:RedSlimeTalk",
            Room1_RedSlimeTrigger = "Room1:RedSlimeTrigger",
            Room2_AngryGreen = "Room2:AngryGreen",
            Room2_BlueSlimeTalk = "Room2:BlueSlimeTalk",
            Room2_GiveGoop = "Room2:GiveGoop",
            Room2_GreenObserve = "Room2:GreenObserve",
            Room2_NoGiveGoop = "Room2:NoGiveGoop",
            Room2_Poster = "Room2:Poster",
            Room2_ThanksGoop = "Room2:ThanksGoop",
            Start = "Start";

        /// <summary>
        /// Get all passage names.
        /// </summary>
        public static string[] All() => all;

        /// <summary>
        /// Get all passage names in a given namespace.
        /// This requires a linear search over all passage names, so don't do this at runtime.
        /// </summary>
        public static string[] InNamespace(string @namespace) => NamespaceUtils.FilterByNamespace(all, @namespace);

        private static readonly string[] all = {
            None,
            EndDialogue,
            Room1_RedSlimeTalk,
            Room1_RedSlimeTrigger,
            Room2_AngryGreen,
            Room2_BlueSlimeTalk,
            Room2_GiveGoop,
            Room2_GreenObserve,
            Room2_NoGiveGoop,
            Room2_Poster,
            Room2_ThanksGoop,
            Start
        };
    }

    /// <summary>
    /// Autogenerated class containing all character strings.
    /// </summary>
    public static partial class Characters
    {
        public const string None = "None",
            GlobalLight = "GlobalLight",
            Room1_Party = "Room1:Party",
            Room1_PartyLight = "Room1:PartyLight",
            Room1_RedSlime = "Room1:RedSlime",
            Room2_BlueSlime = "Room2:BlueSlime",
            Room2_GreenSlime = "Room2:GreenSlime",
            Slime = "Slime",
            Think = "Think";

        /// <summary>
        /// Get all character names.
        /// </summary>
        public static string[] All() => all;

        /// <summary>
        /// Get all character names in a given namespace.
        /// This requires a linear search over all passage names, so don't do this at runtime.
        /// </summary>
        public static string[] InNamespace(string @namespace) => NamespaceUtils.FilterByNamespace(all, @namespace);

        private static readonly string[] all = {
            None,
            GlobalLight,
            Room1_Party,
            Room1_PartyLight,
            Room1_RedSlime,
            Room2_BlueSlime,
            Room2_GreenSlime,
            Slime,
            Think
        };
    }
}
"###;

    // For debugging, write the files so we can diff.
    // std::fs::write("tests/source.cs", &source).unwrap();
    // std::fs::write("tests/expected.cs", &expected).unwrap();

    assert_eq!(source, expected);
}
