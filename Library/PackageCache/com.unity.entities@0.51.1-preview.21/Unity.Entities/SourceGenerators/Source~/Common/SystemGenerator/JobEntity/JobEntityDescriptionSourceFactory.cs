using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Unity.Entities.SourceGen.Common;

namespace Unity.Entities.SourceGen.SystemGeneratorCommon
{
    public partial class JobEntityDescription
    {
        public TypeDeclarationSyntax Generate()
        {
            string partialStructImplementation =
            $@"[global::System.Runtime.CompilerServices.CompilerGenerated]
               {GetPartialStructNameAndInterfaces()}
               {{
                    {UserExecuteMethodParams.Select(p => p.FieldText).SeparateByNewLine()}{Environment.NewLine}
                    {"public Unity.Entities.EntityManager __EntityManager;\n".EmitIfTrue(UserExecuteMethodParams.Any(param => param.RequiresEntityManagerAccess))}
                    [global::System.Runtime.CompilerServices.CompilerGenerated]
                    {GetExecuteMethodSignature()}
                    {{
                        {UserExecuteMethodParams.Select(p => p.VariableDeclarationText).SeparateByNewLine()}{Environment.NewLine}
                        int count = batch.Count;
                        for(int i = 0; i < count; ++i){Environment.NewLine}
                        {{
                            {UserExecuteMethodParams.Where(p => p.RequiresLocalCode).Select(p => p.LocalCodeText).SeparateByNewLine()}{Environment.NewLine}
                            Execute({UserExecuteMethodParams.Select(param => param.ExecuteArgumentText).SeparateByComma()});
                        }}
                    }}

                    {GetScheduleAndRunMethods()}
               }}";

            string GetPartialStructNameAndInterfaces()
            {
                return HasEntityInQueryIndex()
                    ? $"partial struct {m_TypeName} : IJobEntityBatchWithIndex"
                    : $"partial struct {m_TypeName} : IJobEntityBatch";
            }

            string GetExecuteMethodSignature()
            {
                return HasEntityInQueryIndex()
                    ? "public void Execute(ArchetypeChunk batch, int batchIndex, int indexOfFirstEntityInQuery)"
                    : "public void Execute(ArchetypeChunk batch, int batchIndex)";
            }

            return (TypeDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(partialStructImplementation);
        }

        static string GetScheduleAndRunMethods()
        {
            var source =
            $@"
                public Unity.Jobs.JobHandle Schedule(Unity.Entities.EntityQuery query, Unity.Jobs.JobHandle dependsOn = default(Unity.Jobs.JobHandle)) => __ThrowCodeGenException();
                public Unity.Jobs.JobHandle Schedule(Unity.Jobs.JobHandle dependsOn = default(Unity.Jobs.JobHandle)) => __ThrowCodeGenException();

                public Unity.Jobs.JobHandle ScheduleByRef(Unity.Entities.EntityQuery query, Unity.Jobs.JobHandle dependsOn = default(Unity.Jobs.JobHandle)) => __ThrowCodeGenException();
                public Unity.Jobs.JobHandle ScheduleByRef(Unity.Jobs.JobHandle dependsOn = default(Unity.Jobs.JobHandle)) => __ThrowCodeGenException();

                public Unity.Jobs.JobHandle ScheduleParallel(Unity.Entities.EntityQuery query, Unity.Jobs.JobHandle dependsOn = default(Unity.Jobs.JobHandle)) => __ThrowCodeGenException();
                public Unity.Jobs.JobHandle ScheduleParallel(Unity.Jobs.JobHandle dependsOn = default(Unity.Jobs.JobHandle)) => __ThrowCodeGenException();

                public Unity.Jobs.JobHandle ScheduleParallelByRef(Unity.Entities.EntityQuery query, Unity.Jobs.JobHandle dependsOn = default(Unity.Jobs.JobHandle)) => __ThrowCodeGenException();
                public Unity.Jobs.JobHandle ScheduleParallelByRef(Unity.Jobs.JobHandle dependsOn = default(Unity.Jobs.JobHandle)) => __ThrowCodeGenException();

                public void Run(Unity.Entities.EntityQuery query) => __ThrowCodeGenException();
                public void Run() => __ThrowCodeGenException();

                public void RunByRef(Unity.Entities.EntityQuery query) => __ThrowCodeGenException();
                public void RunByRef() => __ThrowCodeGenException();

                Unity.Jobs.JobHandle __ThrowCodeGenException() => throw new global::System.Exception(""This method should have been replaced by source gen."");
            ";

            return source;
        }
    }
}
