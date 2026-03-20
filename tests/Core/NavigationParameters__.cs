using System.Globalization;
using ADaxer.MvvmNav.Abstractions.Navigation;

[assembly: TUnit.Core.NotInParallel]

namespace ADaxer.MvvmNav.Core.Tests.Navigation;

public sealed class NavigationParameters__
{
    [Test]
    public async Task Empty_ShouldExposeEmptyNormalizedString()
    {
        // Arrange
        var sut = NavigationParameters.Empty;

        // Act
        var normalized = sut.ToNormalizedString();
        var missingValue = sut["missing"];

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(normalized).IsEqualTo(string.Empty);
            await Assert.That(missingValue).IsNull();
        }
    }

    [Test]
    public async Task Constructor_WithDictionary_ShouldCopyValues()
    {
        // Arrange
        var source = new Dictionary<string, object?>
        {
            ["Id"] = 42,
            ["Name"] = "Alice"
        };

        // Act
        var sut = new NavigationParameters(source);

        source["Id"] = 99;
        source["Extra"] = "ShouldNotAppear";

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(sut["Id"]).IsEqualTo(42);
            await Assert.That(sut["Name"]).IsEqualTo("Alice");
            await Assert.That(sut["Extra"]).IsNull();
        }
    }

    [Test]
    public async Task Constructor_WithTuples_ShouldStoreValues()
    {
        // Arrange
        var tuples = new (string key, object? value)[]
        {
            ("Id", 42),
            ("Name", "Alice"),
            ("Flag", true)
        };

        // Act
        var sut = new NavigationParameters(tuples);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(sut["Id"]).IsEqualTo(42);
            await Assert.That(sut["Name"]).IsEqualTo("Alice");
            await Assert.That(sut["Flag"]).IsEqualTo(true);
        }
    }

    [Test]
    public async Task Indexer_WithMissingKey_ShouldReturnNull()
    {
        // Arrange
        var sut = new NavigationParameters(("Id", 42));

        // Act
        var value = sut["missing"];

        // Assert
        await Assert.That(value).IsNull();
    }

    [Test]
    public async Task TryGetValue_WithMatchingType_ShouldReturnTrueAndTypedValue()
    {
        // Arrange
        var sut = new NavigationParameters(("Id", 42));

        // Act
        var success = sut.TryGetValue<int>("Id", out var value);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(success).IsTrue();
            await Assert.That(value).IsEqualTo(42);
        }
    }

    [Test]
    public async Task TryGetValue_WithMissingKey_ShouldReturnFalse()
    {
        // Arrange
        var sut = new NavigationParameters(("Id", 42));

        // Act
        var success = sut.TryGetValue<int>("missing", out var value);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(success).IsFalse();
            await Assert.That(value).IsEqualTo(0);
        }
    }

    [Test]
    public async Task TryGetValue_WithWrongType_ShouldReturnFalse()
    {
        // Arrange
        var sut = new NavigationParameters(("Id", 42));

        // Act
        var success = sut.TryGetValue<string>("Id", out var value);

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(success).IsFalse();
            await Assert.That(value).IsNull();
        }
    }

    [Test]
    public async Task GetValueOrDefault_WithMatchingType_ShouldReturnTypedValue()
    {
        // Arrange
        var sut = new NavigationParameters(("Id", 42));

        // Act
        var value = sut.GetValueOrDefault<int>("Id");

        // Assert
        await Assert.That(value).IsEqualTo(42);
    }

    [Test]
    public async Task GetValueOrDefault_WithMissingKey_ShouldReturnDefault()
    {
        // Arrange
        var sut = new NavigationParameters(("Id", 42));

        // Act
        var value = sut.GetValueOrDefault<int>("missing");

        // Assert
        await Assert.That(value).IsEqualTo(0);
    }

    [Test]
    public async Task GetValueOrDefault_WithWrongType_ShouldReturnDefault()
    {
        // Arrange
        var sut = new NavigationParameters(("Id", 42));

        // Act
        var value = sut.GetValueOrDefault<string>("Id");

        // Assert
        await Assert.That(value).IsNull();
    }

    [Test]
    public async Task ToNormalizedString_ShouldSortKeysOrdinally()
    {
        // Arrange
        var sut = new NavigationParameters(
            ("b", 2),
            ("a", 1),
            ("c", 3));

        // Act
        var normalized = sut.ToNormalizedString();

        // Assert
        await Assert.That(normalized).IsEqualTo("a=1|b=2|c=3");
    }

    [Test]
    public async Task ToNormalizedString_ShouldEscapeReservedCharacters()
    {
        // Arrange
        var sut = new NavigationParameters(
            ("a|b=c\\d", "x|y=z\\w"));

        // Act
        var normalized = sut.ToNormalizedString();

        // Assert
        await Assert.That(normalized).IsEqualTo(@"a\|b\=c\\d=x\|y\=z\\w");
    }

    [Test]
    public async Task ToNormalizedString_ShouldRepresentNullAsLiteralNull()
    {
        // Arrange
        var sut = new NavigationParameters(("Value", null));

        // Act
        var normalized = sut.ToNormalizedString();

        // Assert
        await Assert.That(normalized).IsEqualTo("Value=null");
    }

    [Test]
    public async Task ToNormalizedString_ShouldUseInvariantFormatting_ForFormattableValues()
    {
        // Arrange
        var previousCulture = CultureInfo.CurrentCulture;

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var sut = new NavigationParameters(("Number", 12.5m));

            // Act
            var normalized = sut.ToNormalizedString();

            // Assert
            await Assert.That(normalized).IsEqualTo("Number=12.5");
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    [Test]
    public async Task Constructor_WithNullDictionary_ShouldThrow()
    {
        // Arrange
        Func<NavigationParameters> act =
            () => new NavigationParameters((IDictionary<string, object?>)null!);

        // Act & Assert
        await Assert.That(act).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Constructor_WithDuplicateTupleKeys_ShouldThrow()
    {
        // Arrange
        Func<NavigationParameters> act =
            () => new NavigationParameters(
                ("Id", 1),
                ("Id", 2));

        // Act & Assert
        await Assert.That(act).Throws<ArgumentException>();
    }
}
