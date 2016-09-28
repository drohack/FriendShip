using UnityEngine;
using System.Collections;

public class Command_Array : MonoBehaviour {

    public const string pullcordText = "Pullcord (Everyone Pull Down)";

    //Buttons
    public readonly ArrayList buttonCommandArray_EASY = new ArrayList
    {
        "Overdrive",
        "Beast Mode",
        "Cruise Control",
        "Four Wheel Drive",
        "Fog Machine",
        "Rear Thrusters",
        "Forward Thrusters",
        "Waifus",
        "Husbandos",
        "Flux Capacitors"
    };
    //sci-fi stuff
    public readonly ArrayList buttonCommandArray_MEDIUM = new ArrayList
    {
        "Interdimensional Time Warp",
        "Ludicrous Speed",
        "Space Highway",
        "Bicentennial Man",
        "Psychometry",
        "Pyrokinesis",
        "Unidentified Flying Object",
        "Invisibility Cloak",
        "Active Camouflage",
        "Bioluminescence"
    };
    //animals
    public readonly ArrayList buttonCommandArray_HARD = new ArrayList
    {
        "><(((('>",
        "~~(__^·>",
        "ˁ˚ᴥ˚ˀ",
        "Ƹ̵̡Ӝ̵̨̄Ʒ",
        "^(*(oo)*)^",
        "<'))))><",
        "-^^,--,~",
        "'-'_@_",
        "くコ:彡",
        "°j°m"
    };
    //Dials
    public readonly ArrayList dialCommandArray_EASY = new ArrayList
    {
        "Rec Room Radio",
        "Bathroom TVs",
        "CB Radio",
        "Surround Sound",
        "AM/FM Radio",
        "Walkman",
        "Telephone",
        "Softwave",
        "Boombbox",
        "Cable"
    };
    //planets & galaxies
    public readonly ArrayList dialCommandArray_MEDIUM = new ArrayList
    {
        "Mercury",
        "Venus",
        "Earth",
        "Mars",
        "Jupiter",
        "Saturn",
        "Uranus",
        "Neptune",
        "Andromeda",
        "The Cosmos",
        "Large Magellanic",
        "Small Magellanic",
        "Triangulum",
        "Centaurus A",
        "Milky Way"
    };
    //hard planets & made up words
    public readonly ArrayList dialCommandArray_HARD = new ArrayList
    {
        "Proxima Centauri b",
        "Kepler-9b",
        "Gliese 1214 b",
        "OGLE-TR-10b",
        "CoRoT-1 b",
        "Gamma Cephei b",
        "HATS-10b",
        "KOI-192 b",
        "Lupus-TR 3b",
        "NN Ser (AB) c",
        "Carbonok Betanome",
        "Cortate Nonon",
        "Wasapian Xenodeo",
        "Tychodyne Venuxide",
        "Rexolex Starnix",
        "Inlix Lightxide",
        "Anagen Venustia",
        "Machibite Wirime",
        "Faro Kilamorph",
        "Altolios Wavani"
    };
    //L levers
    public readonly ArrayList lLeverCommandArray_EASY = new ArrayList
    {
        "Trap Doors",
        "Steam Rollers",
        "Emergency Brakes",
        "Ejector Seats",
        "Party Lights",
        "Airbags",
        "The Power Generator",
        "The Bubbles",
        "Cooling System",
        "Fasten Seatbelt Sign"
    };
    //sci-fi stuff
    public readonly ArrayList lLeverCommandArray_MEDIUM = new ArrayList
    {
        "Life Support",
        "Communications",
        "Thermal Protection",
        "Android Helpers",
        "Ignorance Accelerator",
        "Steampunk Engines",
        "VR Headset",
        "Shark Repelant",
        "Nano-bots",
        "Replicators"
    };
    //faces
    public readonly ArrayList lLeverCommandArray_HARD = new ArrayList
    {
        "(-(-_(-_-)_-)-)",
        "٩(̾●̮̮̃̾•̃̾)۶",
        "╭∩╮(Ο_Ο)╭∩╮",
        "┌∩┐(◣_◢)┌∩┐",
        "(-.-)Zzz...",
        "(♥_♥)",
        "¯\\_(ツ)_/¯ ",
        "(✖╭╮✖)",
        " ლ(ಠ益ಠ)ლ",
        "(ಠ_ಠ)"
    };
    //Lightswitches
    public readonly ArrayList lightswitchCommandArray_EASY = new ArrayList
    {
        "Hazard Lights",
        "Surveillance Cameras",
        "Kitchen Blender",
        "Coffee Maker",
        "Spotlights",
        "Radar",
        "WiFi Networks",
        "Air Conditioning",
        "Heat Lamps",
        "Lamp"
    };
    //named objects
    public readonly ArrayList lightswitchCommandArray_MEDIUM = new ArrayList
    {
        "Automaton of Acid Beam",
        "Utensil of Darkness",
        "Doomed Ornate Pyramid",
        "Orb of the Fine Charm",
        "Sphere of Magma",
        "Wondrous Thundering Cube",
        "Bar of Spectral Nothing",
        "Vorpal Cone of the Krakens",
        "Unspeakably Clear Box",
        "Glorified Rugged Switch"
        };
    //table flip
    public readonly ArrayList lightswitchCommandArray_HARD = new ArrayList
    {
        "(╯°□°）╯︵┻━┻",
        "┻━┻︵\\(°□°)/︵┻━┻",
        "(/ಠ益ಠ)/︵┻━┻    ︵┻━┻",
        "┏━┓︵/(^.^/)",
        "༼ つ ◕_◕ ༽つ",
        "( ͡° ͜ʖ ͡°)",
        "／人◕‿‿◕人＼",
        "(\\/)(Ö,,,,Ö)(\\/)",
        "(✿ ♥‿♥)",
        "(╯°□°)︻╦╤─–"
        };
    //Plutonium Batteries
    public readonly ArrayList plutoniumBatteryCommandArray_EASY = new ArrayList
    {
        "Engines",
        "Batteries",
        "Turbines",
        "Reactor",
        "Generators",
        "Fusion Core",
        "Lithium-ion Battery",
        "Plutonium Cell",
        "Ship's Power Grid",
        "Weapons"
        };
    //Sci fi engines/drives
    public readonly ArrayList plutoniumBatteryCommandArray_MEDIUM = new ArrayList
    {
        "Quantum Flux Drive",
        "Warp Drive System",
        "Hyperdrive System",
        "Infinite Improbability",
        "FTL Drive",
        "Holtzman Drive",
        "Starburst System",
        "Boom Tubes",
        "Quantum Slip- Stream Drive",
        "Timewarp"
        };
    //Objects
    public readonly ArrayList plutoniumBatteryCommandArray_HARD = new ArrayList
    {
        "☢ ☢ ☢",
        "☣ ☣ ☣",
        "☠ ☠ ☠",
        "☮ ☮ ☮",
        "☭ ☭ ☭",
        "✎ ✉ ✉",
        "Ɛղցìղҽʂ",
        "βąէէҽɾìҽʂ",
        "Ͳմɾҍìղҽʂ",
        "ɾҽąçէօɾ"
        };
    //Shifters
    public readonly ArrayList shifterCommandArray_EASY = new ArrayList
    {
        "Sarcasm",
        "Altitude",
        "Velocity",
        "Puns",
        "Depth",
        "Happiness Level",
        "Power Level",
        "Awareness",
        "Pressure",
        "Value"
        };
    //tools
    public readonly ArrayList shifterCommandArray_MEDIUM = new ArrayList
    {
        "Bionic Opticoscope",
        "Flux Retinoscope",
        "Heart Ultrasupporter",
        "Variable Depth Probe",
        "Nuclear Mallet",
        "Ultrasonic Atomispanner",
        "Zero-gravity Wrench",
        "Flux Screwdriver",
        "Singularity Vise",
        "Catalytic Chronohammer"
        };
    //objects
    public readonly ArrayList shifterCommandArray_HARD = new ArrayList
    {
        "[̲̅$̲̅(̲̅ιοο̲̅)̲̅$̲̅]",
        "^(,,,)^",
        "Ⓞ═╦╗",
        "°º¤ø,¸ ¸,ø¤º°`°º¤ø,¸",
        "[̲̅$̲̅(̲̅1̲̅)̲̅$̲̅]",
        "[̲̅$̲̅(̲̅5̲̅)̲̅$̲̅]",
        "[_]3",
        "())_CRAYON_))>",
        "{ o }====(:::)",
        "இڿڰۣ-ڰۣ—"
        };
    //Sliders
    public readonly ArrayList sliderCommandArray_EASY = new ArrayList
    {
        "Dat Bass",
        "Defcon Level",
        "Mobile Data",
        "Immune System",
        "Metabolism",
        "Confidence",
        "Sound System",
        "Energy",
        "Taxes",
        "Carbon Footprint"
    };
    //items
    public readonly ArrayList sliderCommandArray_MEDIUM = new ArrayList
    {
        "The Amulet of Time",
        "The Rods of Fear, and Greed",
        "The Gate of Misery",
        "The Token of Worlds",
        "The Idol of Blight",
        "The Ancient Hell Skull",
        "The Crystal of Insight",
        "The Thunderous Medallion",
        "The Scarab of Enchantment",
        "The Perfect Lute of Days"
        };
    //audio/words
    public readonly ArrayList sliderCommandArray_HARD = new ArrayList
    {
        "♫♪..|̲̅̅●̲̅̅|̲̅̅=̲̅̅|̲̅̅●̲̅̅|..♫♪",
        "❚█══█❚",
        "█▬█ █▄█ █▬█",
        "┣▬▇▇▇═─",
        "▇ ▂ ▃ ▁ ▅ ▃ ▅",
        "█▬█ █ ▀█▀",
        "♪┏(°.°)┛",
        "(⌐■_■)",
        "d[-_-]b",
        "ⓛⓞⓥⓔ"
    };
    //Valves
    public readonly ArrayList valveCommandArray_EASY = new ArrayList
    {
        "Peanut Butter Lid",
        "Bike Chain",
        "Guitar Strings",
        "Defence",
        "Preasure Valve",
        "Water Flow",
        "Steam Vents",
        "Security",
        "Crankshaft",
        "Steering Wheel"
    };
    //real valves
    public readonly ArrayList valveCommandArray_MEDIUM = new ArrayList
    {
        "Ball Valve",
        "Butterfly Valve",
        "Globe Valve",
        "Gate Valve",
        "Diaphragm Valve",
        "Clapper Valve",
        "Check valve",
        "Needle valve",
        "Pinch valve",
        "Safety valve"
    };
    //concepts
    public readonly ArrayList valveCommandArray_HARD = new ArrayList
    {
        "Grasp on Reality",
        "Belief System",
        "Empirical Research",
        "Existential Crisis",
        "Ignoramus Et Ignorabimus",
        "Intrinsic and Extrinsic Properties",
        "Logical Consequences",
        "Philosophical Analysis",
        "Posthegemony",
        "Righteousness of Self"
    };
    //W Levers
    public readonly ArrayList wLeverCommandArray_EASY = new ArrayList
    {
        "Rear Flags",
        "Exterior Shutters",
        "The Stakes",
        "Your Glass",
        "The Flag",
        "The Squid",
        "The Roof",
        "Blood Pressure",
        "The Bar",
        "The Seats"
    };
    //theories
    public readonly ArrayList wLeverCommandArray_MEDIUM = new ArrayList
    {
        "Special Relativity",
        "General Relativity",
        "Quantum Mechanics",
        "Natrual Selection",
        "Heliocentrism",
        "The Big Bang",
        "Cosmic Expansion",
        "Thermodynamics",
        "Archimedes' Buoyancy Principle",
        "Wave-Particle Duality"
    };
    //Sets
    public readonly ArrayList wLeverCommandArray_HARD = new ArrayList
    {
        "@xxx[{::::::::::::>",
        "▬▬ι═══════ﺤ",
        "[♥]]] [♦]]] [♣]]] [♠]]]",
        "︻デ┳═ー",
        "-♂- -♀- -♂- -♀-",
        "♝ ♗ ♞ ♘ ♟ ♙",
        "✿ ❀ ❁ ✾",
        "☠ ☢ ☣ ☠ ☢ ☣",
        "⌛ ⌚ ⌛ ⌚ ⌛ ⌚",
        "☼ ☀ ☁ ☂ ☃"
    };
}
