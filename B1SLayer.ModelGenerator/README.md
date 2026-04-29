# B1SLayer.ModelGenerator

A command-line tool for generating strongly-typed **C#** or **TypeScript** model classes from SAP Business One Service Layer OData V4 metadata files (EDMX).

Instead of manually maintaining hundreds of model classes, `B1SLayer.ModelGenerator` reads the metadata file and generates a separate file for each type — ready to use in your project.

---

## Features

- Generates code from OData V4 EDMX metadata files
- Supports **C#** and **TypeScript** as target languages
- Creates one file per type (EnumType, ComplexType, EntityType, Action, Function)
- Handles `Collection(...)` types → `List<T>` / `T[]`
- Handles `Nullable` attributes correctly
- Strips namespace prefixes (e.g. `SAPB1.TeamMember` → `TeamMember`)
- Filter generated types with `--include` and `--exclude`

---

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download) or later

---

## Installation

```bash
dotnet tool install --global B1SLayer.ModelGenerator
```

Or clone and build from source:

```bash
git clone https://github.com/SoftwareDesign-Solution/B1SLayer.ModelGenerator.git
cd B1SLayer.ModelGenerator
dotnet build
```

---

## Usage

```bash
B1SLayer.ModelGenerator --metadatafile servicelayer_metadata_v4.xml --namespace Example.Dto --outputdir="Dto" --language cs
```

```bash
B1SLayer.ModelGenerator --metadatafile servicelayer_metadata_v4.xml --namespace Example.Dto --outputdir="Dto" --language ts
```

---

## Command-Line Options

| Option | Alias | Required | Description |
|---|---|---|---|
| `--metadatafile` | `-m` | ✅ | Path to the OData V4 EDMX metadata file |
| `--namespace` | `-n` | ✅ | Target namespace for generated classes (C# only) |
| `--outputdir` | `-o` | ✅ | Output directory for generated files |
| `--language` | | ✅ | Target language: `CSharp` / `cs` or `TypeScript` / `ts` |
| `--include` | | ❌ | Only generate these types (comma- or space-separated) |
| `--exclude` | | ❌ | Skip these types (comma- or space-separated) |

---

## Examples

### Generate C# classes

```bash
B1SLayer.ModelGenerator \
  --metadatafile servicelayer_metadata_v4.xml \
  --namespace Example.Dto \
  --outputdir ./Dto \
  --language cs
```

### Generate TypeScript types

```bash
B1SLayer.ModelGenerator \
  --metadatafile servicelayer_metadata_v4.xml \
  --namespace Example.Dto \
  --outputdir ./models \
  --language ts
```

### Only generate specific types

```bash
B1SLayer.ModelGenerator \
  --metadatafile servicelayer_metadata_v4.xml \
  --namespace Example.Dto \
  --outputdir ./Dto \
  --language cs \
  --include Document,BusinessPartner,Invoice
```

### Exclude specific types

```bash
B1SLayer.ModelGenerator \
  --metadatafile servicelayer_metadata_v4.xml \
  --namespace Example.Dto \
  --outputdir ./Dto \
  --language cs \
  --exclude Document,BusinessPartner
```

### Combine include and exclude

```bash
# Of Document, BusinessPartner and BPAddress — generate only Document and BusinessPartner
B1SLayer.ModelGenerator \
  --metadatafile servicelayer_metadata_v4.xml \
  --namespace Example.Dto \
  --outputdir ./Dto \
  --language cs \
  --include Document,BusinessPartner,BPAddress \
  --exclude BPAddress
```

---

## Output

Each type from the metadata file gets its own file in the output directory:

```
Dto/
├── BoStatus.cs              ← EnumType
├── BoYesNoEnum.cs           ← EnumType
├── BPAddress.cs             ← ComplexType
├── DocumentLine.cs          ← ComplexType
├── Document.cs              ← EntityType
├── BusinessPartner.cs       ← EntityType
└── ...
```

### C# Example Output

```csharp
namespace Example.Dto
{
    public class Document
    {
        public int DocEntry { get; set; }
        public int? DocNum { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime? DocDueDate { get; set; }
        public List<DocumentLine> DocumentLines { get; set; }
    }
}
```

### TypeScript Example Output

```typescript
export type Document = {
    DocEntry: number;
    DocNum: number | null;
    CardCode: string | null;
    CardName: string | null;
    DocDate: Date;
    DocDueDate: Date | null;
    DocumentLines: DocumentLine[] | null;
}
```

---

## EDM to C# Type Mapping

| EDM Type | C# Type |
|---|---|
| `Edm.String` | `string` |
| `Edm.Int16` | `short` |
| `Edm.Int32` | `int` |
| `Edm.Int64` | `long` |
| `Edm.Decimal` | `decimal` |
| `Edm.Double` | `double` |
| `Edm.Single` | `float` |
| `Edm.Boolean` | `bool` |
| `Edm.DateTime` | `DateTime` |
| `Edm.DateTimeOffset` | `DateTimeOffset` |
| `Edm.Guid` | `Guid` |
| `Edm.Binary` | `byte[]` |
| `Collection(T)` | `List<T>` |

## EDM to TypeScript Type Mapping

| EDM Type | TypeScript Type |
|---|---|
| `Edm.String` | `string` |
| `Edm.Int16` / `Int32` / `Int64` | `number` |
| `Edm.Decimal` / `Double` / `Single` | `number` |
| `Edm.Boolean` | `boolean` |
| `Edm.DateTime` / `DateTimeOffset` | `Date` |
| `Edm.Guid` | `string` |
| `Edm.Binary` | `Uint8Array` |
| `Collection(T)` | `T[]` |

---

## Nullable Handling

The OData specification defines `Nullable=true` as the default when the attribute is not present.

| EDMX | C# | TypeScript |
|---|---|---|
| `Nullable="false"` | `int` | `DocEntry: number` |
| `Nullable="true"` | `int?` | `DocNum: number \| null` |
| *(not set)* | `int?` | `DocNum: number \| null` |

> Note: C# reference types (`string`, `byte[]`) do not get a `?` suffix as they are already nullable by nature.

---

## Include / Exclude Filter Logic

| Include | Exclude | Result |
|---|---|---|
| *(empty)* | *(empty)* | All types are generated |
| `BusinessPartner, Document` | *(empty)* | Only BusinessPartner and Document |
| *(empty)* | `BusinessPartner` | Everything except BusinessPartner |
| `BusinessPartner, Document, BPAddress` | `BPAddress` | Only BusinessPartner and Document |

> `--include` takes precedence. `--exclude` is applied afterwards.

---

## License

MIT
