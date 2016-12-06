using UnityEngine;
using System.Collections;

public class Command_Array : MonoBehaviour {

    public const string pullcordText = "Pullcord (Everyone Pull Down)";

    //Buttons
    // "Engage ______"
    public readonly ArrayList buttonCommandArray_EASY = new ArrayList
    {
        "Overdrive",
        "Beast Mode",
        "Cruise Control",
        "Four Wheel Drive",
        "Fog Machine",
        "Rear Thrusters",
        "Forward Thrusters",
        "Side Thrusters",
        "Waifus",
        "Husbandos",
        "Flux Capacitors",
        "Pyrotechnics",
        "Power Transfer",
        "Simulator",
        "Virtual Reality",
        "Simulated Reality",
        "Robot Arm",
        "Ejecter Seats",
        "Spacehorn",
        "Prime Directive",
        "Society",
        "Photofilter",
        "Fido",
        "The Enemy",
        "The Unseen",
        "Monorotor",
        "Voltcannon",
        "A-guage",
        "T-guage",
        "F-guage",
        "Runners",
        "Camera",
        "Jumbotron",
        "Shrink Ray",
        "Tiger Blood",
        "All Range Mode",
        "Depthfinder",
        "Interns",
        "Secretary",
        "Coffee Pot",
        "Doorbell"
    };
    public readonly ArrayList buttonCommandArray_MEDIUM = new ArrayList
    {
        //sci-fi stuff
        "Inter - dimensional Warp",
        "Ludicrous Speed",
        "Space Highway",
        "Bicentennial Man",
        "Psychometry",
        "Pyrokinesis",
        "Unidentified Flying Object",
        "Invisibility Cloak",
        "Active Camouflage",
        "Bioluminescence",
        "Photosynthesis",
        "Stage 1 Rockets",
        "Stage 2 Rockets",
        "Stage 3 Rockets",
        "Docking Probe",
        "Space Elevator",
        "Space Bridge",
        "Floating Orb",
        "Cosmic Ray",
        "Cosmic Sphere",
        "Prism Lance",
        "Vorpal Cone",
        "Astral Projection",
        "Holodeck",
        "Astral Plane"
    };
    public readonly ArrayList buttonCommandArray_HARD = new ArrayList
    {
        //animals
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
    // "Change ______ to Ch. x"
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
        "Cable",
        "Intercom",
        "The World",
        "Screen",
        "Stereo",
        "DJ",
        "TV Station",
        "Olympics",
        "Reruns"
    };
    public readonly ArrayList dialCommandArray_MEDIUM = new ArrayList
    {
        //planets
        "Mercury",
        "Venus",
        "Earth",
        "Mars",
        "Jupiter",
        "Saturn",
        "Uranus",
        "Neptune",
        //galaxies
        "Andromeda",
        "The Cosmos",
        "Large Magellanic",
        "Small Magellanic",
        "Triangulum",
        "Centaurus A",
        "Milky Way"
    };
    
    public readonly ArrayList dialCommandArray_HARD = new ArrayList
    {
        //hard planets
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
        //made up words
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
    // "Turn ON/OFF ______"
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
        "Fasten Seatbelt Sign",
        "Coolant",
        "Guiadance System",
        "Security System",
        "Darklatch",
        "Ionsock",
        "Monothrust",
        "Explosions",
        "Racket",
        "Fashion Police",
        "Grammar Police",
        "Fusion Core"
    };
    public readonly ArrayList lLeverCommandArray_MEDIUM = new ArrayList
    {
        //sci-fi stuff
        "Life Support",
        "Communications",
        "Thermal Protection",
        "Android Helpers",
        "Ignorance Accelerator",
        "Steampunk Engines",
        "VR Headset",
        "Shark Repelant",
        "Nano-bots",
        "Replicators",
        "Cryogenics",
        "Pressure Simulator",
        "Gravity",
        "Nuance Machine",
        "Test Tubes",
        "Escape Pods"
    };
    public readonly ArrayList lLeverCommandArray_HARD = new ArrayList
    {
        //faces
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
    // "Turn ON/OFF ______"
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
        "Lamp",
        "Monitor",
        "Ignition",
        "Heaters",
        "Safty",
        "Manual Hold",
        "Auto Hold",
        "Desk Fan",
        "Cabin Fan",
        "Crew Alert",
        "Interior Lights",
        "Computer",
        "Game",
        "Application",
        "Tablet",
        "Bluetooth",
        "Analyzer",
        "High Beams",
        "Defroster",
        "Windshield Wipers"
    };
    public readonly ArrayList lightswitchCommandArray_MEDIUM = new ArrayList
    {
        //named objects
        "Automaton of Acid Beam",
        "Utensil of Darkness",
        "Doomed Ornate Pyramid",
        "Orb of the Fine Charm",
        "Sphere of Magma",
        "Wondrous Thundering Cube",
        "Bar of Spectral Nothing",
        "Vorpal Cone of the Krakens",
        "Unspeakably Clear Box",
        "Glorified Rugged Switch",
        "Animatronics",
        "Cone of Frost",
        "Death Magnet of Vuu",
        "Shoddy Switch"
        };
    public readonly ArrayList lightswitchCommandArray_HARD = new ArrayList
    {
        //table flip/Faces
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
    // "Power ______"
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
        "Weapons",
        "The Economy",
        "Brain",
        "Morning",
        "Sun",
        "Election",
        "Country",
        "Widgets" 
        };
    public readonly ArrayList plutoniumBatteryCommandArray_MEDIUM = new ArrayList
    {
        //Sci fi engines/drives
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
    public readonly ArrayList plutoniumBatteryCommandArray_HARD = new ArrayList
    {
        //Objects
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
    // "Increase/Decrease ______ to x"
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
        "Value",
        "Arc Duration",
        "Amplitued",
        "Current",
        "Resistance",
        "Pitch",
        "Yaw",
        "Roll",
        "Caring",
        "Overdrive",
        "Fun",
        "Melancholy",
        "Winter"
        };
    public readonly ArrayList shifterCommandArray_MEDIUM = new ArrayList
    {
        //tools
        "Bionic Opticoscope",
        "Flux Retinoscope",
        "Heart Ultrasupporter",
        "Variable Depth Probe",
        "Nuclear Mallet",
        "Ultrasonic Atomispanner",
        "Zero-gravity Wrench",
        "Flux Screwdriver",
        "Singularity Vise",
        "Catalytic Chronohammer",
        "Liver Massager",
        "Drupal Counter",
        "Organ-O-Matic"
        };
    public readonly ArrayList shifterCommandArray_HARD = new ArrayList
    {
        //objects
        "[̲̅$̲̅(̲̅ιοο̲̅)̲̅$̲̅]",
        "^(,,,)^",
        "Ⓞ═╦╗",
        "°º¤ø,¸ ¸,ø¤º°`°º¤ø,¸",
        "[̲̅$̲̅(̲̅1̲̅)̲̅$̲̅]",
        "[̲̅$̲̅(̲̅5̲̅)̲̅$̲̅]",
        "[_]3",
        "())_CRAYON_))>",
        "{ o }====(:::)",
        "இڿڰۣ-ڰۣ—"
        };
    //Sliders
    // "Reduce/Boost ______ to x"
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
        "Carbon Footprint",
        "Rap",
        "Speaker Volume",
        "Headset Volume",
        "Rock & Roll",
        "Tune",
        "Beam",
        "Metasonar"
    };
    public readonly ArrayList sliderCommandArray_MEDIUM = new ArrayList
    {
        //items
        "The Amulet of Time",
        "The Rods of Fear, and Greed",
        "The Gate of Misery",
        "The Token of Worlds",
        "The Idol of Blight",
        "The Ancient Hell Skull",
        "The Crystal of Insight",
        "The Thunderous Medallion",
        "The Scarab of Enchantment",
        "The Perfect Lute of Days",
        //other
        "The Immune System"
        };
    public readonly ArrayList sliderCommandArray_HARD = new ArrayList
    {
        //audio/words
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
    // "Tighten/Loosen ______"
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
        "Steering Wheel",
        "Main Hatch",
        "Screws",
        "Bolts",
        "Guitar Strings"
    };
    public readonly ArrayList valveCommandArray_MEDIUM = new ArrayList
    {
        //real valves
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
    public readonly ArrayList valveCommandArray_HARD = new ArrayList
    {
        //concepts
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
    // "Raise/Lower ______"
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
        "The Seats",
        "The Tempurature",
        "The Humidity",
        "Water Pressure",
        "The Voltage",
        "The Sun",
        "The Moon",
        "The Stars",
        "Frustration",
        "Ventalation Shutters",
        "The Floor",
        "The Minimum Wage",
        "The Lantern",
        "The Torch",
        "The Titanic",
        "The Risk"
    };
    public readonly ArrayList wLeverCommandArray_MEDIUM = new ArrayList
    {
        //theories
        "Special Relativity",
        "General Relativity",
        "Quantum Mechanics",
        "Natrual Selection",
        "Heliocentrism",
        "The Big Bang",
        "Cosmic Expansion",
        "Thermodynamics",
        "Archimedes' Buoyancy Principle",
        "Wave-Particle Duality",
        //Other
        "The Economic Tide",
        "The Boiling Point"
    };
    public readonly ArrayList wLeverCommandArray_HARD = new ArrayList
    {
        //Sets
        "@xxx[{::::::::::::>",
        "▬▬ι═══════ﺤ",
        "[♥]]] [♦]]] [♣]]] [♠]]]",
        "︻デ┳═ー",
        "-♂- -♀- -♂- -♀-",
        "♝ ♗ ♞ ♘ ♟ ♙",
        "✿ ❀ ❁ ✾",
        "☠ ☢ ☣ ☠ ☢ ☣",
        "☼ ☀ ☁ ☂ ☃"
    };
}