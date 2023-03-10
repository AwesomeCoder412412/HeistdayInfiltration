using System;

namespace Unity.Build.Classic.Private
{
    class BuildPipelineSelector : BuildPipelineSelectorBase
    {
        BuildPipelineBase ConstructPipeline(Platform platform)
        {
            if (platform == null)
                throw new ArgumentNullException(nameof(platform));

            return TypeConstructionUtility.TryConstructTypeDerivedFrom<ClassicNonIncrementalPipelineBase>(p =>
            {
                var pipelineType = p.GetType();

                // MissingNonIncrementalPipeline is not a valid pipeline to construct
                if (pipelineType == typeof(MissingClassicNonIncrementalPipeline))
                    return false;

                // Verify pipeline type namespace is from Unity
                var @namespace = pipelineType.Namespace;
                if (string.IsNullOrEmpty(@namespace))
                    return false;

                if (!@namespace.StartsWith("Unity.Build."))
                    return false;

                if (!@namespace.EndsWith(".Classic") && !@namespace.EndsWith(".Classic.Private"))
                    return false;

                // Skip test pipelines
                if (@namespace.Contains("Test"))
                    return false;

                // Verify if platform match
                return p.Platform == platform;
            }, out var pipeline) ? pipeline : null;
        }

        public override BuildPipelineBase SelectFor(Platform platform)
        {
            if (platform == null)
                return null;

            var pipeline = default(BuildPipelineBase);
            if (platform.HasPackage && platform.IsPackageInstalled)
                pipeline = ConstructPipeline(platform);

            return pipeline ?? new MissingClassicNonIncrementalPipeline(platform);
        }
    }
}
