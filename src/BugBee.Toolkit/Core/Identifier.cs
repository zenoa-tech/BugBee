namespace BugBee.Toolkit.Core;


public abstract record Identifier(string Prefix)
{
    protected readonly string prefix = Prefix;
    protected readonly Ulid suffix = Ulid.NewUlid();


    public void Deconstruct(out string prefix, out string suffix)
    {
        prefix = this.prefix;
        suffix = this.suffix.ToString();
    }
}
