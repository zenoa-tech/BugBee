namespace BugBee.UnitTest.Toolkit.Domain;

using BugBee.Toolkit.Domain;

public class IndentifierTests
{
    private const string TestIdentifierPrefix = "ABC";

    public record TestIdentifier: Identifier
    {
        public TestIdentifier(): base(TestIdentifierPrefix) { }
        public override string ToString() => base.ToString();
    }


    [Test]
    public void IdentifiersHaveAPrefix()
    {
        TestIdentifier actual = new();
        Assert.That(actual.Prefix, Is.EqualTo(TestIdentifierPrefix));
    }


    [Test]
    public void IdentifiersDeconstructToPrefixAndId()
    {
        TestIdentifier identifier = new();
        var (prefix, id) = identifier;
        Assert.That(prefix, Is.EqualTo(TestIdentifierPrefix));
        Assert.That(id, Is.Not.Null.And.Not.Empty);
    }


    [Test]
    public void IdentifiersToStringIsPrefixAndId()
    {
        TestIdentifier identifier = new();
        var (prefix, id) = identifier;
        Assert.That(identifier.ToString(), Is.EqualTo($"{prefix}_{id}"));
    }


    [Test]
    public void IdentifierTryParseConstructsTypedIdentifierFromString()
    {
        var id = Ulid.NewUlid().ToString();
        var input = $"{TestIdentifierPrefix}_{id}";
        Assert.That(Identifier.TryParse(input, out TestIdentifier _), Is.True);
    }


    [Test]
    public void IdentifierTryParseReturnsFalseForEmptyString()
    {
        var input = string.Empty;
        Assert.That(Identifier.TryParse(input, out TestIdentifier _), Is.False);
    }


    [Test]
    public void IdentifierTryParseReturnsFalseForStringWithoutUnderscore()
    {
        var input = $"{TestIdentifierPrefix}{Ulid.NewUlid()}";
        Assert.That(Identifier.TryParse(input, out TestIdentifier _), Is.False);
    }


    [Test]
    public void IdentifierTryParseReturnsFalseForStringWithWrongPrefix()
    {
        var input = $"XYZ_{Ulid.NewUlid()}";
        Assert.That(Identifier.TryParse(input, out TestIdentifier _), Is.False);
    }


    [Test]
    public void IdentifierTryParseReturnsFalseForIdWithNonUlidId()
    {
        var input = $"{TestIdentifierPrefix}_{Guid.NewGuid()}";
        Assert.That(Identifier.TryParse(input, out TestIdentifier _), Is.False);
    }

}
