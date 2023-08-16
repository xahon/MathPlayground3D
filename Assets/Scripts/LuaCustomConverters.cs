using UnityEngine;
using MoonSharp.Interpreter;
using System;

/// <summary>
/// These are custom converters for Vector2 and Vector3 structs.
///
/// Add the following call to your code:
/// LuaCustomConverters.RegisterAll();
///
/// To create Vector2 in lua:
/// position = {1.0, 1.0}
///
/// To Vector3 in lua:
/// position = {1.0, 1.0, 1.0}
///
/// </summary>
public static class LuaCustomConverters {

    public static void RegisterAll() {

        // Vector 2

        Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Vector2),
            dynVal => {
                Table table = dynVal.Table;
                float x = (float)(Double)table[1];
                float y = (float)(Double)table[2];
                return new Vector2(x, y);
            }
        );
        Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Vector2>(
            (script, vector) => {
                DynValue x = DynValue.NewNumber(vector.x);
                DynValue y = DynValue.NewNumber(vector.y);
                DynValue dynVal = DynValue.NewTable(script, new DynValue[] { x, y });
                return dynVal;
            }
        );

        // Vector3

        Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Vector3),
            dynVal => {
                Table table = dynVal.Table;
                float x = (float)(Double)table[1];
                float y = (float)(Double)table[2];
                float z = 0;
                if (table.Length > 2)
                {
                    z = (float)(Double)table[3];
                }
                return new Vector3(x, y, z);
            }
        );
        Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Vector3>(
            (script, vector) => {
                DynValue x = DynValue.NewNumber(vector.x);
                DynValue y = DynValue.NewNumber(vector.y);
                DynValue z = DynValue.NewNumber(vector.z);
                DynValue dynVal = DynValue.NewTable(script, new DynValue[] { x, y, z });
                return dynVal;
            }
        );

        // Color

        Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Color),
            dynVal =>
            {
                Table table = dynVal.Table;
                float r = (float)(Double)table[1];
                float g = (float)(Double)table[2];
                float b = (float)(Double)table[3];
                float a = 1;
                if (table.Length >= 4)
                {
                    a = (float)(Double)table[4];
                }
                return new Color(r, g, b, a);
            }
        );
        Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Color>(
            (script, color) =>
            {
                DynValue r = DynValue.NewNumber(color.r);
                DynValue g = DynValue.NewNumber(color.g);
                DynValue b = DynValue.NewNumber(color.b);
                DynValue a = DynValue.NewNumber(color.a);
                DynValue dynVal = DynValue.NewTable(script, new DynValue[] { r, g, b, a });
                return dynVal;
            }
        );
    }

}