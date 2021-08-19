FeatureScript 1378;
import(path : "onshape/std/geometry.fs", version : "1378.0");

// custom icon import
// The icon was done by Eliza Barnett of Team 1745
icon::import(path : "0d5bcf3da41d146e249e87b8", version : "192a00d2c598a1b89189c556");

// belt table import
export import(path : "345e86ba1a0bf0a21325f89e", version : "13d3c24ae97cd8b7d58d0a3f");

// pulley imports
configurablePulley::import(path : "6ef3e78a020e06cf7dfea69e", version : "6c7db015789806cb28041f1b");
vexproGT2::import(path : "a54d2d53a182d3be63c6d339", version : "8077cde7e9caeac68203ae44");
vexpro9mmHTD::import(path : "47092c92c08e409e129a50eb", version : "c2f20b8c78b6f5600754b9d2");
vexpro15mmHTD::import(path : "df1d05256f2986108dfc003e", version : "49269c9c76ccbc66c81c7bd4");

annotation { "Feature Type Name" : "FRC Belt Calculator", "Editing Logic Function" : "editBeltLogic", "Icon" : icon::BLOB_DATA,
        "Feature Type Description" : "Create GT2 and HTD belts, position and size them automatically based on sketch and/or model geometry, and insert and modify custom 3D printable pulleys.<br>" ~
        "FeatureScript created by Alex Kempen."
    }
export const beltGenerator = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Display options", "UIHint" : ["HORIZONTAL_ENUM"] }
        definition.displayOption is DisplayOptions;
        // ---positioning annotations---
        if (definition.displayOption == DisplayOptions.POSITION)
        {
            annotation { "Name" : "Pulley one position", "Filter" : (EntityType.VERTEX && SketchObject.YES) || BodyType.MATE_CONNECTOR, "MaxNumberOfPicks" : 1 }
            definition.firstPoint is Query;

            annotation { "Name" : "Pulley two position", "Filter" : QueryFilterCompound.ALLOWS_AXIS || (EntityType.VERTEX && SketchObject.YES), "MaxNumberOfPicks" : 1,
                        "Description" : "Select a vertex, axis, or mate connector for pulley two to orient towards" }
            definition.secondPoint is Query;

            annotation { "Name" : "Offset location", "UIHint" : UIHint.SHOW_LABEL,
                        "Description" : "Changes the offset location to be measured from the inner face of a pulley. Does nothing if the pulley is not enabled." }
            definition.offsetLocation is OffsetLocation;

            annotation { "Name" : "Offset" }
            isLength(definition.offset, OFFSET_BOUNDS);

            annotation { "Name" : "Flip direction", "UIHint" : UIHint.OPPOSITE_DIRECTION }
            definition.flipOffset is boolean;
        }
        //---belt annotations---
        if (definition.displayOption == DisplayOptions.BELT)
        {
            annotation { "Name" : "Belt type" }
            definition.beltType is BeltType;

            annotation { "Name" : "Auto choose belt", "Default" : true }
            definition.autoBelt is boolean;

            if (definition.autoBelt)
            {
                annotation { "Name" : "Nearest belt options" }
                definition.closestBelt is ClosestBelt;
            }

            if (definition.beltType == BeltType.GT2)
            {
                annotation { "Name" : "Belt size options" }
                definition.GT2Belts is GT2BeltOptions;

                if (definition.autoBelt == false)
                {
                    if (definition.GT2Belts == GT2BeltOptions.VEXPRO)
                    {
                        annotation { "Name" : "VEXpro belts", "Lookup Table" : VEXproGT2BeltTable }
                        definition.vexproBeltsGT2 is LookupTablePath;
                    }
                    else if (definition.GT2Belts == GT2BeltOptions.REV)
                    {
                        annotation { "Name" : "REV belts", "Lookup Table" : REVBeltTable }
                        definition.revBeltsGT2 is LookupTablePath;
                    }
                    else if (definition.GT2Belts == GT2BeltOptions.ANY)
                    {
                        annotation { "Name" : "Belt teeth" }
                        isInteger(definition.GT2BeltTeeth, BELT_TEETH_BOUNDS);
                    }
                    else if (definition.GT2Belts == GT2BeltOptions.CUSTOMONE)
                    {
                        annotation { "Name" : "Custom GT2 Belts", "Lookup Table" : GT2CustomOneTable }
                        definition.customOneGT2 is LookupTablePath;
                    }
                    else if (definition.GT2Belts == GT2BeltOptions.CUSTOMTWO)
                    {
                        annotation { "Name" : "Custom GT2 Belts", "Lookup Table" : GT2CustomTwoTable }
                        definition.customTwoGT2 is LookupTablePath;
                    }
                }
            }
            else if (definition.beltType == BeltType.HTD_9mm)
            {
                annotation { "Name" : "Belt size options" }
                definition.HTD9mmBelts is HTD9mmBeltOptions;

                if (definition.autoBelt == false)
                {
                    if (definition.HTD9mmBelts == HTD9mmBeltOptions.VEXPRO)
                    {
                        annotation { "Name" : "VEXpro belts", "Lookup Table" : VEXproHTDBeltTable }
                        definition.vexpro9mmBelts is LookupTablePath;
                    }
                    else if (definition.HTD9mmBelts == HTD9mmBeltOptions.ANDYMARK)
                    {
                        annotation { "Name" : "AndyMark belts", "Lookup Table" : AndyMark9mmBeltTable }
                        definition.andymark9mmBelts is LookupTablePath;
                    }
                    else if (definition.HTD9mmBelts == HTD9mmBeltOptions.ANY)
                    {
                        annotation { "Name" : "Belt teeth" }
                        isInteger(definition.HTD9mmBeltTeeth, BELT_TEETH_BOUNDS);
                    }
                    else if (definition.HTD9mmBelts == HTD9mmBeltOptions.CUSTOMONE)
                    {
                        annotation { "Name" : "Custom 9mm HTD Belts", "Lookup Table" : HTD9mmCustomOneTable }
                        definition.customOne9mmHTD is LookupTablePath;
                    }
                    else if (definition.HTD9mmBelts == HTD9mmBeltOptions.CUSTOMTWO)
                    {
                        annotation { "Name" : "Custom 9mm HTD Belts", "Lookup Table" : HTD9mmCustomTwoTable }
                        definition.customTwo9mmHTD is LookupTablePath;
                    }
                }
            }
            else if (definition.beltType == BeltType.HTD_15mm)
            {
                annotation { "Name" : "Belt size options" }
                definition.HTD15mmBelts is HTD15mmBeltOptions;

                if (definition.autoBelt == false)
                {
                    if (definition.HTD15mmBelts == HTD15mmBeltOptions.VEXPRO)
                    {
                        annotation { "Name" : "VEXpro belts", "Lookup Table" : VEXproHTDBeltTable }
                        definition.vexpro15mmBelts is LookupTablePath;
                    }
                    else if (definition.HTD15mmBelts == HTD15mmBeltOptions.ANDYMARK)
                    {
                        annotation { "Name" : "AndyMark belts", "Lookup Table" : AndyMark9mmBeltTable }
                        definition.andymark15mmBelts is LookupTablePath;
                    }
                    else if (definition.HTD15mmBelts == HTD15mmBeltOptions.ANY)
                    {
                        annotation { "Name" : "Belt teeth" }
                        isInteger(definition.HTD15mmBeltTeeth, BELT_TEETH_BOUNDS);
                    }
                    else if (definition.HTD15mmBelts == HTD15mmBeltOptions.CUSTOMONE)
                    {
                        annotation { "Name" : "Custom 15mm HTD Belts", "Lookup Table" : HTD15mmCustomOneTable }
                        definition.customOne15mmHTD is LookupTablePath;
                    }
                    else if (definition.HTD15mmBelts == HTD15mmBeltOptions.CUSTOMTWO)
                    {
                        annotation { "Name" : "Custom 15mm HTD Belts", "Lookup Table" : HTD15mmCustomTwoTable }
                        definition.customTwo15mmHTD is LookupTablePath;
                    }
                }
            }
            annotation { "Name" : "Enable belt teeth (slow)", "Default" : false }
            definition.showTeeth is boolean;

            annotation { "Name" : "Center to center adjustment",
                        "Description" : "Adjusts the checked center to center distance. Useful for making slight adjustments to the tension of belts."
                    }
            isLength(definition.centerToCenterAdjustment, CENTERTOCENTERADJUST_BOUNDS);
        }

        if (definition.displayOption == DisplayOptions.PULLEYS)
        {
            // ---pulley annotations---
            annotation { "Group Name" : "Pulley one", "Collapsed By Default" : false }
            {
                annotation { "Name" : "Pulley one type" }
                definition.pulleyOneType is PulleyType;

                // ---pulley one---
                if (definition.pulleyOneType == PulleyType.CUSTOM || definition.pulleyOneType == PulleyType.NONE)
                {
                    annotation { "Name" : "Pulley one teeth" }
                    isInteger(definition.pulleyOneTeeth, FIRST_PULLEY_TEETH_BOUNDS);
                }
                else
                {
                    if (definition.beltType == BeltType.GT2)
                    {
                        annotation { "Name" : "VEXpro pulleys", "Lookup Table" : GT2PulleyTable }
                        definition.pulleyOneTeethVexproGT2 is LookupTablePath;
                    }
                    else
                    {
                        annotation { "Name" : "HTD pulleys", "Lookup Table" : HTDPulleyTable }
                        definition.pulleyOneTeethVexproHTD is LookupTablePath;
                    }
                }

                // ---pulley one options---
                if (definition.pulleyOneType == PulleyType.CUSTOM)
                {
                    annotation { "Name" : "Pulley width" }
                    isLength(definition.pulleyOneWidth, PULLEY_WIDTH_BOUNDS);

                    annotation { "Name" : "Bore type", "UIHint" : "SHOW_LABEL" }
                    definition.pulleyOneBoreType is BoreType;

                    if (definition.pulleyOneBoreType == BoreType.Hex || definition.pulleyOneBoreType == BoreType.Circular)
                    {
                        annotation { "Name" : "Bore diameter" }
                        isLength(definition.pulleyOneBoreDiameter, PULLEY_BORE_BOUNDS);
                    }

                    annotation { "Name" : "Enable flanges", "Default" : true }
                    definition.pulleyOneEnableFlange is boolean;

                    if (definition.pulleyOneEnableFlange)
                    {
                        annotation { "Name" : "Flange width" }
                        isLength(definition.pulleyOneFlangeWidth, PULLEY_FLANGE_WIDTH_BOUNDS);
                    }
                }
            }
            // ---pulley two---
            annotation { "Group Name" : "Pulley two", "Collapsed By Default" : false }
            {
                annotation { "Name" : "Pulley two type" }
                definition.pulleyTwoType is PulleyType;

                if (definition.pulleyTwoType == PulleyType.CUSTOM || definition.pulleyTwoType == PulleyType.NONE)
                {
                    annotation { "Name" : "Pulley two teeth" }
                    isInteger(definition.pulleyTwoTeeth, FIRST_PULLEY_TEETH_BOUNDS);
                }
                else
                {
                    if (definition.beltType == BeltType.GT2)
                    {
                        annotation { "Name" : "VEXpro pulleys", "Lookup Table" : GT2PulleyTable }
                        definition.pulleyTwoTeethVexproGT2 is LookupTablePath;
                    }
                    else
                    {
                        annotation { "Name" : "HTD pulleys", "Lookup Table" : HTDPulleyTable }
                        definition.pulleyTwoTeethVexproHTD is LookupTablePath;
                    }
                }
                //---pulley two options---
                if (definition.pulleyTwoType == PulleyType.CUSTOM)
                {
                    annotation { "Name" : "Pulley width" }
                    isLength(definition.pulleyTwoWidth, PULLEY_WIDTH_BOUNDS);

                    annotation { "Name" : "Bore type", "UIHint" : "SHOW_LABEL" }
                    definition.pulleyTwoBoreType is BoreType;

                    if (definition.pulleyTwoBoreType == BoreType.Hex || definition.pulleyTwoBoreType == BoreType.Circular)
                    {
                        annotation { "Name" : "Bore diameter" }
                        isLength(definition.pulleyTwoBoreDiameter, PULLEY_BORE_BOUNDS);
                    }

                    annotation { "Name" : "Enable flanges", "Default" : true }
                    definition.pulleyTwoEnableFlange is boolean;

                    if (definition.pulleyTwoEnableFlange)
                    {
                        annotation { "Name" : "Flange width" }
                        isLength(definition.pulleyTwoFlangeWidth, PULLEY_FLANGE_WIDTH_BOUNDS);
                    }
                }
            }

            if (definition.pulleyOneType == PulleyType.CUSTOM || definition.pulleyTwoType == PulleyType.CUSTOM)
            {
                annotation { "Name" : "Pulley teeth size adjustment",
                            "Description" : "Slightly adjusts the size of the teeth profile of the pulleys."
                        }
                isLength(definition.fitAdjustment, FIT_ADJUSTMENT_BOUNDS);

                annotation { "Name" : "Create composite part", "Default" : false,
                            "Description" : "Creates a composite part of the belt and pulleys. Useful for assembling multiple copies of the same belt run rapidly." }
                definition.createComposite is boolean;
            }
        }
    } // end of precondition
    {
        var referencesToTransform = [definition.firstPoint, definition.secondPoint];
        var remainingTransform = getRemainderPatternTransform(context, { "references" : qUnion(referencesToTransform) });

        // ---offset adjustments---
        if (definition.offsetLocation == OffsetLocation.PULLEYONE)
        {
            if (definition.pulleyOneType == PulleyType.CUSTOM)
            {
                definition.offset = definition.offset + definition.pulleyOneWidth / 2;

                if (definition.pulleyOneEnableFlange)
                {
                    definition.offset = definition.offset + definition.pulleyOneFlangeWidth;
                }
            }
        }
        else if (definition.offsetLocation == OffsetLocation.PULLEYTWO)
        {
            if (definition.pulleyTwoType == PulleyType.CUSTOM)
            {
                definition.offset = definition.offset + definition.pulleyTwoWidth / 2;

                if (definition.pulleyTwoEnableFlange)
                {
                    definition.offset = definition.offset + definition.pulleyTwoFlangeWidth;
                }
            }
        }

        if (definition.pulleyOneType == PulleyType.VEXPRO)
        {
            if (definition.beltType == BeltType.GT2)
            {
                definition.offset = definition.offset + 0.625 * inch / 2;
            }
            else if (definition.beltType == BeltType.HTD_9mm)
            {
                definition.offset = definition.offset + 14.5 * millimeter / 2;
            }
            else if (definition.beltType == BeltType.HTD_15mm)
            {
                definition.offset = definition.offset + 22 * millimeter / 2;
            }
        }
        // reverse direction when definition.flipOffset is clicked
        definition.offset = definition.offset * (definition.flipOffset ? -1 : 1);

        // ---selection determinations---
        // sketch plane variables
        var beltOrigin is Vector = YZ_PLANE.origin;

        var beltDefaultPlane is Plane = YZ_PLANE;

        var zDirection is Vector = YZ_PLANE.normal;
        var xDirection is Vector = YZ_PLANE.x;

        if (evaluateQuery(context, qSketchFilter(definition.firstPoint, SketchObject.YES)) != [])
        // if a first point has been selected, and is from a sketch:
        {
            beltOrigin = evVertexPoint(context, { "vertex" : definition.firstPoint });

            zDirection = evOwnerSketchPlane(context, { "entity" : definition.firstPoint }).normal;

            xDirection = evOwnerSketchPlane(context, { "entity" : definition.firstPoint }).x;

            beltDefaultPlane = evOwnerSketchPlane(context, { "entity" : definition.firstPoint });
        }

        if (evaluateQuery(context, definition.firstPoint) != [] && evaluateQuery(context, qBodyType(definition.firstPoint, BodyType.MATE_CONNECTOR)) != [])
        // if a first point has been selected, and is a mate connector:
        {
            var mateConnectorCoords = evMateConnector(context, { "mateConnector" : definition.firstPoint });

            beltOrigin = mateConnectorCoords.origin;

            zDirection = mateConnectorCoords.zAxis;

            xDirection = mateConnectorCoords.xAxis;

            beltDefaultPlane = plane(mateConnectorCoords);
        }
        var pulleyOrigin is Vector = beltOrigin - (definition.offset * zDirection);
        beltDefaultPlane.Origin = pulleyOrigin;
        var pulleyOneCoords is CoordSystem = coordSystem(pulleyOrigin, -xDirection, zDirection);

        var secondPoint;
        if (evaluateQuery(context, definition.secondPoint) != [])
        // if a second point has been selected:
        {
            if (evaluateQuery(context, qEntityFilter(definition.secondPoint, EntityType.VERTEX)) != [])
            // if the second point is a vertex:
            {
                // get vertex point
                secondPoint = project(beltDefaultPlane, evVertexPoint(context, { "vertex" : definition.secondPoint }));
            }
            else
            // (second point is an axis):
            {
                // get rotation axis
                var axis is Line = evAxis(context, { "axis" : definition.secondPoint });

                // evaluate axis intersection with default plane
                var axisIntersection is LinePlaneIntersection = intersection(beltDefaultPlane, axis);
                if (axisIntersection.dim == 0)
                // if the axis interesects the plane at one point:
                {
                    secondPoint = axisIntersection.intersection;
                }
                else if (axisIntersection.dim == 1)
                // if the selected axis is collinear with the belt:
                {
                    throw regenError("Selected axis is collinear with belt.", definition.secondPoint);
                }
                else if (axisIntersection.dim == -1)
                // if the selected axis does not intersect the belt:
                {
                    throw regenError("Selected axis does not intersect belt.", definition.secondPoint);
                }
            }
            if (beltOrigin == secondPoint)
            {
                throw regenError("Selections return same vertex", qUnion([definition.firstPoint, definition.secondPoint]));
            }
            xDirection = beltOrigin - secondPoint;
        }

        beltOrigin -= (definition.offset * zDirection);
        var beltCoords is CoordSystem = coordSystem(beltOrigin, -xDirection, zDirection);
        var sketchPlane is Plane = plane(beltCoords);

        // ---belt variable assignment---
        // variables which depend on belt type
        var beltType;
        var teethPitch;
        var beltTeeth;
        var beltWidth;
        var beltArray;

        if (definition.beltType == BeltType.GT2)
        {
            beltType = "GT2";
            teethPitch = 3 * millimeter;
            beltWidth = 9 * millimeter;

            if (definition.GT2Belts == GT2BeltOptions.VEXPRO)
            {
                beltTeeth = getLookupTable(VEXproGT2BeltTable, definition.vexproBeltsGT2) * unitless;
                beltArray = sort(values(values(VEXproGT2BeltTable)[2]), function(a, b)
                    {
                        return a - b;
                    });
            }
            else if (definition.GT2Belts == GT2BeltOptions.REV)
            {
                beltTeeth = getLookupTable(REVBeltTable, definition.revBeltsGT2) * unitless;
                beltArray = sort(values(values(REVBeltTable)[2]), function(a, b)
                    {
                        return a - b;
                    });
            }
            else if (definition.GT2Belts == GT2BeltOptions.ANY)
            {
                beltTeeth = definition.GT2BeltTeeth * unitless;
            }
            else if (definition.GT2Belts == GT2BeltOptions.CUSTOMONE)
            {
                println(GT2CustomOneTable);
                println(beltArray);
                beltTeeth = getLookupTable(GT2CustomOneTable, definition.customOneGT2) * unitless;
                beltArray = sort(values(values(GT2CustomOneTable)[2]), function(a, b)
                    {
                        return a - b;
                    });

            }
            else if (definition.GT2Belts == GT2BeltOptions.CUSTOMTWO)
            {
                beltTeeth = getLookupTable(GT2CustomTwoTable, definition.customTwoGT2) * unitless;
                beltArray = sort(values(values(GT2CustomTwoTable)[2]), function(a, b)
                    {
                        return a - b;
                    });
            }
        }
        else if (definition.beltType == BeltType.HTD_9mm)
        {
            beltType = "HTD";
            teethPitch = 5 * millimeter;
            beltWidth = 9 * millimeter;
            if (definition.HTD9mmBelts == HTD9mmBeltOptions.VEXPRO)
            {
                beltTeeth = getLookupTable(VEXproHTDBeltTable, definition.vexpro9mmBelts) * unitless;
                beltArray = sort(values(values(VEXproHTDBeltTable)[2]), function(a, b)
                    {
                        return a - b;
                    });
            }
            else if (definition.HTD9mmBelts == HTD9mmBeltOptions.ANDYMARK)
            {
                beltTeeth = getLookupTable(AndyMark9mmBeltTable, definition.andymark9mmBelts) * unitless;
                beltArray = sort(values(values(AndyMark9mmBeltTable)[2]), function(a, b)
                    {
                        return a - b;
                    });
            }
            else if (definition.HTD9mmBelts == HTD9mmBeltOptions.ANY)
            {
                beltTeeth = definition.HTD9mmBeltTeeth * unitless;
            }
            else if (definition.HTD9mmBelts == HTD9mmBeltOptions.CUSTOMONE)
            {
                beltTeeth = getLookupTable(HTD9mmCustomOneTable, definition.customOne9mmHTD) * unitless;
                beltArray = sort(values(values(HTD9mmCustomOneTable)[2]), function(a, b)
                    {
                        return a - b;
                    });
            }
            else if (definition.HTD9mmBelts == HTD9mmBeltOptions.CUSTOMTWO)
            {
                beltTeeth = getLookupTable(HTD9mmCustomTwoTable, definition.customTwo9mmHTD) * unitless;
                beltArray = sort(values(values(HTD9mmCustomTwoTable)[2]), function(a, b)
                    {
                        return a - b;
                    });
            }
        }
        else if (definition.beltType == BeltType.HTD_15mm)
        {
            beltType = "HTD";
            teethPitch = 5 * millimeter;
            beltWidth = 15 * millimeter;
            if (definition.HTD15mmBelts == HTD15mmBeltOptions.VEXPRO)
            {
                beltTeeth = getLookupTable(VEXproHTDBeltTable, definition.vexpro15mmBelts) * unitless;
                beltArray = sort(values(values(VEXproHTDBeltTable)[2]), function(a, b)
                    {
                        return a - b;
                    });
            }
            else if (definition.HTD15mmBelts == HTD15mmBeltOptions.ANDYMARK)
            {
                beltTeeth = getLookupTable(AndyMark15mmBeltTable, definition.andymark15mmBelts) * unitless;
                beltArray = sort(values(values(AndyMark15mmBeltTable)[2]), function(a, b)
                    {
                        return a - b;
                    });
            }
            else if (definition.HTD15mmBelts == HTD15mmBeltOptions.ANY)
            {
                beltTeeth = definition.HTD15mmBeltTeeth * unitless;
            }
            else if (definition.HTD15mmBelts == HTD15mmBeltOptions.CUSTOMONE)
            {
                beltTeeth = getLookupTable(HTD15mmCustomOneTable, definition.customOne15mmHTD) * unitless;
                beltArray = sort(values(values(HTD15mmCustomOneTable)[2]), function(a, b)
                    {
                        return a - b;
                    });
            }
            else if (definition.HTD15mmBelts == HTD15mmBeltOptions.CUSTOMTWO)
            {
                beltTeeth = getLookupTable(HTD15mmCustomTwoTable, definition.customTwo15mmHTD) * unitless;
                beltArray = sort(values(values(HTD15mmCustomTwoTable)[2]), function(a, b)
                    {
                        return a - b;
                    });
            }
        }


        var beltThickness is ValueWithUnits = teethPitch / 3;
        var teethRadius is ValueWithUnits = teethPitch / 4;

        // ---pulley teeth---
        var pulleyOneTeeth;
        var pulleyTwoTeeth;

        if (definition.pulleyOneType == PulleyType.CUSTOM || definition.pulleyOneType == PulleyType.NONE)
        {
            pulleyOneTeeth = definition.pulleyOneTeeth;
        }
        else if (definition.pulleyOneType == PulleyType.VEXPRO)
        {
            if (beltType == "GT2")
            {
                pulleyOneTeeth = getLookupTable(GT2PulleyTable, definition.pulleyOneTeethVexproGT2).Teeth;
            }
            else if (beltType == "HTD")
            {
                pulleyOneTeeth = getLookupTable(HTDPulleyTable, definition.pulleyOneTeethVexproHTD).Teeth;
            }
        }

        if (definition.pulleyTwoType == PulleyType.CUSTOM || definition.pulleyTwoType == PulleyType.NONE)
        {
            pulleyTwoTeeth = definition.pulleyTwoTeeth;
        }
        else if (definition.pulleyTwoType == PulleyType.VEXPRO)
        {
            if (beltType == "GT2")
            {
                pulleyTwoTeeth = getLookupTable(GT2PulleyTable, definition.pulleyTwoTeethVexproGT2).Teeth;
            }
            else if (beltType == "HTD")
            {
                pulleyTwoTeeth = getLookupTable(HTDPulleyTable, definition.pulleyTwoTeethVexproHTD).Teeth;
            }
        }

        var pulleyOneDiameter is ValueWithUnits = (pulleyOneTeeth * teethPitch / PI);
        var pulleyTwoDiameter is ValueWithUnits = (pulleyTwoTeeth * teethPitch / PI);

        // ---automatic belt selector---
        // determine distance between selections
        var selectionDistance;

        if (evaluateQuery(context, definition.firstPoint) != [] && evaluateQuery(context, definition.secondPoint) != [])
        {
            secondPoint = secondPoint - (definition.offset * zDirection);

            selectionDistance = (evDistance(context, {
                                "side0" : beltOrigin,
                                "side1" : secondPoint
                            })).distance;
        }

        var calculatedTeeth;
        var bestBelt is number = 0;
        var largePulley;
        var smallPulley;

        // determine which pulley is larger than the other. Neccessary since the calculatedTeeth formula is non-reversible.
        if (pulleyOneDiameter < pulleyTwoDiameter)
        {
            largePulley = pulleyTwoDiameter;
            smallPulley = pulleyOneDiameter;
        }
        else
        {
            largePulley = pulleyOneDiameter;
            smallPulley = pulleyTwoDiameter;
        }

        if (definition.autoBelt)
        {
            if (evaluateQuery(context, definition.firstPoint) != [] && evaluateQuery(context, definition.secondPoint) != [])
            {
                calculatedTeeth = (largePulley ^ 2 - 2 * largePulley * smallPulley + smallPulley ^ 2 + 8 * selectionDistance ^ 2 + 2 * largePulley * selectionDistance * PI + 2 * smallPulley * selectionDistance * PI) / (4 * selectionDistance * teethPitch);

                if ((beltType == "GT2" && definition.GT2Belts == GT2BeltOptions.ANY) || (definition.beltType == BeltType.HTD_9mm && definition.HTD9mmBelts == HTD9mmBeltOptions.ANY) || (definition.beltType == BeltType.HTD_15mm && definition.HTD15mmBelts == HTD15mmBeltOptions.ANY))
                {
                    if (definition.closestBelt == ClosestBelt.CLOSEST)
                    {
                        bestBelt = round(calculatedTeeth);
                    }

                    if (definition.closestBelt == ClosestBelt.LARGE)
                    {
                        bestBelt = ceil(calculatedTeeth);
                    }

                    if (definition.closestBelt == ClosestBelt.SMALL)
                    {
                        bestBelt = floor(calculatedTeeth);
                    }

                }
                else
                {
                    if (definition.closestBelt == ClosestBelt.CLOSEST)
                    {
                        for (var belt in beltArray)
                        {
                            if (belt <= calculatedTeeth || belt == 0)
                            {
                                bestBelt = belt;
                            }
                            else
                            {
                                if (abs(bestBelt - calculatedTeeth) >= abs(belt - calculatedTeeth))
                                // smaller belt, larger belt
                                {
                                    bestBelt = belt;
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (definition.closestBelt == ClosestBelt.LARGE)
                    {
                        for (var belt in beltArray)
                        {
                            if (belt >= calculatedTeeth || belt == beltArray[(size(beltArray) - 1)])
                            {
                                bestBelt = belt;
                                break;
                            }
                        }
                    }

                    if (definition.closestBelt == ClosestBelt.SMALL)
                    {
                        for (var belt in beltArray)
                        {
                            if (belt <= calculatedTeeth || belt == 0)
                            {
                                bestBelt = belt;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                beltTeeth = bestBelt;
            }
            else
            {
                reportFeatureInfo(context, id, "You must make both position selections before the belt size can be choosen automatically.");
            }
        }

        var centerToCenter is ValueWithUnits = (((beltTeeth * teethPitch) - ((PI / 2) * (largePulley + smallPulley))) / 4 + sqrt((((beltTeeth * teethPitch) - ((PI * unitless / 2) * (largePulley + smallPulley))) / 4) ^ 2 - ((largePulley - smallPulley) ^ 2) / 8));

        centerToCenter = centerToCenter + definition.centerToCenterAdjustment;

        // ---center to center related readouts and warnings---
        if (evaluateQuery(context, definition.firstPoint) != [] && evaluateQuery(context, definition.secondPoint) != [])
        {
            if (definition.autoBelt && bestBelt != 0)
            {
                if (selectionDistance < centerToCenter - 0.001 * inch || selectionDistance > centerToCenter + 0.001 * inch)
                {
                    reportFeatureWarning(context, id, "The automatically choosen " ~ beltTeeth ~ "T belt's center to center distance (" ~ (roundToPrecision(centerToCenter / inch, 3)) ~ " inches) is not equal to the distance between selections.");
                }
                else
                {
                    reportFeatureInfo(context, id, "The automatically choosen " ~ beltTeeth ~ "T belt matches the distance between selections.");
                }
            }
            else
            {
                if (selectionDistance > centerToCenter + 0.001 * inch || selectionDistance < centerToCenter - 0.001 * inch)
                {
                    reportFeatureWarning(context, id, "The choosen belt's center to center distance (" ~ (roundToPrecision(centerToCenter / inch, 3)) ~ " inches) is not equal to the distance between selections.");
                }
                else
                {
                    reportFeatureInfo(context, id, "The choosen belt matches the distance between selections.");
                }
            }
        }
        else
        {
            if (definition.autoBelt == false)
            {
                reportFeatureInfo(context, id, "The belt center to center distance is " ~ (roundToPrecision(centerToCenter / inch, 3)) ~ " inches.");
            }
        }

        // tangent and arc math
        // https://gieseanw.wordpress.com/2012/09/12/finding-external-tangent-points-for-two-circles/
        // tangentLength = D and H, tangentToSmallCenter = Y

        // radii of pulleyOneDiameter and pulleyTwoDiameter
        var pulleyOneRadius = pulleyOneDiameter / 2;
        var pulleyTwoRadius = pulleyTwoDiameter / 2;

        // center to center too small error
        if ((pulleyOneRadius + pulleyTwoRadius) > centerToCenter)
        {
            reportFeatureWarning(context, id, "The center to center distance of the belt (" ~ (roundToPrecision(centerToCenter / inch, 3)) ~ " inches) is too small for the selected pulleys.");
        }

        // pulley bore size error
        if (definition.pulleyOneBoreDiameter >= pulleyOneDiameter && definition.pulleyOneType == PulleyType.CUSTOM && (definition.pulleyOneBoreType == BoreType.Hex || definition.pulleyOneBoreType == BoreType.Circular))
        {
            reportFeatureWarning(context, id, "The bore diameter of pulley one is too large.");
        }
        if (definition.pulleyTwoBoreDiameter >= pulleyTwoDiameter && definition.pulleyTwoType == PulleyType.CUSTOM && (definition.pulleyTwoBoreType == BoreType.Hex || definition.pulleyTwoBoreType == BoreType.Circular))
        {
            reportFeatureWarning(context, id, "The bore diameter of pulley two is too large.");
        }

        // pythagorean theorem math
        pulleyOneDiameter -= (beltType == "GT2" ? 0.03 * inch : 0.045 * inch);
        pulleyTwoDiameter -= (beltType == "GT2" ? 0.03 * inch : 0.045 * inch);
        pulleyOneRadius = pulleyOneDiameter / 2;
        pulleyTwoRadius = pulleyTwoDiameter / 2;

        var tangentLength = sqrt((centerToCenter ^ 2) - ((pulleyOneRadius - pulleyTwoRadius) ^ 2));
        var tangentToCenter = sqrt(tangentLength ^ 2 + pulleyTwoRadius ^ 2);

        // law of cosines
        var theta = acos((pulleyOneRadius ^ 2 + centerToCenter ^ 2 - tangentToCenter ^ 2) / (2 * pulleyOneRadius * centerToCenter));

        // x and y-axis shifts
        var horizontalShift = pulleyOneRadius * cos(theta);
        var smallHorizontalShift = (pulleyTwoDiameter / pulleyOneDiameter) * horizontalShift + centerToCenter;

        var verticalShift = pulleyOneRadius * sin(theta);
        var smallVerticalShift = (pulleyTwoDiameter / pulleyOneDiameter) * verticalShift;

        // ---belt sketch---
        var beltSketch = newSketchOnPlane(context, id + "beltSketch", {
                "sketchPlane" : sketchPlane
            });

        // initial belt profile
        // draws the outer contour of the belt precisely
        skLineSegment(beltSketch, "line1", {
                    "start" : vector(horizontalShift, verticalShift), //upper point
                    "end" : vector(smallHorizontalShift, smallVerticalShift) //upper right point
                });
        skLineSegment(beltSketch, "line2", {
                    "start" : vector(horizontalShift, -verticalShift), //bottom point
                    "end" : vector(smallHorizontalShift, -smallVerticalShift) //bottom right point
                });
        skArc(beltSketch, "arc1", {
                    "start" : vector(horizontalShift, verticalShift), //upper point
                    "mid" : vector(-pulleyOneRadius, 0 * millimeter), //leftmost point
                    "end" : vector(horizontalShift, -verticalShift) //bottom point
                });
        skArc(beltSketch, "arc2", {
                    "start" : vector(smallHorizontalShift, smallVerticalShift), //upper right point
                    "mid" : vector(pulleyTwoRadius + centerToCenter, 0 * millimeter), //rightmost point
                    "end" : vector(smallHorizontalShift, -smallVerticalShift) //bottom right point
                });

        // point where second pulley goes
        skPoint(beltSketch, "point1", { "position" : vector(centerToCenter, 0 * millimeter) });

        // finish beltSketch
        skSolve(beltSketch);

        // second pulley coordinates
        var pulleyTwoOrigin is Vector = evVertexPoint(context, {
                "vertex" : qBodyType(sketchEntityQuery(id + "beltSketch", EntityType.VERTEX, "point1"), BodyType.POINT)
            });
        var pulleyTwoPlane is Plane = plane(pulleyTwoOrigin, zDirection);
        var pulleyTwoCoords is CoordSystem = coordSystem(pulleyTwoPlane);

        // create a surface of the belt profile
        opExtrude(context, id + "extrude1", {
                    "entities" : qBodyType(qCreatedBy(id + "beltSketch", EntityType.EDGE), BodyType.WIRE),
                    "direction" : zDirection,
                    "endBound" : BoundingType.BLIND,
                    "endDepth" : 1 / 2 * beltWidth,
                    "startBound" : BoundingType.BLIND,
                    "startDepth" : 1 / 2 * beltWidth
                });

        // create the outer belt profile by thickening the surface of the belt profile
        opThicken(context, id + "thicken1", {
                    "entities" : qBodyType(qCreatedBy(id + "extrude1", EntityType.BODY), BodyType.SHEET),
                    "thickness1" : beltThickness,
                    "thickness2" : 0 * millimeter
                });

        // delete the surface of the belt profile
        opDeleteBodies(context, id + "deleteBodies1", {
                    "entities" : qCreatedBy(id + "extrude1")
                });

        // ---create belt teeth---
        // runs if showTeeth is toggled
        if (definition.showTeeth)
        {
            //create belt teeth sketch
            var teethSketch = newSketchOnPlane(context, id + "teethSketch", {
                    "sketchPlane" : sketchPlane
                });

            // create tooth circle to the left of pulley one
            skCircle(teethSketch, "circle1", {
                        "center" : vector(-pulleyOneRadius, 0 * millimeter),
                        "radius" : teethRadius
                    });

            //finish teethSketch
            skSolve(teethSketch);

            // extrude tooth circle
            opExtrude(context, id + "extrude2", {
                        "entities" : qCreatedBy(id + "teethSketch", EntityType.FACE),
                        "direction" : zDirection,
                        "endBound" : BoundingType.BLIND,
                        "endDepth" : 1 / 2 * beltWidth,
                        "startBound" : BoundingType.BLIND,
                        "startDepth" : 1 / 2 * beltWidth
                    });

            // pattern the extrusion of the tooth circle
            curvePattern(context, id + "curvePattern1", {
                        "patternType" : PatternType.PART,
                        "entities" : qCreatedBy(id + "extrude2", EntityType.BODY),
                        "defaultScope" : false,
                        "booleanScope" : qCreatedBy(id + "thicken1", EntityType.BODY),
                        "operationType" : NewBodyOperationType.ADD,
                        "edges" : qBodyType(qCreatedBy(id + "beltSketch", EntityType.EDGE), BodyType.WIRE),
                        "instanceCount" : beltTeeth
                    });
        }

        // delete all sketches created by Featurescript
        opDeleteBodies(context, id + "deleteSketches", { "entities" : qSketchFilter(qCreatedBy(id, EntityType.BODY), SketchObject.YES) });

        // ---pulley imports---
        const instantiator = newInstantiator(id + "instantiator");

        // custom pulley one
        if (definition.pulleyOneType == PulleyType.CUSTOM)
        {
            addInstance(instantiator, configurablePulley::build, {
                        "configuration" : {
                            "Pulley_Type" : beltType,
                            "Teeth" : pulleyOneTeeth,
                            "Size_Mod" : definition.fitAdjustment,
                            "Width" : definition.pulleyOneWidth,
                            "Bore" : definition.pulleyOneBoreType,
                            "Bore_Diameter" : definition.pulleyOneBoreDiameter,
                            "Flanged" : definition.pulleyOneEnableFlange,
                            "Flange_Width" : definition.pulleyOneFlangeWidth,
                            "Mate_Connectors" : false // set to true to add outer mate connectors to custom pulleys
                        },
                        "transform" : transform(toWorld(pulleyOneCoords)),
                        "name" : "pulleyone"
                    });
        }

        // custom pulley two
        if (definition.pulleyTwoType == PulleyType.CUSTOM)
        {
            addInstance(instantiator, configurablePulley::build, {
                        "configuration" : {
                            "Pulley_Type" : beltType,
                            "Teeth" : pulleyTwoTeeth,
                            "Size_Mod" : definition.fitAdjustment,
                            "Width" : definition.pulleyTwoWidth,
                            "Bore" : definition.pulleyTwoBoreType,
                            "Bore_Diameter" : definition.pulleyTwoBoreDiameter,
                            "Flanged" : definition.pulleyTwoEnableFlange,
                            "Flange_Width" : definition.pulleyTwoFlangeWidth,
                            "Mate_Connectors" : false // set to true to add outer mate connectors to custom pulleys
                        },
                        "transform" : transform(toWorld(pulleyTwoCoords)),
                        "name" : "pulleytwo"
                    });
        }

        // VEXpro pulley one
        if (definition.pulleyOneType == PulleyType.VEXPRO)
        {
            if (definition.beltType == BeltType.GT2)
            {
                addInstance(instantiator, vexproGT2::build, {
                            "configuration" : {
                                "Pulley_type" : getLookupTable(GT2PulleyTable, definition.pulleyOneTeethVexproGT2).Pulley,
                            },
                            "transform" : transform(toWorld(pulleyOneCoords)),
                            "name" : "pulleyone"
                        });
            }
            else if (definition.beltType == BeltType.HTD_9mm)
            {
                addInstance(instantiator, vexpro9mmHTD::build, {
                            "configuration" : {
                                "Pulley_type" : getLookupTable(HTDPulleyTable, definition.pulleyOneTeethVexproHTD).Pulley
                            },
                            "transform" : transform(toWorld(pulleyOneCoords)),
                            "name" : "pulleyone"
                        });
            }
            else if (definition.beltType == BeltType.HTD_15mm)
            {
                addInstance(instantiator, vexpro15mmHTD::build, {
                            "configuration" : {
                                "Pulley_type" : getLookupTable(HTDPulleyTable, definition.pulleyOneTeethVexproHTD).Pulley
                            },
                            "transform" : transform(toWorld(pulleyOneCoords)),
                            "name" : "pulleyone"
                        });
            }
        }

        // VEXpro pulley two
        if (definition.pulleyTwoType == PulleyType.VEXPRO)
        {
            if (definition.beltType == BeltType.GT2)
            {
                addInstance(instantiator, vexproGT2::build, {
                            "configuration" : {
                                "Pulley_type" : getLookupTable(GT2PulleyTable, definition.pulleyTwoTeethVexproGT2).Pulley
                            },
                            "transform" : transform(toWorld(pulleyTwoCoords)),
                            "name" : "pulleytwo"
                        });
            }
            else if (definition.beltType == BeltType.HTD_9mm)
            {
                addInstance(instantiator, vexpro9mmHTD::build, {
                            "configuration" : {
                                "Pulley_type" : getLookupTable(HTDPulleyTable, definition.pulleyTwoTeethVexproHTD).Pulley
                            },
                            "transform" : transform(toWorld(pulleyTwoCoords)),
                            "name" : "pulleytwo"
                        });
            }
            else if (definition.beltType == BeltType.HTD_15mm)
            {
                addInstance(instantiator, vexpro15mmHTD::build, {
                            "configuration" : {
                                "Pulley_type" : getLookupTable(HTDPulleyTable, definition.pulleyTwoTeethVexproHTD).Pulley
                            },
                            "transform" : transform(toWorld(pulleyTwoCoords)),
                            "name" : "pulleytwo"
                        });
            }
        }
        instantiate(context, instantiator);

        // ---body queries---
        // query for the belt
        var beltQuery is Query = qCreatedBy(id + "thicken1", EntityType.BODY);

        var pulleyBodiesQuery is Query = qBodyType(qCreatedBy(id + "instantiator", EntityType.BODY), BodyType.SOLID);

        // query for custom pulley one (and mate connector)
        var pulleyOneQuery is Query = qCreatedBy(id + "instantiator" + "pulleyone", EntityType.BODY);

        // query for custom pulley two (and mate connector)
        var pulleyTwoQuery is Query = qCreatedBy(id + "instantiator" + "pulleytwo", EntityType.BODY);

        // ---mate connectors---
        // create mate connectors on the belt if no pulley is being created
        if (definition.pulleyOneType == PulleyType.NONE)
        {
            opMateConnector(context, id + "mateConnector1", {
                        "coordSystem" : pulleyOneCoords,
                        "owner" : beltQuery
                    });
        }
        if (definition.pulleyTwoType == PulleyType.NONE)
        {
            opMateConnector(context, id + "mateConnector2", {
                        "coordSystem" : pulleyTwoCoords,
                        "owner" : beltQuery
                    });
        }

        // create composite part
        if ((definition.pulleyOneType != PulleyType.NONE || definition.enablePulleyTwo != PulleyType.NONE) && definition.createComposite)
        // if pulleyOne and/or pulleyTwo are enabled, and createComposite is checked:
        {
            opCreateCompositePart(context, id + "compositePart1", {
                        "bodies" : qUnion([beltQuery, pulleyBodiesQuery])
                    });
        }

        var compositeQuery is Query = qCreatedBy(id + "compositePart1", EntityType.BODY);

        // set belt properties
        setProperty(context, {
                    "entities" : beltQuery,
                    "propertyType" : PropertyType.MATERIAL,
                    "value" : material("Viton Rubber", 1827 * kilogram / meter ^ 3) // default belt material and density
                });
        setProperty(context, {
                    "entities" : beltQuery,
                    "propertyType" : PropertyType.APPEARANCE,
                    "value" : color(0.53, 0.53, 0.53) // default belt color (RGB value, x/255)
                });
        setProperty(context, {
                    "entities" : beltQuery,
                    "propertyType" : PropertyType.NAME,
                    "value" : beltTeeth ~ "T " ~ beltType ~ " Belt"
                });

        // set pulley one properties
        // when changing default values, be sure to update pulley two as well
        if (definition.pulleyOneType == PulleyType.CUSTOM)
        {
            setProperty(context, {
                        "entities" : pulleyOneQuery,
                        "propertyType" : PropertyType.NAME,
                        "value" : pulleyOneTeeth ~ "T " ~ beltType ~ " Custom Pulley"
                    });

            setProperty(context, {
                        "entities" : pulleyOneQuery,
                        "propertyType" : PropertyType.MATERIAL,
                        "value" : material("Onyx", 1.18 * gram / centimeter ^ 3) // custom pulley material and density
                    });

            setProperty(context, {
                        "entities" : pulleyOneQuery,
                        "propertyType" : PropertyType.APPEARANCE,
                        "value" : color(0.3, 0.3, 0.3) // custom pulley color (RGB value, x/255)
                    });
        }
        else if (definition.pulleyOneType == PulleyType.VEXPRO)
        {
            setProperty(context, {
                        "entities" : pulleyOneQuery,
                        "propertyType" : PropertyType.EXCLUDE_FROM_BOM,
                        "value" : true
                    });

        }
        // set pulley two properties
        if (definition.pulleyTwoType == PulleyType.CUSTOM)
        {
            setProperty(context, {
                        "entities" : pulleyTwoQuery,
                        "propertyType" : PropertyType.NAME,
                        "value" : pulleyTwoTeeth ~ "T " ~ beltType ~ " Custom Pulley"
                    });

            setProperty(context, {
                        "entities" : pulleyTwoQuery,
                        "propertyType" : PropertyType.MATERIAL,
                        "value" : material("Onyx", 1.18 * gram / centimeter ^ 3) // custom pulley material and density
                    });

            setProperty(context, {
                        "entities" : pulleyTwoQuery,
                        "propertyType" : PropertyType.APPEARANCE,
                        "value" : color(0.3, 0.3, 0.3) // custom pulley color (RGB value, x/255)
                    });
        }
        else if (definition.pulleyTwoType == PulleyType.VEXPRO)
        {
            setProperty(context, {
                        "entities" : pulleyTwoQuery,
                        "propertyType" : PropertyType.EXCLUDE_FROM_BOM,
                        "value" : true
                    });
        }

        // set composite part properties
        if ((definition.pulleyOneType != PulleyType.NONE || definition.pulleyTwoType != PulleyType.NONE) && definition.createComposite)
        {
            setProperty(context, {
                        "entities" : compositeQuery,
                        "propertyType" : PropertyType.NAME,
                        "value" : pulleyOneTeeth ~ ":" ~ pulleyTwoTeeth ~ " " ~ beltType ~ " Belt Run" //composite part name
                    });
        }

        transformResultIfNecessary(context, id, remainingTransform);
    }); // end of feature definition



export function editBeltLogic(context is Context, id is Id, oldDefinition is map, definition is map, isCreating is boolean) returns map
{
    // update the widths of the pulleys when the beltType is changed
    if (oldDefinition.beltType != definition.beltType)
    // if the beltType is changed:
    {
        if (definition.beltType == BeltType.GT2)
        {
            definition.pulleyOneWidth = defaultGT2PulleyWidth; // When you change the beltType GT2, custom pulleys become this wide
            definition.pulleyTwoWidth = defaultGT2PulleyWidth;
            return definition;
            if (definition.beltType == BeltType.HTD_9mm)
            {
                definition.pulleyOneWidth = default9mmHTDPulleyWidth; // When you change the beltType to 9mm HTD, custom pulleys become this wide
                definition.pulleyTwoWidth = default9mmHTDPulleyWidth;
            }
        }
        if (definition.beltType == BeltType.HTD_15mm)
        {
            definition.pulleyOneWidth = default15mmHTDPulleyWidth; // When you change the beltType to 15mm HTD, custom pulleys become this wide
            definition.pulleyTwoWidth = default15mmHTDPulleyWidth;
            return definition;
        }
    }
    return definition;
}
