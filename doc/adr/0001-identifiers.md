# 1. Identifiers

Date: 2024-02-15

## Status

Status: Accepted on 2024-02-17

## Context

### Introduction
The context will provide some historical background. The context will present
the canonical problem given for primitive identifiers. The context will then
present the primary problem in the BugBee system given the lens of the BugBee
Project Manifesto. The solutions for using primitive types, using typed records,
and using source generators will be considered. The choice of random generator
will briefly be discussed.


### Background
The system will consist of many entities and value objects. Entities have
identifiers, and are equal when the identifiers are equal. Identifiers have
typically been defined as primitive values. In the past, the database would
provide an integer sequence. The default choice for sometime has been some form
of GUID or UUID. The primitive identifier serves systems very well, as the
primitive identifier provides very little friction. The compiler or the GUID
struct provide all the methods and operations developers need.

### Primitive Obsession
The canonical example of what's wrong with using scalar or GUID values is often
cited as an example of "primitive obsession". Some system may include a method
that takes two identifiers.

```csharp
public Book AddAuthorToBook(Guid bookId, Guid authorId)
    => new GetBook(bookId).WithAuthor(authorId);

```

The usage of the method would be something like this:

```csharp
...
    AddAuthorToBook(authorId, bookId);
...
```

The code above contains a very subtle bug. The compiler and the type system
can't detect that you accidentally switched the order of the parameters in the
call to the method.

### Defining a Type for the Identifier
A common solution to the primitive obsession problem is to define a class around
the primitive type:

``` csharp
public record BookId(Guid Id);
public record AuthorId(Guid Id);

AuthorId authorId = new(Guid.NewGuid());
BookId bookId = new(Guid.NewGuid());

public Book AddAuthorToBook(BookId bookId, AuthorId authorId)
    => new GetBook(bookId).WithAuthor(authorId);

...
    AddAuthorToBook(authorId, bookId); // compiler error!
```

This fixes the problem of mixing identifiers of one type for another. When
viewed through the lens of **developer experience**, however, this requires
developers to define another type for every entity type created. **This smells of overhead and boilerplate**.

## Choice of Which Primitive Type
Integer sequences assigned from the database aren't in widespread use in modern
systems. Letting the system define the identifier has too many benefits in terms
of isolation and testing (e.g., you can test entity creation without a
database).

The GUID or UUID are the defacto standard. They do provide friction as they are
binary types that have to be serialized somehow to store in the database or over
the wire. This requires another technical decision and something developers have
to be aware of when they want to write queries, especially if there is some
adhoc query tool in use, or the developer is querying for identifiers in the
log. The solution is often to serialize the GUID as a string.

Strings are universal and humans can understand and type them. Strings have a
strong benefit when using an ad hoc reporting tool or a log monitor. When
combined with someway to generate random strings, they provide very little
friction for humans. The tradeoff is performance. Performance is trumped by
**developer experience** as long as **quality** can be maintained.

## String Prefixes
Stripe goes a step further. Stripe defines its identifiers with a type prefix.
The identifier for a payment method might look something like this
`pm_a1vXTabwwxxzAf`, and an identifier for a customer might look something like
this `cus_76kfjsawerttyyzw23`. This allows the human compilers, to avoid mixing
identifiers, much like the primitive obsession mitigation does for the machine
compiler. This has strong credits towards both **developer experience** and
**quality of design**. This is a desireable property.

Where does the prefix for identifiers live (e.g., `pm_` and `cus_`)? Where does
the code to generate a new identifier live? Given our system is constructed
using a mainstream language supporting implementation inheritance, a single
abstract base class could provide the implementation as well as provide a hook
for orthogonal concerns like serialization.

## Using a Type for Identifiers
The context has now come back to both avoiding primitive obsession as a
side effect of improving **developer experience** and **quality**. An
`Identifier` base class can be defined to encapsulate the construction of
derived identifiers with a common prefix and a random identifier. The
implementation of the class and example of usage is given below.

``` csharp
public record PersonId: Identifier
{
    public PersonId(): Identifier("PER")
    {

    }
}


PersonId id = new();

```

## Performance
The `record`, without a struct, is going to cause a heap allocation. The
`record` keyword luckily provides value-type semantics for us. There is a
discussion of performance and some benchmarks given in the referenced video[1].
Unfortunately, `struct` is sealed and there's no way to provide default
implementations in a base class.

## Default Interface Implementations
Related to extension methods in use, the default implementation in an interface
is a viable choice for providing base usage. Without the need for the prefix,
this might provide the least developer friction while avoiding the performance
problems of classes and record classes. When requiring the prefix,
the field to store prefix somewhere else, statically, and introduces too much
developer friction.

## Source Generators and Macro Support
C# doesn't support macros. There is support for source generation as a part of
the compilation process. There's even a library[2] that supports strongly typed
identifiers. Unfortunately, this introduces magic into the system and causes
impedance instead of friction; a poor tradeoff.

## Generating the Random Part of the String
There are many choices here. The use of a GUID would be fine. There's also a
ULID with library support[3]. The ULID is another binary structure, has strong
uniqueness guarantees, and also has the attribute of being lexicographical
sortable and monotonically ordered. This should improve indexing based on
identifiers as a result, and wont impact the developer.

## Decision
- Use a strongly typed identifier that uses a string prefix and a ULID random
   identifier.

Arbitrarily, follow these requirements for defining an identifier prefix.
- The prefix MUST use uppercase letters only.
- The prefix MUST be 5 characters or less.
- The prefix SHOULD be more than 1 character (prefer 2 or 3 character
  prefixes).
- The prefix SHOULD be less than 4 characters (prefer 2 or 3 character
  prefixes).

## Consequences
There is some friction introduced into the **developer experience**. The
tradeoff is the improvement to the lives of anyone having to use an identifier
for queries or troubleshooting.

## Appendix

### Wish list for `C#`
- syntax

    ```csharp
    // Wish list for C# syntax
    public record PersonId: Identifier("PER");

    ```

- Allow structs to have inheritance in narrow circumstances.

## References
- [1] [Stop Using Records as Strongly Typed IDs!](https://www.youtube.com/watch?v=dJxVj6390hk)
- [2] [StronglyTypedId](https://github.com/andrewlock/StronglyTypedId)
- [2] [ULID](https://github.com/Cysharp/Ulid)
