using System;
using System.Collections.Generic;
using System.Text;

namespace B1SLayer.ModelGenerator.Generators
{
    /// <summary>
    /// Definiert die unterstützten Generatortypen, die aus den EDMX-Metadaten generiert werden können.
    /// Wird als Schlüssel im Generator-Dictionary verwendet um typsicheren Zugriff zu gewährleisten.
    /// </summary>
    public enum GeneratorKind
    {
        /// <summary>Generiert Enum-Typen aus EDMX EnumType-Elementen</summary>
        EnumType,

        /// <summary>Generiert komplexe Typen aus EDMX ComplexType-Elementen</summary>
        ComplexType,

        /// <summary>Generiert Entitätstypen aus EDMX EntityType-Elementen</summary>
        EntityType,
    }
}
