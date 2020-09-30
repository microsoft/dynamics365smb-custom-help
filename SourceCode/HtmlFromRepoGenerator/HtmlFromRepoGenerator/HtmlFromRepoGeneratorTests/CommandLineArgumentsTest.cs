using System;
using System.IO;
using HtmlFromRepoGenerator;
using Xunit;

namespace HtmlFromRepoGeneratorTests
{
    public class CommandLineArgumentsTest
    {
        private static CommandLineArguments GetValidArgs()
        {
            return new CommandLineArguments
            {
                Out = Path.GetTempPath(),
                ReplaceUrl = "http://github.com/en-us",
                Lng = "en-US",
                ExternalText = "some-external-text"
            };
        }

        [Fact]
        public void Test_IsValid_Returns_True_If_Parameters_Are_Valid()
        {
            CommandLineArguments args = new CommandLineArguments();
            args.InitializeFromCmdLine("--out", "some-paths",
                                       "--replaceUrl", "https://github.com",
                                       "--externalText", "some-external-text",
                                       "--repo", "https://github.com/en-us");
            Assert.True(args.IsValid());
            Assert.Equal(string.Empty, args.ErrorText);

            args = new CommandLineArguments();
            args.InitializeFromCmdLine("--out", Path.GetTempPath(),
                                       "--replaceUrl", "https://github.com",
                                       "--externalText", "some-external-text",
                                       "--doNotClone",
                                       "--removeGitFolder",
                                       "--rtl",
                                       "--logs", Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            Assert.True(args.IsValid());
            Assert.Equal(string.Empty, args.ErrorText);

            args = new CommandLineArguments();
            args.InitializeFromCmdLine("--out", "some-out",
                                       "--enOut", "some-en-out",
                                       "--replaceUrl", "https://github.com/en-us",
                                       "--externalText", "some-external-text",
                                       "--repo", "https://github.com/de-de",
                                       "--enRepo", "https://github.com/en-us",
                                       "--lng", "en-us");
            Assert.True(args.IsValid());
            Assert.Equal(string.Empty, args.ErrorText);
        }

        [Fact]
        public void Test_IsValid_Returns_False_If_Mandatory_Parameters_Are_Missing()
        {
            CommandLineArguments args = new CommandLineArguments();

            args.InitializeEmpty();
            Assert.False(args.IsValid());

            const string expectedText = @"Please correct the following errors!
The --Out parameter is empty. Please specify valid path.";

            Assert.Equal(expectedText, args.ErrorText);
        }

        [Fact]
        public void Test_IsValid_Returns_False_If_DoNotClone_And_Repo_Set_Together()
        {
            CommandLineArguments args = GetValidArgs();
            args.DoNotClone = true;
            args.Repo = "some-repo";

            args.InitializeEmpty();
            Assert.False(args.IsValid());

            const string expectedErrorText = @"Please correct the following errors!
Parameters --donotclone and --repo could not be specified together.";

            Assert.Equal(expectedErrorText, args.ErrorText);
        }

        [Fact]
        public void Test_Is_Valid_Returns_False_If_DoNotClone_And_EnRepo_Set_Together()
        {
            CommandLineArguments args = GetValidArgs();
            args.DoNotClone = true;
            args.EnRepo = "some-repo";

            args.InitializeEmpty();
            Assert.False(args.IsValid());

            const string expectedErrorText = @"Please correct the following errors!
Parameters --donotclone and --enRepo could not be specified together.";

            Assert.Equal(expectedErrorText, args.ErrorText);
        }

        [Theory]
        [InlineData("abcd")]
        [InlineData("file://c:\\some-data\\1.txt")]
        public void Test_IsValid_Returns_False_If_Repo_Url_Is_Not_Valid(string url)
        {
            CommandLineArguments args = GetValidArgs();
            args.Out = "not-existing";
            args.Repo = url;

            args.InitializeEmpty();
            Assert.False(args.IsValid());

            const string expectedErrorText = @"Please correct the following errors!
Incorrect --repo parameter. Please specify valid absolute URL.";

            Assert.Equal(expectedErrorText, args.ErrorText);
        }

        [Theory]
        [InlineData((string)null)]
        [InlineData("")]
        public void Test_IsValid_Returns_False_If_Out_Is_Null_Or_Empty(string outPath)
        {
            CommandLineArguments args = GetValidArgs();
            args.Out = outPath;

            args.InitializeEmpty();
            Assert.False(args.IsValid());

            const string expectedErrorText = @"Please correct the following errors!
The --Out parameter is empty. Please specify valid path.";

            Assert.Equal(expectedErrorText, args.ErrorText);
        }

        [Fact]
        public void Test_IsValid_Returns_False_If_DoNotClone_Set_And_Repo_Not_Set_And_Out_Does_Not_Exist()
        {
            CommandLineArguments args = GetValidArgs();
            args.DoNotClone = true;
            args.Out = "not-existing";

            args.InitializeEmpty();
            Assert.False(args.IsValid());

            const string expectedErrorText = @"Please correct the following errors!
The path specified in --out doesn't exist: not-existing";

            Assert.Equal(expectedErrorText, args.ErrorText);
        }

        [Fact]
        public void Test_IsValid_Returns_False_If_DoNotClone_Not_Set_And_Repo_Set_And_Out_Does_Exists()
        {
            CommandLineArguments args = GetValidArgs();
            args.Repo = "https://github.com";

            args.InitializeEmpty();
            Assert.False(args.IsValid());

            string expectedErrorText = $"Please correct the following errors!\r\nThe path specified in --out already exists: {args.Out}";

            Assert.Equal(expectedErrorText, args.ErrorText);
        }

        [Fact]
        public void Test_IsValid_Returns_False_If_DoNotClone_Not_Set_And_EnRepo_Set_And_EnOut_Not_Set()
        {
            CommandLineArguments args = GetValidArgs();
            args.EnRepo = "https://github.com";

            args.InitializeEmpty();
            Assert.False(args.IsValid());

            const string expectedErrorText = @"Please correct the following errors!
The --enOut parameter is empty. Please specify valid path.";

            Assert.Equal(expectedErrorText, args.ErrorText);
        }

        [Theory]
        [InlineData("abcd")]
        [InlineData("file://c:\\some-data\\1.txt")]
        public void Test_IsValid_Returns_False_If_EnRepo_Url_Is_Not_Valid(string url)
        {
            CommandLineArguments args = GetValidArgs();
            args.EnRepo = url;
            args.EnOut = "some-en-out";
            args.ReplaceUrl = "https://microsoft.com/en-us";

            args.InitializeEmpty();
            Assert.False(args.IsValid());

            const string expectedErrorText = @"Please correct the following errors!
Incorrect --enRepo parameter. Please specify valid absolute URL.";

            Assert.Equal(expectedErrorText, args.ErrorText);
        }

        [Fact]
        public void Test_IsValid_Returns_False_If_DoNotClone_Set_And_EnRepo_Not_Set_And_EnOut_Doesnt_Exist()
        {
            CommandLineArguments args = GetValidArgs();
            args.DoNotClone = true;
            args.EnOut = "not-existing";
            args.Lng = "en-US";
            args.ReplaceUrl = "https://microsoft.com/en-us";

            args.InitializeEmpty();
            Assert.False(args.IsValid());

            const string expectedErrorText = @"Please correct the following errors!
The path specified in --enOut doesn't exist: not-existing";

            Assert.Equal(expectedErrorText, args.ErrorText);
        }

        [Fact]
        public void Test_IsValid_Returns_False_If_DoNotClone_Not_Set_And_EnRepo_Set_And_EnOut_Exists()
        {
            CommandLineArguments args = GetValidArgs();
            args.EnRepo = "https://github.com";
            args.EnOut = Path.GetTempPath();
            args.Lng = "en-US";
            args.ReplaceUrl = "https://microsoft.com/en-us";

            args.InitializeEmpty();
            Assert.False(args.IsValid());

            string expectedErrorText = $"Please correct the following errors!\r\nThe path specified in --enOut already exists: {args.EnOut}";

            Assert.Equal(expectedErrorText, args.ErrorText);
        }

        [Fact]
        public void Test_IsValid_Returns_False_If_Lng_Is_Not_Set_And_EnOut_Is_Set()
        {
            CommandLineArguments args = GetValidArgs();
            args.Lng = null;
            args.EnOut = "some-en-out";

            args.InitializeEmpty();
            Assert.False(args.IsValid());

            const string expectedErrorText = @"Please correct the following errors!
Please specify --lng parameter. Note: the --replaceUrl must contain a language identifier that match the value of -–lng value";

            Assert.Equal(expectedErrorText, args.ErrorText);
        }

        [Fact]
        public void Test_It_Should_Return_False_If_Couldnt_Create_Logs_Directory()
        {
            CommandLineArguments args = GetValidArgs();
            args.LogsDir = "*";

            args.InitializeEmpty();
            Assert.False(args.IsValid());

            const string expectedErrorText = @"Please correct the following errors!
Could not create directory * for the --logsDir parameter: Illegal characters in path.";

            Assert.Equal(expectedErrorText, args.ErrorText);
        }
    }
}