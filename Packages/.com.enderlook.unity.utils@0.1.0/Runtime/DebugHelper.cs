using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;

using Debug = UnityEngine.Debug;
using UnityObject = UnityEngine.Object;

namespace Enderlook.Unity.Utils
{
    public static class DebugHelper
    {
        public enum TraceMode { Hidden, OneLine, FullStackTrace }

        /// <summary>
        /// Default <see cref="TraceMode"/> used when a method provides a null trace.
        /// </summary>
        public static TraceMode traceMode = TraceMode.FullStackTrace;

        /// <summary>
        /// Print to console all <paramref name="objects"/> as strings separated by ', '.<br/>
        /// <see langword="null"/>s are turned into "null".
        /// Objects of type <see cref="Color"/> will have included the HTMl tag color.
        /// </summary>
        /// <param name="objects">Objects to print in console.</param>
        public static void Log(params object[] objects) => Debug.Log(GetStrings(objects));

        /// <summary>
        /// Print to console <paramref name="object"/> as strings.<br/>
        /// <see langword="null"/>s are turned into "null".
        /// Objects of type <see cref="Color"/> will have included the HTMl tag color.
        /// </summary>
        /// <param name="object">Object to print in console.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void Log(object @object, UnityObject context = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Debug.Log(@object, context);
            MethodInformation(traceMode, memberName, sourceFilePath, sourceLineNumber, context, Debug.Log);
        }

        /// <summary>
        /// Print to console all <paramref name="objects"/> as strings separated by ', '.
        /// <see langword="null"/>s are turned into "null".
        /// Objects of type <see cref="Color"/> will have included the HTMl tag color.
        /// </summary>
        /// <param name="objects">Objects to print in console.</param>
        public static void LogWarning(params object[] objects) => Debug.LogWarning(GetStrings(objects));

        /// <summary>
        /// Print to console <paramref name="object"/> as strings.<br/>
        /// <see langword="null"/>s are turned into "null".
        /// Objects of type <see cref="Color"/> will have included the HTMl tag color.
        /// </summary>
        /// <param name="object">Object to print in console.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogWarning(object @object, UnityObject context = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Debug.LogWarning(@object, context);
            MethodInformation(traceMode, memberName, sourceFilePath, sourceLineNumber, context, Debug.LogWarning);
        }

        /// <summary>
        /// Print to console all <paramref name="objects"/> as strings separated by ', '.
        /// <see langword="null"/>s are turned into "null".
        /// Objects of type <see cref="Color"/> will have included the HTMl tag color.
        /// </summary>
        /// <param name="objects">Objects to print in console.</param>
        public static void LogError(params object[] objects) => Debug.LogError(GetStrings(objects));

        /// <summary>
        /// Print to console <paramref name="object"/> as strings.<br/>
        /// <see langword="null"/>s are turned into "null".
        /// Objects of type <see cref="Color"/> will have included the HTMl tag color.
        /// </summary>
        /// <param name="object">Object to print in console.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogError(object @object, UnityObject context = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Debug.LogError(@object, context);
            MethodInformation(traceMode, memberName, sourceFilePath, sourceLineNumber, context, Debug.LogError);
        }

        private static void MethodInformation(TraceMode? mode, string memberName, string sourceFilePath, int sourceLineNumber, UnityObject context, Action<string, UnityObject> debug)
        {
            if (mode == null)
                mode = traceMode;
            if (mode == TraceMode.Hidden)
                return;
            StringBuilder stringBuilder = new StringBuilder("<i><color=green><size=10> StackTrace info of last log. ");

            stringBuilder
                .Append(memberName)
                .Append(" (at ")
                .Append("Assets");

            foreach (string str in sourceFilePath.Split(new string[] { "Assets" }, StringSplitOptions.None).Skip(1))
                stringBuilder.Append(str);

            stringBuilder
                .Append(':')
                .Append(sourceLineNumber)
                .Append(")");

            if (mode == TraceMode.FullStackTrace)
                stringBuilder
                    .Append("\n")
                    .Append(new StackTrace().ToString().Split(new string[] { "\n" }, StringSplitOptions.None).Last());

            debug(stringBuilder.ToString() + "</size></color></i>", context);
        }

        /// <summary>
        /// Print to console all <paramref name="objects"/> as strings separated by ', ', preceded by message.<br/>
        /// <see langword="null"/>s are turned into "null".
        /// Objects of type <see cref="Color"/> will have included the HTMl tag color.
        /// </summary>
        /// <param name="message">A message to print in console. It will be treated as an additional object.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="traceMode">Tracing mode showed in console. If <see langword="null"/> it will use class <see cref="traceMode"/>.</param>
        /// <param name="memberName">Do not complete.</param>
        /// <param name="sourceFilePath">Do not complete.</param>
        /// <param name="sourceLineNumber">Do not complete.</param>
        /// <param name="objects">Objects to print in console.</param>
        public static void Log(string message = null, UnityObject context = null, TraceMode? traceMode = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            params object[] objects)
        {
            Debug.Log(GetStrings(message, objects), context);
            MethodInformation(traceMode, memberName, sourceFilePath, sourceLineNumber, context, Debug.Log);
        }

        /// <summary>
        /// Print to console all <paramref name="objects"/> as strings separated by ', ', preceded by message.<br/>
        /// <see langword="null"/>s are turned into "null".
        /// Objects of type <see cref="Color"/> will have included the HTMl tag color.
        /// </summary>
        /// <param name="message">A message to print in console. It will be treated as an additional object.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="traceMode">Tracing mode showed in console. If <see langword="null"/> it will use class <see cref="traceMode"/>.</param>
        /// <param name="memberName">Do not complete.</param>
        /// <param name="sourceFilePath">Do not complete.</param>
        /// <param name="sourceLineNumber">Do not complete.</param>
        /// <param name="objects">Objects to print in console.</param>
        public static void LogError(string message = null, UnityObject context = null, TraceMode? traceMode = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            params object[] objects)
        {
            Debug.Log(GetStrings(message, objects), context);
            MethodInformation(traceMode, memberName, sourceFilePath, sourceLineNumber, context, Debug.LogError);
        }

        /// <summary>
        /// Print to console all <paramref name="objects"/> as strings separated by ', ', preceded by message.<br/>
        /// <see langword="null"/>s are turned into "null".
        /// Objects of type <see cref="Color"/> will have included the HTMl tag color.
        /// </summary>
        /// <param name="message">A message to print in console. It will be treated as an additional object.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="traceMode">Tracing mode showed in console. If <see langword="null"/> it will use class <see cref="traceMode"/>.</param>
        /// <param name="memberName">Do not complete.</param>
        /// <param name="sourceFilePath">Do not complete.</param>
        /// <param name="sourceLineNumber">Do not complete.</param>
        /// <param name="objects">Objects to print in console.</param>
        public static void LogWarning(string message = null, UnityObject context = null, TraceMode? traceMode = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            params object[] objects)
        {
            Debug.Log(GetStrings(message, objects), context);
            MethodInformation(traceMode, memberName, sourceFilePath, sourceLineNumber, context, Debug.LogWarning);
        }

        /// <summary>
        /// Return an string with the <paramref name="object"/> as string.<br/>
        /// <see langword="null"/>s are turned into "null".<br/>
        /// Objects of type <see cref="Color"/> will have included the HTMl tag color.
        /// </summary>
        /// <param name="object">Object to get string.</param>
        /// <returns>String version of <paramref name="object"/>.</returns>
        public static string GetString(object @object)
        {
            if (@object == null)
                return "null";
            if (@object is Color color)
                return color.ToColoredString();
            return @object.ToString();
        }

        /// <summary>
        /// Return an string with all objects as strings separated by ', '.<br/>
        /// <see langword="null"/>s are turned into "null".
        /// Objects of type <see cref="Color"/> will have included the HTMl tag color.
        /// </summary>
        /// <param name="objects">Objects to join as string.</param>
        /// <returns>String version of <paramref name="objects"/>.</returns>
        public static string GetStrings(params object[] objects) => string.Join(", ", objects.Select(GetString));

        private static string GetStrings(string message, object[] objects) => (message == null ? "" : message + ", ") + GetStrings(objects);

        /// <summary>
        /// Print to console all <paramref name="enumerable"/> as strings separated by , preceded by message.<br/>
        /// <see langword="null"/>s are turned into "null".
        /// Objects of type <see cref="Color"/> will have included the HTMl tag color.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of <paramref name="enumerable"/> elements.</typeparam>
        /// <param name="enumerable">Enumerable to print in console.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="compact">Whenever it use a single log or several ones, one per line.</param>
        /// <param name="traceMode">Tracing mode showed in console. If <see langword="null"/> it will use class <see cref="traceMode"/>.</param>
        /// <param name="memberName">Do not complete.</param>
        /// <param name="sourceFilePath">Do not complete.</param>
        /// <param name="sourceLineNumber">Do not complete.</param>
        public static void LogLines<T>(IEnumerable<T> enumerable, UnityObject context = null, bool compact = false, TraceMode? traceMode = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (compact)
                Log(string.Join("\n", enumerable), context);
            else
                foreach (T item in enumerable)
                    Log(item, context);

            MethodInformation(traceMode, memberName, sourceFilePath, sourceLineNumber, context, Debug.Log);
        }

        /// <summary>
        /// Print to console all <paramref name="enumerable"/> as strings separated by , preceded by message.<br/>
        /// <see langword="null"/>s are turned into "null".
        /// Objects of type <see cref="Color"/> will have included the HTMl tag color.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of <paramref name="enumerable"/> elements.</typeparam>
        /// <param name="enumerable">Enumerable to print in console.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="compact">Whenever it use a single log or several ones, one per line.</param>
        /// <param name="traceMode">Tracing mode showed in console. If <see langword="null"/> it will use class <see cref="traceMode"/>.</param>
        /// <param name="memberName">Do not complete.</param>
        /// <param name="sourceFilePath">Do not complete.</param>
        /// <param name="sourceLineNumber">Do not complete.</param>
        public static void LogWarningLines<T>(IEnumerable<T> enumerable, UnityObject context = null, bool compact = false, TraceMode? traceMode = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (compact)
                LogWarning(string.Join("\n", enumerable), context);
            else
                foreach (T item in enumerable)
                    LogWarning(item, context);

            MethodInformation(traceMode, memberName, sourceFilePath, sourceLineNumber, context, Debug.LogWarning);
        }

        /// <summary>
        /// Print to console all <paramref name="enumerable"/> as strings separated by , preceded by message.<br/>
        /// <see langword="null"/>s are turned into "null".
        /// Objects of type <see cref="Color"/> will have included the HTMl tag color.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of <paramref name="enumerable"/> elements.</typeparam>
        /// <param name="enumerable">Enumerable to print in console.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="compact">Whenever it use a single log or several ones, one per line.</param>
        /// <param name="traceMode">Tracing mode showed in console. If <see langword="null"/> it will use class <see cref="traceMode"/>.</param>
        /// <param name="memberName">Do not complete.</param>
        /// <param name="sourceFilePath">Do not complete.</param>
        /// <param name="sourceLineNumber">Do not complete.</param>
        public static void LogErrorLines<T>(IEnumerable<T> enumerable, UnityObject context = null, bool compact = false, TraceMode? traceMode = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (compact)
                LogError(string.Join("\n", enumerable), context);
            else
                foreach (T item in enumerable)
                    LogError(item, context);

            MethodInformation(traceMode, memberName, sourceFilePath, sourceLineNumber, context, Debug.LogError);
        }
    }
}