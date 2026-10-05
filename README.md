# Do more in .NET

**Domore** is a family of small, focused .NET libraries for configuration and command lines. Each package does one thing well, has almost no dependencies, and targets everything from .NET Framework 4.0 through .NET 10, so you can use it in a brand-new service or a decade-old desktop app.

All packages are MIT licensed and ship with SourceLink and symbol packages, so you can step straight into the source while debugging.

## Packages

| Package | What it does |
|---------|--------------|
| [Domore.Conf](#domoreconf) | Populate plain objects from forgiving `key = value` text or `.conf` files. |
| [Domore.Conf.Cli](#domoreconfcli) | Turn command lines into typed objects, with usage and help text generated for you. |
| [Domore.Conf.ConfigurationManager](#domoreconfconfigurationmanager) | Use `app.config` `<appSettings>` as a Domore.Conf source. |

Install any of them with `dotnet add package <name>`.

---

## Domore.Conf

Configure .NET objects from readable text. There's no schema, no binding setup, and no ceremony: write `key = value` lines, and Domore.Conf fills in your existing objects, including nested properties, lists, and dictionaries. It can also write objects back out as conf text.

```csharp
using Domore.Conf.Extensions;

var settings = new AppSettings().ConfFrom(@"
    AppSettings.Host             = example.com
    AppSettings.Port             = 8080
    AppSettings.Database.Host    = db.example.com
    AppSettings.Servers[0]       = primary
    AppSettings.Labels[region]   = west
");

var text = settings.ConfText(); // ...and back to conf text
```

- **Forgiving by design.** Keys ignore case and whitespace (`Home planet` matches `HomePlanet`), and lines that aren't settings are ignored, so comments are optional.
- **Files that compose.** Load a file with `Conf.Contain("settings.conf")`, split settings across files with `@conf.include`, and let later settings override earlier ones.
- **Hot reload.** `ConfFile` can watch a file and reconfigure your object whenever the file changes.
- **Zero-config default.** With no source set, Domore.Conf finds the `.conf` file next to your app and can seed it from a `.conf.default` file.

📖 [Full Domore.Conf documentation](source/Domore.Conf/README.md)

## Domore.Conf.Cli

Describe a command as a class, and let Domore.Conf.Cli parse the command line, convert the values, enforce required arguments, run your validations, and generate usage and manual text from the same type.

```csharp
using Domore.Conf;
using Domore.Conf.Cli;

[ConfHelp("Copies a file.")]
public sealed class Copy {
    [CliArgument(0), CliRequired, ConfHelp("The file to copy.")]
    public string Source { get; set; }

    [CliArgument(1), CliRequired, ConfHelp("Where to copy the file.")]
    public string Destination { get; set; }

    [ConfHelp("Replace the destination if it exists.")]
    public bool Overwrite { get; set; }
}

var cli = new CliProvider(new CliSetup());
var copy = cli.Configure(new Copy(), "copy report.txt backup/ overwrite=true");

Console.WriteLine(cli.Display(new Copy()));
// copy <source> <destination> [overwrite=<true/false>]
```

Positional arguments, argument lists, `name=value` parameters, examples, and clear, typed exceptions (`Missing required: source, destination`) all come built in.

📖 [Full Domore.Conf.Cli documentation](source/Domore.Conf.Cli/README.md)

## Domore.Conf.ConfigurationManager

Already have settings in `app.config`? Point Domore.Conf at `<appSettings>` and keep the same configuration code:

```csharp
using Domore.Conf;

Conf.ContentProvider = new AppSettingsProvider();
var settings = Conf.Configure(new AppSettings());
```

---

## License

[MIT](LICENSE) © Ken Yourek
