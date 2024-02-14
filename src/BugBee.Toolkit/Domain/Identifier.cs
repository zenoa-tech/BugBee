namespace BugBee.Toolkit.Domain;

public abstract record Identifier(string Prefix)
{
    protected string Id { get; private set;} = Ulid.NewUlid().ToString();

    public void Deconstruct(out string prefix, out string id) =>
        (prefix, id) = (Prefix, Id);

    public override string ToString() => $"{Prefix}_{Id}";

    public static bool TryParse<T>(string value, out T identifier)
        where T : Identifier, new()
    {
        identifier = new T();

        var parts = value.Split('_');
        if (parts.Length != 2)
        {
            return false;
        }

        if (parts[0] != identifier.Prefix)
        {
            return false;
        }

        if (!Ulid.TryParse(parts[1], out Ulid id))
        {
            return false;
        }
        identifier.Id = id.ToString();

        return true;
    }


}