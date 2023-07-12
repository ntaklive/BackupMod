using System;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Application.Commands;
using ntaklive.BackupMod.Application.Commands.Args;
using Xunit;

namespace ntaklive.BackupMod.Tests
{
    public class CommandParserTests
    {
        [Theory]
        [InlineData("test name _123")]
        [InlineData("test name")]
        [InlineData("testName")]
        [InlineData("")]
        public void Parse_BackupCommandWithTitleArg_Success(string title)
        {
            var parser = new CommandParser();
            
            ValueResult<(Type commandType, CommandArgs args)> parseResult = parser.Parse($@"bp -t {title}");
            var args = parseResult.Data.args as BackupCommandArgs;

            Assert.True(parseResult.IsSuccess);
            Assert.True(parseResult.Data.commandType.IsAssignableFrom(typeof(BackupCommand)) == true);
            Assert.True(args != null);
            Assert.True(args!.Title == title);
        }              
        
        [Fact]
        public void Parse_BackupCommand_Success()
        {
            var parser = new CommandParser();
            
            ValueResult<(Type commandType, CommandArgs args)> parseResult = parser.Parse("bp");
            var args = parseResult.Data.args as BackupCommandArgs;

            Assert.True(parseResult.IsSuccess);
            Assert.True(parseResult.Data.commandType.IsAssignableFrom(typeof(BackupCommand)) == true);
            Assert.True(args != null);
            Assert.True(args!.Title == "");
        }        
        
        [Fact]
        public void Parse_BackupCommandWithInvalidArgument_Fail()
        {
            var parser = new CommandParser();
            
            ValueResult<(Type commandType, CommandArgs args)> parseResult = parser.Parse("bp invalid_argument");
            
            Assert.False(parseResult.IsSuccess);
        }        
        
        [Theory]
        [InlineData("@%$&*%*#")]
        [InlineData("@%$ &*%*#")]
        public void Parse_BackupCommandWithInvalidTitleArg_Fail(string title)
        {
            var parser = new CommandParser();
            
            ValueResult<(Type commandType, CommandArgs args)> parseResult = parser.Parse($@"bp -t {title}");

            Assert.False(parseResult.IsSuccess);
        }
    }
}