using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace DirectorySizeAgent
{
    [TestFixture]
    public class ScannerTest
    {
        [TestCase(@"C:\\Windows", 1000000)]
        [TestCase(@"blah", 0)]
        public void TestDirectories(string path, long minSize)
        {
            var size = Scanner.GetDirectorySize(path);
            size.Should().BeGreaterOrEqualTo(minSize);
        }
    }
}
