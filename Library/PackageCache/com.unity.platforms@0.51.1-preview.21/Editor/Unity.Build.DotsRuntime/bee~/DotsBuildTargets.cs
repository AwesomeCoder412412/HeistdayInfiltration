using System;
using Bee.DotNet;
using Bee.NativeProgramSupport;
using NiceIO;

namespace DotsBuildTargets
{
    public abstract class DotsBuildSystemTarget
    {
        // Disabling by default; Eventually: ScriptingBackend == ScriptingBackend.Dotnet;
        protected virtual bool CanRunMultiThreadedJobs => false;

        // disabled by default because it takes work to enable each platform for burst
        public virtual bool CanUseBurst => false;

        public abstract string Identifier { get; }
        public abstract ToolChain ToolChain { get; }

        public virtual ScriptingBackend ScriptingBackend { get; set; } = ScriptingBackend.TinyIl2cpp;
        public virtual TargetFramework TargetFramework => TargetFramework.Tiny;

        public virtual DotsRuntimeCSharpProgramConfiguration CustomizeConfigForSettings(DotsRuntimeCSharpProgramConfiguration config, FriendlyJObject settings) => config;

        // called for every main/"game" program that is built for this target, to give the target a chance to customize it
        // or to create additional targets that depend on it
        public virtual void CustomizeMainProgram(DotsRuntimeCSharpProgram main)
        {
        }

        // required if more than one set of binaries need to be generated (for example Android armv7 + arm64)
        // see comment in https://github.com/Unity-Technologies/dots/blob/master/TinySamples/Packages/com.unity.dots.runtime/bee%7E/BuildProgramSources/DotsConfigs.cs
        // DotsConfigs.MakeConfigs() method for details.
        public virtual DotsBuildSystemTarget ComplementaryTarget => null;

        public virtual bool ValidateManagedDebugging(ref bool mdb)
        {
            return true;
        }

        public virtual BurstCompiler CreateBurstCompiler(DotsRuntimeCSharpProgramConfiguration config) => null;

        // Give the target an opportunity to do something with a toplevel main program, that was fully built/setup to setupGame,
        // that was built using the given config to the deployed directory, and with the given final binary.
        // For example, setting up adjacent gradle/xcode projects, etc.
        public virtual void AfterDeployedFinalProgram(AsmDefCSharpProgram gameProgram,
            DotNetAssembly setupGame,
            BuiltNativeProgram builtNativeProgram,
            DotsRuntimeCSharpProgramConfiguration config,
            NPath deployDirPath, NPath deployBinaryPath)
        {
        }
    }
}
