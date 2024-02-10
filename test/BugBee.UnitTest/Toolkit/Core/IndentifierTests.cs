namespace BugBee.UnitTest.Toolkit.Core;

using BugBee.Toolkit.Core;

public class IndentifierTests
{
    public record TestIdentifier: Identifier
    {
        public TestIdentifier() : base("test") { }
    }


    [Test]
    public void IdentifiersHaveTwoPartsThatDeconstructToStrings()
    {
        TestIdentifier identifier = new();
        var (prefix, suffix) = identifier;
        Assert.Multiple(() =>
        {
            Assert.That(prefix, Is.EqualTo("test"));
            Assert.That(suffix, Is.Not.Empty);
        });
    }


    [Test]
    public void IdentifiersHaveSuffixThatParsesToUlid()
    {
        var (_, suffix) = new TestIdentifier();

        Assert.That(suffix, Is.Not.Empty);
        Assert.That(Ulid.TryParse(suffix, out _), Is.True);
    }

}
