using NUnit.Framework;
using System;

namespace Unity.Build.Tests
{
    [TestFixture]
    class BuildPipelineTests : BuildTestsBase
    {
        [Test]
        public void CanBuild_IsTrue()
        {
            var pipeline = new TestBuildPipeline();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.True);
        }

        [Test]
        public void CanBuild_WithoutConfig_Throws()
        {
            var pipeline = new TestBuildPipeline();
            Assert.Throws<ArgumentNullException>(() => pipeline.CanBuild(null));
        }

        [Test]
        public void CanBuild_WithoutPlatform_Throws()
        {
            var pipeline = new TestBuildPipeline();
            var config = BuildConfiguration.CreateInstance();
            Assert.Throws<NullReferenceException>(() => pipeline.CanBuild(config));
        }

        [Test]
        public void CanBuild_WhenCannotBuild_IsFalse()
        {
            var pipeline = new TestBuildPipelineCantBuild();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.False);
        }

        [Test]
        public void CanBuild_WithComponents_IsTrue()
        {
            var pipeline = new TestBuildPipelineWithUsedComponents();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.True);
        }

        [Test]
        public void CanBuild_WithMissingComponents_IsTrue()
        {
            var pipeline = new TestBuildPipelineWithMissingComponents();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.True);
        }

        [Test]
        public void CanBuild_WithInvalidComponents_Throws()
        {
            var pipeline = new TestBuildPipelineWithInvalidComponents();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.Throws<InvalidOperationException>(() => pipeline.CanBuild(config));
        }

        [Test]
        public void Build_Succeeds()
        {
            var pipeline = new TestBuildPipeline();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.True);
            using (var progress = new BuildProgress("Building...", "Please wait!"))
            {
                Assert.That(pipeline.Build(config, progress).Succeeded, Is.True);
            }
        }

        [Test]
        public void Build_WithoutConfig_Throws()
        {
            var pipeline = new TestBuildPipeline();
            Assert.Throws<ArgumentNullException>(() => pipeline.Build(null));
        }

        [Test]
        public void Build_WithoutPlatform_Throws()
        {
            var pipeline = new TestBuildPipeline();
            var config = BuildConfiguration.CreateInstance();
            Assert.Throws<NullReferenceException>(() => pipeline.Build(config));
        }

        [Test]
        public void Build_WithoutProgress_Succeeds()
        {
            var pipeline = new TestBuildPipeline();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.True);
            Assert.That(pipeline.Build(config).Succeeded, Is.True);
        }

        //[Test]
        //public void Build_WhileUnityIsCompiling_Fails()
        //{
        //    //@TODO: How to test this?
        //}

        [Test]
        public void Build_WhenCannotBuild_Fails()
        {
            var pipeline = new TestBuildPipelineCantBuild();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.False);
            Assert.That(pipeline.Build(config).Succeeded, Is.False);
        }

        [Test]
        public void Build_WhenBuildFails_Fails()
        {
            var pipeline = new TestBuildPipelineBuildFails();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.True);
            Assert.That(pipeline.Build(config).Succeeded, Is.False);
        }

        [Test]
        public void Build_WhenBuildThrows_Fails()
        {
            var pipeline = new TestBuildPipelineBuildThrows();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.True);
            Assert.That(pipeline.Build(config).Succeeded, Is.False);
        }

        [Test]
        public void Build_WithComponents_Succeeds()
        {
            var pipeline = new TestBuildPipelineWithUsedComponents();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.True);
            Assert.That(pipeline.Build(config).Succeeded, Is.True);
        }

        [Test]
        public void Build_WithMissingComponents_Fails()
        {
            var pipeline = new TestBuildPipelineWithMissingComponents();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.True);
            Assert.That(pipeline.Build(config).Succeeded, Is.False);
        }

        [Test]
        public void Build_WithInvalidComponents_Throws()
        {
            var pipeline = new TestBuildPipelineWithInvalidComponents();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.Throws<InvalidOperationException>(() => pipeline.CanBuild(config));
            Assert.Throws<InvalidOperationException>(() => pipeline.Build(config));
        }

        [Test]
        public void BuildIncremental_Succeeds()
        {
            var pipeline = new TestBuildPipeline();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.True);
            using (var process = pipeline.BuildIncremental(config))
            {
                while (process.Update()) { }
                Assert.That(process.Result.Succeeded, Is.True);
            }
        }

        [Test]
        public void BuildIncremental_WithoutConfig_Throws()
        {
            var pipeline = new TestBuildPipeline();
            Assert.Throws<ArgumentNullException>(() => pipeline.BuildIncremental(null));
        }

        [Test]
        public void BuildIncremental_WithoutPlatform_Throws()
        {
            var pipeline = new TestBuildPipeline();
            var config = BuildConfiguration.CreateInstance();
            Assert.Throws<NullReferenceException>(() => pipeline.BuildIncremental(config));
        }

        [Test]
        public void BuildIncremental_WhenCannotBuild_Fails()
        {
            var pipeline = new TestBuildPipelineCantBuild();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.False);
            using (var process = pipeline.BuildIncremental(config))
            {
                while (process.Update()) { }
                Assert.That(process.Result.Succeeded, Is.False);
            }
        }

        [Test]
        public void BuildIncremental_WhenBuildFails_Fails()
        {
            var pipeline = new TestBuildPipelineBuildFails();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.True);
            using (var process = pipeline.BuildIncremental(config))
            {
                while (process.Update()) { }
                Assert.That(process.Result.Succeeded, Is.False);
            }
        }

        [Test]
        public void BuildIncremental_WhenBuildThrows_Fails()
        {
            var pipeline = new TestBuildPipelineBuildThrows();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.True);
            using (var process = pipeline.BuildIncremental(config))
            {
                while (process.Update()) { }
                Assert.That(process.Result.Succeeded, Is.False);
            }
        }

        [Test]
        public void BuildIncremental_WithComponents_Succeeds()
        {
            var pipeline = new TestBuildPipelineWithUsedComponents();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.True);
            using (var process = pipeline.BuildIncremental(config))
            {
                while (process.Update()) { }
                Assert.That(process.Result.Succeeded, Is.True);
            }
        }

        [Test]
        public void BuildIncremental_WithMissingComponents_Fails()
        {
            var pipeline = new TestBuildPipelineWithMissingComponents();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanBuild(config).Succeeded, Is.True);
            using (var process = pipeline.BuildIncremental(config))
            {
                while (process.Update()) { }
                Assert.That(process.Result.Succeeded, Is.False);
            }
        }

        [Test]
        public void BuildIncremental_WithInvalidComponents_Throws()
        {
            var pipeline = new TestBuildPipelineWithInvalidComponents();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.Throws<InvalidOperationException>(() => pipeline.CanBuild(config));
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var process = pipeline.BuildIncremental(config))
                {
                    while (process.Update()) { }
                    Assert.That(process.Result.Succeeded, Is.False);
                }
            });
        }

        [Test]
        public void CanRun_IsTrue()
        {
            var pipeline = new TestBuildPipeline();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.Build(config).Succeeded, Is.True);
            Assert.That(pipeline.CanRun(config).Succeeded, Is.True);
        }

        [Test]
        public void CanRun_WithoutConfig_Throws()
        {
            var pipeline = new TestBuildPipeline();
            Assert.Throws<ArgumentNullException>(() => pipeline.CanRun(null));
        }

        [Test]
        public void CanRun_WithoutPlatform_Throws()
        {
            var pipeline = new TestBuildPipeline();
            var config = BuildConfiguration.CreateInstance();
            Assert.Throws<NullReferenceException>(() => pipeline.CanRun(config));
        }

        [Test]
        public void CanRun_WithoutBuild_IsFalse()
        {
            var pipeline = new TestBuildPipeline();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanRun(config).Succeeded, Is.False);
        }

        [Test]
        public void CanRun_WhenBuildFails_IsFalse()
        {
            var pipeline = new TestBuildPipelineBuildFails();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.Build(config).Succeeded, Is.False);
            Assert.That(pipeline.CanRun(config).Succeeded, Is.False);
        }

        [Test]
        public void CanRun_WhenCannotRun_IsFalse()
        {
            var pipeline = new TestBuildPipelineCantRun();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.Build(config).Succeeded, Is.True);
            Assert.That(pipeline.CanRun(config).Succeeded, Is.False);
        }

        [Test]
        public void CanRun_WithComponents_IsTrue()
        {
            var pipeline = new TestBuildPipelineWithUsedComponents();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.Build(config).Succeeded, Is.True);
            Assert.That(pipeline.CanRun(config).Succeeded, Is.True);
        }

        [Test]
        public void CanRun_WithMissingComponents_IsFalse()
        {
            var pipeline = new TestBuildPipelineWithMissingComponents();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.Build(config).Succeeded, Is.False);
            Assert.That(pipeline.CanRun(config).Succeeded, Is.False);
        }

        [Test]
        public void CanRun_WithInvalidComponents_IsFalse()
        {
            var pipeline = new TestBuildPipelineWithInvalidComponents();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.Throws<InvalidOperationException>(() => pipeline.Build(config));
            Assert.That(pipeline.CanRun(config).Succeeded, Is.False);
        }

        [Test]
        public void Run_Succeeds()
        {
            var pipeline = new TestBuildPipeline();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.Build(config).Succeeded, Is.True);
            Assert.That(pipeline.CanRun(config).Succeeded, Is.True);
            using (var result = pipeline.Run(config))
            {
                Assert.That(result.Succeeded, Is.True);
            }
        }

        [Test]
        public void Run_WithoutConfig_Throws()
        {
            var pipeline = new TestBuildPipeline();
            Assert.Throws<ArgumentNullException>(() => pipeline.Run(null));
        }

        [Test]
        public void Run_WithoutPlatform_Fails()
        {
            var pipeline = new TestBuildPipeline();
            var config = BuildConfiguration.CreateInstance();
            Assert.That(pipeline.Run(config).Succeeded, Is.False);
        }

        [Test]
        public void Run_WhenCannotRun_Fails()
        {
            var pipeline = new TestBuildPipelineCantRun();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.Build(config).Succeeded, Is.True);
            Assert.That(pipeline.CanRun(config).Succeeded, Is.False);
            Assert.That(pipeline.Run(config).Succeeded, Is.False);
        }

        [Test]
        public void Run_WithoutBuild_Fails()
        {
            var pipeline = new TestBuildPipeline();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.CanRun(config).Succeeded, Is.False);
            using (var result = pipeline.Run(config))
            {
                Assert.That(result.Succeeded, Is.False);
            }
        }

        [Test]
        public void Run_WhenBuildFails_Fails()
        {
            var pipeline = new TestBuildPipelineBuildFails();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.Build(config).Succeeded, Is.False);
            Assert.That(pipeline.CanRun(config).Succeeded, Is.False);
            using (var result = pipeline.Run(config))
            {
                Assert.That(result.Succeeded, Is.False);
            }
        }

        [Test]
        public void Run_WhenCannotRun_IsFalse()
        {
            var pipeline = new TestBuildPipelineCantRun();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.Build(config).Succeeded, Is.True);
            Assert.That(pipeline.CanRun(config).Succeeded, Is.False);
            using (var result = pipeline.Run(config))
            {
                Assert.That(result.Succeeded, Is.False);
            }
        }

        [Test]
        public void Run_WhenRunFails_Fails()
        {
            var pipeline = new TestBuildPipelineRunFails();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.Build(config).Succeeded, Is.True);
            Assert.That(pipeline.CanRun(config).Succeeded, Is.True);
            using (var result = pipeline.Run(config))
            {
                Assert.That(result.Succeeded, Is.False);
            }
        }

        [Test]
        public void Run_WhenRunThrows_Fails()
        {
            var pipeline = new TestBuildPipelineRunThrows();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.Build(config).Succeeded, Is.True);
            Assert.That(pipeline.CanRun(config).Succeeded, Is.True);
            using (var result = pipeline.Run(config))
            {
                Assert.That(result.Succeeded, Is.False);
            }
        }

        [Test]
        public void Run_WithComponents_Succeeds()
        {
            var pipeline = new TestBuildPipelineWithUsedComponents();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.Build(config).Succeeded, Is.True);
            Assert.That(pipeline.CanRun(config).Succeeded, Is.True);
            using (var result = pipeline.Run(config))
            {
                Assert.That(result.Succeeded, Is.True);
            }
        }

        [Test]
        public void Run_WithMissingComponents_Fails()
        {
            var pipeline = new TestBuildPipelineWithMissingComponents();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.Build(config).Succeeded, Is.False);
            Assert.That(pipeline.CanRun(config).Succeeded, Is.False);
            using (var result = pipeline.Run(config))
            {
                Assert.That(result.Succeeded, Is.False);
            }
        }

        [Test]
        public void Run_WithInvalidComponents_Fails()
        {
            var pipeline = new TestBuildPipelineWithInvalidComponents();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.Throws<InvalidOperationException>(() => pipeline.Build(config));
            Assert.That(pipeline.CanRun(config).Succeeded, Is.False);
            using (var result = pipeline.Run(config))
            {
                Assert.That(result.Succeeded, Is.False);
            }
        }

        [Test]
        public void Clean_Succeeds()
        {
            var pipeline = new TestBuildPipeline();
            var config = BuildConfiguration.CreateInstance(c => c.SetComponent<TestBuildPipelineComponent>());
            Assert.That(pipeline.Clean(config).Succeeded, Is.True);
        }

        [Test]
        public void Clean_WithoutConfig_Throws()
        {
            var pipeline = new TestBuildPipeline();
            Assert.Throws<ArgumentNullException>(() => pipeline.Clean(null));
        }

        [Test]
        public void Clean_WithoutPlatform_Fails()
        {
            var pipeline = new TestBuildPipeline();
            var config = BuildConfiguration.CreateInstance();
            Assert.That(pipeline.Clean(config).Succeeded, Is.False);
        }
    }
}
