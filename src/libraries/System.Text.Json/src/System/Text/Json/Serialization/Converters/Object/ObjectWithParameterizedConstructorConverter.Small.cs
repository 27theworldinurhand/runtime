﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    /// <summary>
    /// Implementation of <cref>JsonObjectConverter{T}</cref> that supports the deserialization
    /// of JSON objects using parameterized constructors.
    /// </summary>
    internal sealed class SmallObjectWithParameterizedConstructorConverter<T, TArg0, TArg1, TArg2, TArg3> : ObjectWithParameterizedConstructorConverter<T> where T : notnull
    {
        protected override object CreateObject(ref ReadStackFrame frame)
        {
            var createObject = (JsonClassInfo.ParameterizedConstructorDelegate<T, TArg0, TArg1, TArg2, TArg3>)
                frame.JsonClassInfo.CreateObjectWithArgs!;
            var arguments = (Arguments<TArg0, TArg1, TArg2, TArg3>)frame.CtorArgumentState!.Arguments;
            return createObject!(arguments.Arg0, arguments.Arg1, arguments.Arg2, arguments.Arg3);
        }

        protected override bool ReadAndCacheConstructorArgument(ref ReadStack state, ref Utf8JsonReader reader, JsonParameterInfo jsonParameterInfo)
        {
            Debug.Assert(state.Current.CtorArgumentState!.Arguments != null);
            var arguments = (Arguments<TArg0, TArg1, TArg2, TArg3>)state.Current.CtorArgumentState.Arguments;

            bool success;

            switch (jsonParameterInfo.Position)
            {
                case 0:
                    success = ((JsonParameterInfo<TArg0>)jsonParameterInfo).ReadJsonTyped(ref state, ref reader, out TArg0 arg0);
                    if (success)
                    {
                        arguments.Arg0 = arg0;
                    }
                    break;
                case 1:
                    success = ((JsonParameterInfo<TArg1>)jsonParameterInfo).ReadJsonTyped(ref state, ref reader, out TArg1 arg1);
                    if (success)
                    {
                        arguments.Arg1 = arg1;
                    }
                    break;
                case 2:
                    success = ((JsonParameterInfo<TArg2>)jsonParameterInfo).ReadJsonTyped(ref state, ref reader, out TArg2 arg2);
                    if (success)
                    {
                        arguments.Arg2 = arg2;
                    }
                    break;
                case 3:
                    success = ((JsonParameterInfo<TArg3>)jsonParameterInfo).ReadJsonTyped(ref state, ref reader, out TArg3 arg3);
                    if (success)
                    {
                        arguments.Arg3 = arg3;
                    }
                    break;
                default:
                    Debug.Fail("This should never happen.");
                    throw new InvalidOperationException();
            }

            return success;
        }

        protected override void InitializeConstructorArgumentCaches(ref ReadStack state, JsonSerializerOptions options)
        {
            JsonClassInfo classInfo = state.Current.JsonClassInfo;

            if (classInfo.CreateObjectWithArgs == null)
            {
                classInfo.CreateObjectWithArgs =
                    options.MemberAccessorStrategy.CreateParameterizedConstructor<T, TArg0, TArg1, TArg2, TArg3>(ConstructorInfo!);
            }

            var arguments = new Arguments<TArg0, TArg1, TArg2, TArg3>();

            foreach (JsonParameterInfo parameterInfo in classInfo.ParameterCache!.Values)
            {
                if (parameterInfo.ShouldDeserialize)
                {
                    int position = parameterInfo.Position;

                    switch (position)
                    {
                        case 0:
                            arguments.Arg0 = ((JsonParameterInfo<TArg0>)parameterInfo).TypedDefaultValue!;
                            break;
                        case 1:
                            arguments.Arg1 = ((JsonParameterInfo<TArg1>)parameterInfo).TypedDefaultValue!;
                            break;
                        case 2:
                            arguments.Arg2 = ((JsonParameterInfo<TArg2>)parameterInfo).TypedDefaultValue!;
                            break;
                        case 3:
                            arguments.Arg3 = ((JsonParameterInfo<TArg3>)parameterInfo).TypedDefaultValue!;
                            break;
                        default:
                            Debug.Fail("We should never get here.");
                            break;
                    }
                }
            }

            state.Current.CtorArgumentState!.Arguments = arguments;
        }
    }
}
