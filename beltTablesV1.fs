FeatureScript 1378;
import(path : "onshape/std/geometry.fs", version : "1378.0");

// default bounds are formated as such: [lower limit, default value, upper limit].
// Thus, FIRST_PULLEY_TEETH_BOUNDS allows any number of pulley teeth between 1 and 1e50, and defaults to 24 teeth.
export const FIRST_PULLEY_TEETH_BOUNDS = { (unitless) : [1, 24, 1e50] } as IntegerBoundSpec;
export const SECOND_PULLEY_TEETH_BOUNDS = { (unitless) : [1, 36, 1e50] } as IntegerBoundSpec;
export const BELT_TEETH_BOUNDS = { (unitless) : [1, 100, 1e50] } as IntegerBoundSpec;
export const OFFSET_BOUNDS = { (inch) : [-1e50, 0, 1e50] } as LengthBoundSpec;
export const CENTERTOCENTERADJUST_BOUNDS = { (inch) : [-0.50, 0, 0.50] } as LengthBoundSpec;

// pulley default bounds
export const PULLEY_BORE_BOUNDS = { (inch) : [0, 0.5, 1e50] } as LengthBoundSpec;
export const PULLEY_FLANGE_WIDTH_BOUNDS = { (inch) : [0, 0.0625, 1e50] } as LengthBoundSpec;
export const FIT_ADJUSTMENT_BOUNDS = { (inch) : [-0.05, 0, 0.05] } as LengthBoundSpec;

// default custom pulley width for GT2 belts (update both numbers)
export const defaultGT2PulleyWidth = 0.4375 * inch;
export const PULLEY_WIDTH_BOUNDS = { (inch) : [0, 0.4375, 1e50] } as LengthBoundSpec;

// default custom pulley width for 9mm HTD belts
export const default9mmHTDPulleyWidth = 0.4375 * inch;

// default custom pulley width for 15mm HTD belts
export const default15mmHTDPulleyWidth = 0.75 * inch;

// GT2 Belts
export enum GT2BeltOptions
{
    annotation { "Name" : "VEXpro belts" }
    VEXPRO,
    annotation { "Name" : "REV belts" }
    REV,
    annotation { "Name" : "Any belt size" }
    ANY,
    annotation { "Name" : "Your custom name here", "Hidden" : true } // change to false to show first table below.
    CUSTOMONE, // internal variable name, don't change
    annotation { "Name" : "Your custom name here", "Hidden" : true } // change to false to show second table below.
    CUSTOMTWO // internal variable name, don't change
}

// default supplier tables
export const VEXproGT2BeltTable = {
        "name" : "GT2Belts",
        "displayName" : "VEXpro belts",
        "default" : "100T",
        "entries" : {
            "45T" : 45,
            "50T" : 50,
            "55T" : 55,
            "60T" : 60,
            "70T" : 70,
            "85T" : 85,
            "90T" : 90,
            "100T" : 100,
            "105T" : 105,
            "110T" : 110,
            "115T" : 115,
            "120T" : 120,
            "125T" : 125,
            "140T" : 140,
            "180T" : 180
        }
    };

export const REVBeltTable = {
        "name" : "REVBelts",
        "displayName" : "REV belts",
        "default" : "105T",
        "entries" : {
            "55T" : 55,
            "85T" : 85,
            "105T" : 105,
            "120T" : 120,
            "145T" : 145,
            "210T" : 210,
            "270T" : 270
        }
    };

// custom GT2 Tables
export const GT2CustomOneTable = {
        "name" : "CustomOneGT2", // this name is internal and won't be seen by the user
        "displayName" : "Your table name here", // enter a name of your choosing
        "default" : "55T", // choose a default belt size which is in the table
        "entries" : {
            // Remember that there should be a comma after each entry except the last one. You can have as many entries as you like.
            "55T" : 55,
            "80T Sample" : 80,
            "110T Generic Belt" : 110
        }
    };

export const GT2CustomTwoTable = {
        "name" : "CustomTwoGT2",
        "displayName" : "Your Name Here", // enter a name of your choosing
        "default" : "105T", // choose a default belt size which is in the table
        "entries" : {
            // Remember that there should be a comma after each entry except the last one. You can have as many entries as you like.
            "55T" : 55,
            "85T" : 85,
            "105T" : 105,
            "120T" : 120,
            "270T" : 270
        }
    };


// 9mm wide belts
export enum HTD9mmBeltOptions
{
    annotation { "Name" : "VEXpro belts" }
    VEXPRO,
    annotation { "Name" : "AndyMark belts" }
    ANDYMARK,
    annotation { "Name" : "Any belt size" }
    ANY,
    annotation { "Name" : "Your custom name here", "Hidden" : true } // change to false to show first table below.
    CUSTOMONE, // internal variable name, don't change
    annotation { "Name" : "Your custom name here", "Hidden" : true } // change to false to show second table below.
    CUSTOMTWO // internal variable name, don't change
}

export const HTD9mmCustomOneTable = {
        "name" : "CustomOne9mmBelts",
        "displayName" : "Your Name Here", // enter a name of your choosing
        "default" : "105T", // choose a default belt size which is in the table
        "entries" : {
            // Remember that there should be a comma after each entry except the last one. You can have as many entries as you like.
            "85T" : 85,
            "105T" : 105,
            "210T" : 210,
            "270T" : 270
        }
    };

export const HTD9mmCustomTwoTable = {
        "name" : "CustomTwo9mmBelts",
        "displayName" : "Your Name Here", // enter a name of your choosing
        "default" : "105T", // choose a default belt size which is in the table
        "entries" : {
            // Remember that there should be a comma after each entry except the last one. You can have as many entries as you like.
            "55T" : 55,
            "85T" : 85,
            "105T" : 105,
            "120T" : 120,
            "145T" : 145
        }
    };

// 15mm wide belts
export enum HTD15mmBeltOptions
{
    annotation { "Name" : "VEXpro belts" }
    VEXPRO,
    annotation { "Name" : "AndyMark belts" }
    ANDYMARK,
    annotation { "Name" : "Any belt size" }
    ANY,
    annotation { "Name" : "Your custom name here", "Hidden" : true } // change to false to show first table below.
    CUSTOMONE, // internal variable name, don't change
    annotation { "Name" : "Your custom name here", "Hidden" : true } // change to false to show second table below.
    CUSTOMTWO // internal variable name, don't change
}

export const HTD15mmCustomOneTable = {
        "name" : "CustomOne15mmBelts",
        "displayName" : "Your Name Here", // enter a name of your choosing
        "default" : "105T", // choose a default belt size which is in the table
        "entries" : {
            // Remember that there should be a comma after each entry except the last one. You can have as many entries as you like.
            "55T" : 55,
            "85T" : 85,
            "105T" : 105,
            "145T" : 145,
            "210T" : 210,
            "270T" : 270
        }
    };

export const HTD15mmCustomTwoTable = {
        "name" : "CustomTwo15mmBelts",
        "displayName" : "Your Name Here", // enter a name of your choosing
        "default" : "105T", // choose a default belt size which is in the table
        "entries" : {
            // Remember that there should be a comma after each entry except the last one. You can have as many entries as you like.
            "55T" : 55,
            "105T" : 105,
            "120T" : 120,
            "145T" : 145,
            "210T" : 210,
            "270T" : 270
        }
    };

// VEXpro sells their HTD belts in 9mm and 15mm wide sizes, so there is only one table for all their HTD offerings.
export const VEXproHTDBeltTable = {
        "name" : "HTDBelts",
        "displayName" : "VEXpro belts",
        "default" : "100T",
        "entries" : {
            "60T" : 60,
            "70T" : 70,
            "80T" : 80,
            "90T" : 90,
            "100T" : 100,
            "104T" : 104,
            "110T" : 110,
            "120T" : 120,
            "130T" : 130,
            "140T" : 140,
            "150T" : 150,
            "160T" : 160,
            "170T" : 170,
            "180T" : 180,
            "200T" : 200,
            "225T" : 225,
            "250T" : 250
        }
    };

export const AndyMark9mmBeltTable = {
        "name" : "AndyMark9mmBelts",
        "displayName" : "AndyMark belts",
        "default" : "93T",
        "entries" : {
            "40T" : 40,
            "45T" : 45,
            "48T" : 48,
            "93T" : 93
        }
    };

export const AndyMark15mmBeltTable = {
        "name" : "AndyMark15mmBelts",
        "displayName" : "AndyMark belts",
        "default" : "80T",
        "entries" : {
            "55T" : 55,
            "85T" : 85,
            "80T" : 80,
            "104T" : 104,
            "107T" : 107,
            "117T" : 117,
            "110T" : 110,
            "120T" : 120,
            "131T" : 131,
            "140T" : 140,
            "151T" : 151,
            "160T" : 160,
            "170T" : 170,
            "180T" : 180,
            "200T" : 200
        }
    };

export enum DisplayOptions
{
    annotation { "Name" : "Position" }
    POSITION,
    annotation { "Name" : "Belt" }
    BELT,
    annotation { "Name" : "Pulleys" }
    PULLEYS
}

export enum BeltType
{
    annotation { "Name" : "9mm wide GT2" }
    GT2,
    annotation { "Name" : "9mm wide HTD" }
    HTD_9mm,
    annotation { "Name" : "15mm wide HTD" }
    HTD_15mm
}

export enum ClosestBelt
{
    annotation { "Name" : "Closest belt" }
    CLOSEST,
    annotation { "Name" : "Next largest belt" }
    LARGE,
    annotation { "Name" : "Next smallest belt" }
    SMALL
}

// --- Pulley Option Enums ---
export enum PulleyType
{
    annotation { "Name" : "No pulley" }
    NONE,
    annotation { "Name" : "Custom pulley" }
    CUSTOM,
    annotation { "Name" : "VEXpro pulley", "Hidden" : true }
    VEXPRO
}

export const GT2PulleyTable = {
        "name" : "GT2Pulleys",
        "displayName" : "GT2 pulleys",
        "default" : "24T",
        "entries" : {
            "12T BAG pinion" : { "Pulley" : "GT2_12T_Bag", "Teeth" : 12 },
            "12T 550 pinion" : { "Pulley" : "GT2_12T_550", "Teeth" : 12 },
            "12T 775 pinion" : { "Pulley" : "GT2_12T_775", "Teeth" : 12 },
            "16T Falcon pinion" : { "Pulley" : "GT2_16T_Falcon", "Teeth" : 16 },
            "24T" : { "Pulley" : "GT2_24T", "Teeth" : 24 },
            "36T" : { "Pulley" : "GT2_36T", "Teeth" : 36 },
            "48T" : { "Pulley" : "GT2_48T", "Teeth" : 48 },
            "60T" : { "Pulley" : "GT2_60T", "Teeth" : 60 }
        }
    };

export const HTDPulleyTable = {
        "name" : "HTDPulleys",
        "displayName" : "HTD pulleys",
        "default" : "24T",
        "entries" : {
            "18T" : { "Pulley" : "HTD_18T", "Teeth" : 18 },
            "24T" : { "Pulley" : "HTD_24T", "Teeth" : 24 },
            "30T" : { "Pulley" : "HTD_30T", "Teeth" : 30 },
            "36T" : { "Pulley" : "HTD_36T", "Teeth" : 36 }
        }
    };

export enum BoreType
{
    annotation { "Name" : "Hex" }
    Hex,
    annotation { "Name" : "Circular" }
    Circular,
    annotation { "Name" : "VEXpro spline" }
    Spline,
    annotation { "Name" : "None" }
    None
}

export enum OffsetLocation
{
    annotation { "Name" : "Center" }
    CENTER,
    annotation { "Name" : "Pulley one" }
    PULLEYONE,
    annotation { "Name" : "Pulley two" }
    PULLEYTWO
}

