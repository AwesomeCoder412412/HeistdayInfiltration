using System.Collections.Generic;
using System.IO;

namespace Unity.Build.Classic.Private
{
    sealed class CopyAdditionallyProvidedFilesStepBeforeBuild : BuildStepBase
    {
        string streamingAssets = "Assets/StreamingAssets";

        class ProviderInfo
        {
            public List<string> Paths = new List<string>();
            public List<string> Directories = new List<string>();
        }

        public override BuildResult Run(BuildContext context)
        {
            var info = new ProviderInfo();

            var classicSharedData = context.GetValue<ClassicSharedData>();

            var oldStreamingAssetsDirectory = classicSharedData.StreamingAssetsDirectory;
            classicSharedData.StreamingAssetsDirectory = streamingAssets;

            foreach (var customizer in classicSharedData.Customizers)
                customizer.OnBeforeRegisterAdditionalFilesToDeploy();

            foreach (var customizer in classicSharedData.Customizers)
            {
                customizer.RegisterAdditionalFilesToDeploy((from, to) =>
                {
                    var parent = Path.GetDirectoryName(to);

                    if (!Directory.Exists(parent))
                    {
                        Directory.CreateDirectory(parent);
                        info.Directories.Add(parent);
                    }
                    File.Copy(from, to, true);
                    info.Paths.Add(to);
                });
            }
            classicSharedData.StreamingAssetsDirectory = oldStreamingAssetsDirectory;
            context.SetValue(info);
            return context.Success();
        }

        public override BuildResult Cleanup(BuildContext context)
        {
            var info = context.GetValue<ProviderInfo>();
            foreach (var f in info.Paths)
            {
                if (File.Exists(f))
                    File.Delete(f);

                // We probably created some meta files we need to clean up
                var metaPath = $"{f}.meta";
                if(File.Exists(metaPath))
                    File.Delete(metaPath);
            }

            // We may have created directories as well, so we should remove those too
            foreach (var d in info.Directories)
            {
                if (Directory.Exists(d))
                    Directory.Delete(d, true);

                // We probably created some meta files we need to clean up
                var metaPath = $"{d}.meta";
                if(File.Exists(metaPath))
                    File.Delete(metaPath);
            }

            return context.Success();
        }
    }
}
