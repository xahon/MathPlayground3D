using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using UnityEngine;
using UnityEngine.Scripting;

public class InterpreterFrontendException : Exception
{
    public InterpreterFrontendException(string message) : base(message)
    {
    }
}

public class InterpreterFrontend : MonoBehaviour
{
    [SerializeField]
    private MainUi mainUi;

    [SerializeField]
    private Sandbox sandbox;

    private readonly Dictionary<string, string> cachedUserInput = new Dictionary<string, string>();

    private Script script;
    private DynValue updateFunc = DynValue.Nil;
    private bool isInUpdate = false;
    private API api;

    private class LuaFunctionAttribute : Attribute
    {
        public string LuaFuncName { get; }

        public LuaFunctionAttribute(string luaFuncName)
        {
            LuaFuncName = luaFuncName;
        }
    }

    private void Awake()
    {
        api = new API(this);

        UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
        UserData.RegisterAssembly();
        LuaCustomConverters.RegisterAll();

        script = new Script();

        // Bind all Lua functions
        HashSet<string> registeredLuaFuncNames = new HashSet<string>();
        Assembly.GetCallingAssembly().GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            .Where(m => m.GetCustomAttributes(typeof(LuaFunctionAttribute), false).Length > 0)
            .ToList()
            .ForEach(m =>
            {
                ParameterInfo[] parameters = m.GetParameters();
                bool parametersOk =
                    parameters.Length == 2 &&
                    parameters[0].ParameterType == typeof(ScriptExecutionContext) &&
                    parameters[1].ParameterType == typeof(CallbackArguments);

                if (m.ReturnParameter == null || m.ReturnParameter.ParameterType != typeof(DynValue) || !parametersOk)
                {
                    Debug.LogError($"Lua function {m.Name} has invalid signature");
                    return;
                }

                string luaFuncName = ((LuaFunctionAttribute)m.GetCustomAttributes(typeof(LuaFunctionAttribute), false)[0]).LuaFuncName;
                if (!registeredLuaFuncNames.Add(luaFuncName))
                {
                    Debug.LogError($"Lua function {luaFuncName} is already registered");
                    return;
                }

                Func<ScriptExecutionContext, CallbackArguments, DynValue> func;
                if (m.IsStatic)
                {
                    func = (ctx, args) => (DynValue)m.Invoke(null, new object[] { ctx, args });
                }
                else
                {
                    func = (ctx, args) => (DynValue)m.Invoke(api, new object[] { ctx, args });
                }

                script.Globals[luaFuncName] = DynValue.NewCallback(func);
            });
    }

    private void LateUpdate()
    {
        sandbox.BeginFrame();
        if (updateFunc.IsNotNil())
        {
            isInUpdate = true;
            try
            {
                script.Call(updateFunc, DynValue.NewNumber(Time.deltaTime), DynValue.NewNumber(Time.time));
            }
            catch (Exception e)
            {
                sandbox.ResetState();
                mainUi.ResetUserInput();
                updateFunc = DynValue.Nil;
                Debug.LogException(e);
            }
            isInUpdate = false;
        }
        sandbox.EndFrame();
    }

    public void RunCode(string code)
    {
        updateFunc = DynValue.Nil;
        sandbox.ResetState();
        mainUi.ResetUserInput();

        try
        {
            script.DoString(code);
            updateFunc = script.Globals.Get("update");
        }
        catch (Exception e)
        {
            sandbox.ResetState();
            mainUi.ResetUserInput();
            Debug.LogException(e);
        }
    }

    private class API
    {
        private InterpreterFrontend interpreter;

        public API(InterpreterFrontend interpreter)
        {
            this.interpreter = interpreter;
        }

        [Preserve]
        [UsedImplicitly]
        [LuaFunction("print")]
        public DynValue Log(ScriptExecutionContext ctx, CallbackArguments args)
        {
            switch (args[0].Type)
            {
                case DataType.Boolean:
                    print(args[0].Boolean);
                    break;
                case DataType.Number:
                    print(args[0].Number);
                    break;
                case DataType.String:
                    print(args[0].String);
                    break;
                case DataType.Function:
                    break;
                case DataType.Table:
                    print(args[0].Table.ToString());
                    break;
                case DataType.Tuple:
                    print(args[0].Tuple.ToString());
                    break;
                case DataType.UserData:
                    print(args[0].UserData.ToString());
                    break;
            }
            return DynValue.Nil;
        }

        [Preserve]
        [UsedImplicitly]
        [LuaFunction("input")]
        public DynValue DefineUserInput(ScriptExecutionContext ctx, CallbackArguments args)
        {
            if (interpreter.isInUpdate)
            {
                throw new InterpreterFrontendException("Cannot define user input in update function");
            }

            string varname = args[0].String;
            string type = args[1].String;

            RectTransform uiElement = interpreter.mainUi.DefineUserInput(varname, type);
            if (uiElement != null && type == "float_slider")
            {
                float minValue = 0.0f;
                if (args.Count > 2)
                {
                    minValue = (float)args[2].Number;
                }

                float maxValue = 1.0f;
                if (args.Count > 3)
                {
                    maxValue = (float)args[3].Number;
                }

                float defaultValue = Mathf.Clamp(interpreter.cachedUserInput.TryGetValue(varname, out string value) ? float.Parse(value) : 0, minValue, maxValue);
                if (args.Count > 4)
                {
                    defaultValue = Mathf.Clamp((float)args[4].Number, minValue, maxValue);
                }

                FloatSliderUi sliderUi = uiElement.GetComponentInChildren<FloatSliderUi>();

                sliderUi.OnValueChanged += (value) =>
                {
                    interpreter.script.Globals[varname] = DynValue.NewNumber(value);
                    interpreter.cachedUserInput[varname] = value.ToString();
                };

                sliderUi.Init(varname, minValue, maxValue, defaultValue);
            }

            return DynValue.Nil;
        }

        [Preserve]
        [UsedImplicitly]
        [LuaFunction("color_norm")]
        public DynValue SetColorNorm(ScriptExecutionContext ctx, CallbackArguments args)
        {
            float r = (float)args[0].Number;
            float g = (float)args[1].Number;
            float b = (float)args[2].Number;
            float a = 1.0f;
            if (args.Count > 3)
            {
                a = (float)args[3].Number;
            }

            interpreter.sandbox.SetColor(r, g, b, a);
            return DynValue.Nil;
        }

        [Preserve]
        [UsedImplicitly]
        [LuaFunction("color")]
        public DynValue SetColor(ScriptExecutionContext ctx, CallbackArguments args)
        {
            float r = (float)args[0].Number / 255.0f;
            float g = (float)args[1].Number / 255.0f;
            float b = (float)args[2].Number / 255.0f;
            float a = 1.0f;
            if (args.Count > 3)
            {
                a = (float)args[3].Number / 255.0f;
            }

            interpreter.sandbox.SetColor(r, g, b, a);
            return DynValue.Nil;
        }

        [Preserve]
        [UsedImplicitly]
        [LuaFunction("line")]
        public DynValue DrawLine(ScriptExecutionContext ctx, CallbackArguments args)
        {
            Vector3 p1 = args[0].ToObject<Vector3>();
            Vector3 p2 = args[1].ToObject<Vector3>();

            interpreter.sandbox.DrawLine(p1, p2);
            return DynValue.Nil;
        }

        [Preserve]
        [UsedImplicitly]
        [LuaFunction("vector")]
        public DynValue DrawVector(ScriptExecutionContext ctx, CallbackArguments args)
        {
            Vector3 origin = args[0].ToObject<Vector3>();
            Vector3 scaledDir = args[1].ToObject<Vector3>();

            interpreter.sandbox.DrawVector(origin, scaledDir);
            return DynValue.Nil;
        }

        [Preserve]
        [UsedImplicitly]
        [LuaFunction("tri")]
        public DynValue DrawTriangle(ScriptExecutionContext ctx, CallbackArguments args)
        {
            Vector3 p1 = args[0].ToObject<Vector3>();
            Vector3 p2 = args[1].ToObject<Vector3>();
            Vector3 p3 = args[2].ToObject<Vector3>();
            bool filled = false;
            if (args.Count > 3)
            {
                filled = args[3].Boolean;
            }

            interpreter.sandbox.DrawTriangle(p1, p2, p3, filled);
            return DynValue.Nil;
        }
    }
}
