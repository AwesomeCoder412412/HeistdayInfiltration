using NUnit.Framework;
using System;
using System.Linq;
using Unity.Serialization.Json;
using UnityEditor;

namespace Unity.Build.Tests
{
    [TestFixture]
    class PlatformTests
    {
        [Test]
        public void Platform_Serialization()
        {
            var platform = KnownPlatforms.Windows.GetPlatform();
            var serialized = JsonSerialization.ToJson(platform);
            var deserialized = JsonSerialization.FromJson<Platform>(serialized);
            Assert.That(platform, Is.EqualTo(deserialized));
        }

        [Test]
        public void BuildTarget_GetPlatform()
        {
            foreach (var buildTarget in Enum.GetValues(typeof(BuildTarget)).Cast<BuildTarget>())
            {
                if (buildTarget == BuildTarget.NoTarget ||
                    buildTarget == BuildTarget.StandaloneWindows ||
                    buildTarget.HasAttribute<ObsoleteAttribute>())
                {
                    Assert.That(buildTarget.GetPlatform(), Is.Null);
                }
                else
                {
                    Assert.That(buildTarget.GetPlatform(), Is.Not.Null);
                }
            }
        }

        [Test]
        public void MissingPlatform()
        {
            var missingPlatform = new MissingPlatform(KnownPlatforms.Windows.Name);
            Assert.That(missingPlatform, Is.EqualTo(KnownPlatforms.Windows.GetPlatform()));
        }
    }
}
