using UnityEngine;
using System.Collections;

public class Command_Array : MonoBehaviour {
    //Buttons
    private const string button1Text = "Overdrive";
    private const string button2Text = "Beast Mode";
    private const string button3Text = "Cruise Control";
    private const string button4Text = "Four Wheel Drive";
    private const string button5Text = "Fog Machine";
    private const string button6Text = "Rear Thrusters";
    private const string button7Text = "Forward Thrusters";
    private const string button8Text = "Waifus"; 
    private const string button9Text = "Husbandos";
    private const string button10Text = "Flux Capacitors";
    //sci-fi stuff
    private const string button11Text = "Interdimensional Time Warp";
    private const string button12Text = "Ludicrous Speed";
    private const string button13Text = "Space Highway";
    private const string button14Text = "Bicentennial Man";
    private const string button15Text = "Psychometry";
    private const string button16Text = "Pyrokinesis";
    private const string button17Text = "Unidentified Flying Object";
    private const string button18Text = "Invisibility Cloak";
    private const string button19Text = "Active Camouflage";
    private const string button20Text = "Bioluminescence";
    //animals
    private const string button21Text = "><(((('>";
    private const string button22Text = "~~(__^·>";
    private const string button23Text = "ˁ˚ᴥ˚ˀ";
    private const string button24Text = "Ƹ̵̡Ӝ̵̨̄Ʒ";
    private const string button25Text = "^(*(oo)*)^";
    private const string button26Text = "<'))))><";
    private const string button27Text = "-^^,--,~";
    private const string button28Text = "'-'_@_";
    private const string button29Text = "くコ:彡";
    private const string button30Text = "°j°m";
    //Dials
    private const string dial1Text = "Rec Room Radio";
    private const string dial2Text = "Bathroom TVs";
    private const string dial3Text = "CB Radio";
    private const string dial4Text = "Surround Sound";
    private const string dial5Text = "AM/FM Radio";
    private const string dial6Text = "Walkman";
    private const string dial7Text = "Telephone";
    private const string dial8Text = "Softwave";
    private const string dial9Text = "Boombbox";
    private const string dial10Text = "Cable";
    //planets
    private const string dial11Text = "Proxima Centauri b";
    private const string dial12Text = "Kepler-9b";
    private const string dial13Text = "Gliese 1214 b";
    private const string dial14Text = "OGLE-TR-10b";
    private const string dial15Text = "CoRoT-1 b";
    private const string dial16Text = "Gamma Cephei b";
    private const string dial17Text = "HATS-10b";
    private const string dial18Text = "KOI-192 b";
    private const string dial19Text = "Lupus-TR 3b";
    private const string dial20Text = "NN Ser (AB) c";
    //made up words
    private const string dial21Text = "Carbonok Betanome";
    private const string dial22Text = "Cortate Nonon";
    private const string dial23Text = "Wasapian Xenodeo";
    private const string dial24Text = "Tychodyne Venuxide";
    private const string dial25Text = "Rexolex Starnix";
    private const string dial26Text = "Inlix Lightxide";
    private const string dial27Text = "Anagen Venustia";
    private const string dial28Text = "Machibite Wirime";
    private const string dial29Text = "Faro Kilamorph";
    private const string dial30Text = "Altolios Wavani";
    //L levers
    private const string lLever1Text = "Trap Doors";
    private const string lLever2Text = "Steam Rollers";
    private const string lLever3Text = "Emergency Brakes";
    private const string lLever4Text = "Ejector Seats";
    private const string lLever5Text = "Party Lights";
    private const string lLever6Text = "Airbags";
    private const string lLever7Text = "The Power Generator";
    private const string lLever8Text = "The Bubbles";
    private const string lLever9Text = "Cooling System";
    private const string lLever10Text = "Fasten Seatbelt Sign";
    //sci-fi stuff
    private const string lLever11Text = "Life Support";
    private const string lLever12Text = "Communications";
    private const string lLever13Text = "Thermal Protection";
    private const string lLever14Text = "Android Helpers";
    private const string lLever15Text = "Ignorance Accelerator";
    private const string lLever16Text = "Steampunk Engines";
    private const string lLever17Text = "VR Headset";
    private const string lLever18Text = "Shark Repelant";
    private const string lLever19Text = "Nano-bots";
    private const string lLever20Text = "Replicators";
    //faces
    private const string lLever21Text = "(-(-_(-_-)_-)-)";
    private const string lLever22Text = "٩(̾●̮̮̃̾•̃̾)۶";
    private const string lLever23Text = "╭∩╮(Ο_Ο)╭∩╮";
    private const string lLever24Text = "┌∩┐(◣_◢)┌∩┐";
    private const string lLever25Text = "(-.-)Zzz...";
    private const string lLever26Text = "(♥_♥)";
    private const string lLever27Text = "¯\\_(ツ)_/¯ ";
    private const string lLever28Text = "(✖╭╮✖)";
    private const string lLever29Text = " ლ(ಠ益ಠ)ლ";
    private const string lLever30Text = "(ಠ_ಠ)";
    //Lightswitches
    private const string lightswitch1Text = "Hazard Lights";
    private const string lightswitch2Text = "Surveillance Cameras";
    private const string lightswitch3Text = "Kitchen Blender";
    private const string lightswitch4Text = "Coffee Maker";
    private const string lightswitch5Text = "Spotlights";
    private const string lightswitch6Text = "Radar";
    private const string lightswitch7Text = "WiFi Networks";
    private const string lightswitch8Text = "Air Conditioning";
    private const string lightswitch9Text = "Heat Lamps";
    private const string lightswitch10Text = "Lamp";
    //named objects
    private const string lightswitch11Text = "Automaton of Acid Beam";
    private const string lightswitch12Text = "Utensil of Darkness";
    private const string lightswitch13Text = "Doomed Ornate Pyramid";
    private const string lightswitch14Text = "Orb of the Fine Charm";
    private const string lightswitch15Text = "Sphere of Magma";
    private const string lightswitch16Text = "Wondrous Thundering Cube";
    private const string lightswitch17Text = "Bar of Spectral Nothing";
    private const string lightswitch18Text = "Vorpal Cone of the Krakens";
    private const string lightswitch19Text = "Unspeakably Clear Box";
    private const string lightswitch20Text = "Glorified Rugged Switch";
    //table flip
    private const string lightswitch21Text = "(╯°□°）╯︵┻━┻";
    private const string lightswitch22Text = "┻━┻︵\\(°□°)/︵┻━┻";
    private const string lightswitch23Text = "(ノಠ益ಠ)ノ彡︵┻━┻︵┻━┻";
    private const string lightswitch24Text = "┏━┓︵/(^.^/)";
    private const string lightswitch25Text = "༼ つ ◕_◕ ༽つ";
    private const string lightswitch26Text = "( ͡° ͜ʖ ͡°)";
    private const string lightswitch27Text = "／人 ⌒ ‿‿ ⌒ 人＼";
    private const string lightswitch28Text = "(\\/)(Ö,,,,Ö)(\\/)";
    private const string lightswitch29Text = "(✿ ♥‿♥)";
    private const string lightswitch30Text = "(╯°□°)︻╦╤─ – – –";
    //Shifters
    private const string shifter1Text = "Sarcasm";
    private const string shifter2Text = "Altitude";
    private const string shifter3Text = "Velocity";
    private const string shifter4Text = "Puns";
    private const string shifter5Text = "Depth";
    private const string shifter6Text = "Happiness Level";
    private const string shifter7Text = "Power Level";
    private const string shifter8Text = "Awareness";
    private const string shifter9Text = "Pressure";
    private const string shifter10Text = "Value";
    //tools
    private const string shifter11Text = "Bionic Opticoscope";
    private const string shifter12Text = "Flux Retinoscope";
    private const string shifter13Text = "Heart Ultrasupporter";
    private const string shifter14Text = "Variable Depth Probe";
    private const string shifter15Text = "Nuclear Mallet";
    private const string shifter16Text = "Ultrasonic Atomispanner";
    private const string shifter17Text = "Zero-gravity Wrench";
    private const string shifter18Text = "Flux Screwdriver";
    private const string shifter19Text = "Singularity Vise";
    private const string shifter20Text = "Catalytic Chronohammer";
    //objects
    private const string shifter21Text = "[̲̅$̲̅(̲̅ιοο̲̅)̲̅$̲̅]";
    private const string shifter22Text = "^(;,;)^";
    private const string shifter23Text = "Ⓞ═╦╗";
    private const string shifter24Text = "°º¤ø,¸ ¸,ø¤º°`°º¤ø,¸";
    private const string shifter25Text = "[̲̅$̲̅(̲̅1̲̅)̲̅$̲̅]";
    private const string shifter26Text = "[̲̅$̲̅(̲̅5̲̅)̲̅$̲̅]";
    private const string shifter27Text = "[_]3";
    private const string shifter28Text = "())_CRAYON_))>";
    private const string shifter29Text = "{ o }====(:::)";
    private const string shifter30Text = "இڿڰۣ-ڰۣ—";
    //Sliders
    private const string slider1Text = "Dat Bass";
    private const string slider2Text = "Defcon Level";
    private const string slider3Text = "Mobile Data";
    private const string slider4Text = "Immune System";
    private const string slider5Text = "Metabolism";
    private const string slider6Text = "Confidence";
    private const string slider7Text = "Sound System";
    private const string slider8Text = "Energy";
    private const string slider9Text = "Taxes";
    private const string slider10Text = "Carbon Footprint";
    //items
    private const string slider11Text = "The Amulet of Time";
    private const string slider12Text = "The Rods of Fear, and Greed";
    private const string slider13Text = "The Gate of Misery";
    private const string slider14Text = "The Token of Worlds";
    private const string slider15Text = "The Idol of Blight";
    private const string slider16Text = "The Ancient Hell Skull";
    private const string slider17Text = "The Crystal of Insight";
    private const string slider18Text = "The Thunderbolt Medallion";
    private const string slider19Text = "The Scarab of Enchantment";
    private const string slider20Text = "The Perfect Lute of Days";
    //audio/words
    private const string slider21Text = "♫♪..|̲̅̅●̲̅̅|̲̅̅=̲̅̅|̲̅̅●̲̅̅|..♫♪";
    private const string slider22Text = "❚█══█❚";
    private const string slider23Text = "█▬█ █▄█ █▬█ █▄█";
    private const string slider24Text = "┣▬▇▇▇═─";
    private const string slider25Text = "▇ ▂ ▃ ▁ ▁ ▅ ▃ ▅";
    private const string slider26Text = "█▬█ █ ▀█▀";
    private const string slider27Text = "♪┏(°.°)┛";
    private const string slider28Text = "(⌐■_■)";
    private const string slider29Text = "d[-_-]b";
    private const string slider30Text = "ⓛⓞⓥⓔ";
    //Valves
    private const string valve1Text = "Peanut Butter Lid";
    private const string valve2Text = "Bike Chain";
    private const string valve3Text = "Guitar Strings";
    private const string valve4Text = "Defence";
    private const string valve5Text = "Preasure Valve";
    private const string valve6Text = "Water Flow";
    private const string valve7Text = "Steam Vents";
    private const string valve8Text = "Security";
    private const string valve9Text = "Crankshaft";
    private const string valve10Text = "Steering Wheel";
    //real valves
    private const string valve11Text = "Ball Valve";
    private const string valve12Text = "Butterfly Valve";
    private const string valve13Text = "Globe Valve";
    private const string valve14Text = "Gate Valve";
    private const string valve15Text = "Diaphragm Valve";
    private const string valve16Text = "Clapper Valve";
    private const string valve17Text = "Check valve";
    private const string valve18Text = "Needle valve";
    private const string valve19Text = "Pinch valve";
    private const string valve20Text = "Safety valve";
    //concepts
    private const string valve21Text = "Grasp on Reality";
    private const string valve22Text = "Belief System";
    private const string valve23Text = "Empirical Research";
    private const string valve24Text = "Existential Crisis";
    private const string valve25Text = "Ignoramus Et Ignorabimus";
    private const string valve26Text = "Intrinsic and Extrinsic Properties";
    private const string valve27Text = "Logical Consequences";
    private const string valve28Text = "Philosophical Analysis";
    private const string valve29Text = "Posthegemony";
    private const string valve30Text = "Righteousness of Self";
    //W Levers
    private const string wLever1Text = "Rear Flags";
    private const string wLever2Text = "Exterior Shutters";
    private const string wLever3Text = "The Stakes";
    private const string wLever4Text = "Your Glass";
    private const string wLever5Text = "The Flag";
    private const string wLever6Text = "The Squid";
    private const string wLever7Text = "The Roof";
    private const string wLever8Text = "Blood Pressure";
    private const string wLever9Text = "The Bar";
    private const string wLever10Text = "The Seats";
    //theories
    private const string wLever11Text = "Special Relativity";
    private const string wLever12Text = "General Relativity";
    private const string wLever13Text = "Quantum Mechanics";
    private const string wLever14Text = "Natrual Selection";
    private const string wLever15Text = "Heliocentrism";
    private const string wLever16Text = "The Big Bang";
    private const string wLever17Text = "Cosmic Expansion";
    private const string wLever18Text = "Thermodynamics";
    private const string wLever19Text = "Archimedes' Buoyancy Principle";
    private const string wLever20Text = "Wave-Particle Duality";
    //Sets
    private const string wLever21Text = "@xxx[{::::::::::::>";
    private const string wLever22Text = "▬▬ι═══════ﺤ";
    private const string wLever23Text = "[♥]]] [♦]]] [♣]]] [♠]]]";
    private const string wLever24Text = "︻デ┳═ー";
    private const string wLever25Text = "-♂- -♀- -♂- -♀-";
    private const string wLever26Text = "♝ ♗ ♞ ♘ ♟ ♙";
    private const string wLever27Text = "✿ ❀ ❁ ✾";
    private const string wLever28Text = "☠ ☢ ☣ ☠ ☢ ☣";
    private const string wLever29Text = "⌛ ⌚ ⌛ ⌚ ⌛ ⌚";
    private const string wLever30Text = "☼ ☀ ☁ ☂ ☃";

    public const string pullcordText = "(Everyone Pull Down)";

    public ArrayList buttonCommandArray_EASY = new ArrayList { button1Text, button2Text, button3Text, button4Text, button5Text, button6Text, button7Text, button8Text, button9Text, button10Text };
    public ArrayList buttonCommandArray_MEDIUM = new ArrayList { button11Text, button12Text, button13Text, button14Text, button15Text, button16Text, button17Text, button18Text, button19Text, button20Text };
    public ArrayList buttonCommandArray_HARD = new ArrayList { button21Text, button22Text, button23Text, button24Text, button25Text, button26Text, button27Text, button28Text, button29Text, button30Text };
    public ArrayList lLeverCommandArray_EASY = new ArrayList { lLever1Text, lLever2Text, lLever3Text, lLever4Text, lLever5Text, lLever6Text, lLever7Text, lLever8Text, lLever9Text, lLever10Text };
    public ArrayList lLeverCommandArray_MEDIUM = new ArrayList { lLever11Text, lLever12Text, lLever13Text, lLever14Text, lLever15Text, lLever16Text, lLever17Text, lLever18Text, lLever19Text, lLever20Text };
    public ArrayList lLeverCommandArray_HARD = new ArrayList { lLever21Text, lLever22Text, lLever23Text, lLever24Text, lLever25Text, lLever26Text, lLever27Text, lLever28Text, lLever29Text, lLever30Text };
    public ArrayList wLeverCommandArray_EASY = new ArrayList { wLever1Text, wLever2Text, wLever3Text, wLever4Text, wLever5Text, wLever6Text, wLever7Text, wLever8Text, wLever9Text, wLever10Text };
    public ArrayList wLeverCommandArray_MEDIUM = new ArrayList { wLever11Text, wLever12Text, wLever13Text, wLever14Text, wLever15Text, wLever16Text, wLever17Text, wLever18Text, wLever19Text, wLever20Text };
    public ArrayList wLeverCommandArray_HARD = new ArrayList { wLever21Text, wLever22Text, wLever23Text, wLever24Text, wLever25Text, wLever26Text, wLever27Text, wLever28Text, wLever29Text, wLever30Text };
    public ArrayList dialCommandArray_EASY = new ArrayList { dial1Text, dial2Text, dial3Text, dial4Text, dial5Text, dial6Text, dial7Text, dial8Text, dial9Text, dial10Text };
    public ArrayList dialCommandArray_MEDIUM = new ArrayList { dial11Text, dial12Text, dial13Text, dial14Text, dial15Text, dial16Text, dial17Text, dial18Text, dial19Text, dial20Text };
    public ArrayList dialCommandArray_HARD = new ArrayList { dial21Text, dial22Text, dial23Text, dial24Text, dial25Text, dial26Text, dial27Text, dial28Text, dial29Text, dial30Text };
    public ArrayList lightswitchCommandArray_EASY = new ArrayList { lightswitch1Text, lightswitch2Text, lightswitch3Text, lightswitch4Text, lightswitch5Text, lightswitch6Text, lightswitch7Text, lightswitch8Text, lightswitch9Text, lightswitch10Text };
    public ArrayList lightswitchCommandArray_MEDIUM = new ArrayList { lightswitch11Text, lightswitch12Text, lightswitch13Text, lightswitch14Text, lightswitch15Text, lightswitch16Text, lightswitch17Text, lightswitch18Text, lightswitch19Text, lightswitch20Text };
    public ArrayList lightswitchCommandArray_HARD = new ArrayList { lightswitch21Text, lightswitch22Text, lightswitch23Text, lightswitch24Text, lightswitch25Text, lightswitch26Text, lightswitch27Text, lightswitch28Text, lightswitch29Text, lightswitch30Text };
    public ArrayList shifterCommandArray_EASY = new ArrayList { shifter1Text, shifter2Text, shifter3Text, shifter4Text, shifter5Text, shifter6Text, shifter7Text, shifter8Text, shifter9Text, shifter10Text };
    public ArrayList shifterCommandArray_MEDIUM = new ArrayList { shifter11Text, shifter12Text, shifter13Text, shifter14Text, shifter15Text, shifter16Text, shifter17Text, shifter18Text, shifter19Text, shifter20Text };
    public ArrayList shifterCommandArray_HARD = new ArrayList { shifter21Text, shifter22Text, shifter23Text, shifter24Text, shifter25Text, shifter26Text, shifter27Text, shifter28Text, shifter29Text, shifter30Text };
    public ArrayList sliderCommandArray_EASY = new ArrayList { slider1Text, slider2Text, slider3Text, slider4Text, slider5Text, slider6Text, slider7Text, slider8Text, slider9Text, slider10Text };
    public ArrayList sliderCommandArray_MEDIUM = new ArrayList { slider11Text, slider12Text, slider13Text, slider14Text, slider15Text, slider16Text, slider17Text, slider18Text, slider19Text, slider20Text };
    public ArrayList sliderCommandArray_HARD = new ArrayList { slider21Text, slider22Text, slider23Text, slider24Text, slider25Text, slider26Text, slider27Text, slider28Text, slider29Text, slider30Text };
    public ArrayList valveCommandArray_EASY = new ArrayList { valve1Text, valve2Text, valve3Text, valve4Text, valve5Text, valve6Text, valve7Text, valve8Text, valve9Text, valve10Text };
    public ArrayList valveCommandArray_MEDIUM = new ArrayList { valve11Text, valve12Text, valve13Text, valve14Text, valve15Text, valve16Text, valve17Text, valve18Text, valve19Text, valve20Text };
    public ArrayList valveCommandArray_HARD = new ArrayList { valve21Text, valve22Text, valve23Text, valve24Text, valve25Text, valve26Text, valve27Text, valve28Text, valve29Text, valve30Text };
}
