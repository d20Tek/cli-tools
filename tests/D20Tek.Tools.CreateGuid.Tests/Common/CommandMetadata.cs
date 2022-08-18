//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace D20Tek.Tools.CreateGuid.Tests.Common
{
    public class CommandMetadata
    {
        public string Name { get; }

        public HashSet<string> Aliases { get; }

        public string? Description { get; set; }

        public object? Data { get; set; }

        public Type? CommandType { get; }

        public Type SettingsType { get; }

        public Func<CommandContext, CommandSettings, int>? Delegate { get; }

        public bool IsDefaultCommand { get; }

        public bool IsHidden { get; set; }

        public IList<CommandMetadata> Children { get; }

        public IList<string[]> Examples { get; }

        private CommandMetadata(
            string name,
            Type? commandType,
            Type settingsType,
            Func<CommandContext, CommandSettings, int>? @delegate,
            bool isDefaultCommand)
        {
            Name = name;
            Aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            CommandType = commandType;
            SettingsType = settingsType;
            Delegate = @delegate;
            IsDefaultCommand = isDefaultCommand;

            Children = new List<CommandMetadata>();
            Examples = new List<string[]>();
        }

        public static CommandMetadata FromBranch(Type settings, string name)
        {
            return new CommandMetadata(name, null, settings, null, false);
        }

        public static CommandMetadata FromBranch<TSettings>(string name)
            where TSettings : CommandSettings
        {
            return new CommandMetadata(name, null, typeof(TSettings), null, false);
        }

        public static CommandMetadata FromType<TCommand>(string name, bool isDefaultCommand = false)
            where TCommand : class, ICommand
        {
            var settingsType = GetSettingsType(typeof(TCommand));
            if (settingsType == null)
            {
                throw new InvalidOperationException();
            }

            return new CommandMetadata(name, typeof(TCommand), settingsType, null, isDefaultCommand);
        }

        public static CommandMetadata FromDelegate<TSettings>(
            string name, Func<CommandContext, CommandSettings, int>? @delegate = null)
                where TSettings : CommandSettings
        {
            return new CommandMetadata(name, null, typeof(TSettings), @delegate, false);
        }

        private static Type? GetSettingsType(Type commandType)
        {
            if (typeof(ICommand).GetTypeInfo().IsAssignableFrom(commandType) &&
                GetGenericTypeArguments(commandType, typeof(ICommand<>), out var result))
            {
                return result[0];
            }

            return null;
        }

        private static bool GetGenericTypeArguments(
            Type? type,
            Type genericType,
            [NotNullWhen(true)] out Type[]? genericTypeArguments)
        {
            while (type != null)
            {
                foreach (var @interface in type.GetTypeInfo().GetInterfaces())
                {
                    if (!@interface.GetTypeInfo().IsGenericType || @interface.GetGenericTypeDefinition() != genericType)
                    {
                        continue;
                    }

                    genericTypeArguments = @interface.GenericTypeArguments;
                    return true;
                }

                type = type.GetTypeInfo().BaseType;
            }

            genericTypeArguments = null;
            return false;
        }
    }
}
