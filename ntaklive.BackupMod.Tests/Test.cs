using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ntaklive.BackupMod.Tests
{
    public class Test
    {
        private readonly ITestOutputHelper _out;

        public Test(ITestOutputHelper @out)
        {
            _out = @out;
        }
    
        [Fact]
        public void Test_test()
        {
            var parameterNames = new[] { "title", "count", "size", "other" };
            var groupNames = new[] { "size", "title", "other", "count" };
        
            groupNames = groupNames.OrderBy(x => Array.IndexOf(parameterNames, x)).ToArray();
        
            _out.WriteLine(parameterNames.Aggregate((s1, s2) => $"{s1} {s2}"));
            _out.WriteLine(groupNames.Aggregate((s1, s2) => $"{s1} {s2}"));
        }
    
    
    }
}