using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace B1SLayer.ModelGenerator.Test.Helpers
{
    /// <summary>
    /// Hilfsklasse zum einfachen Erstellen von XElements für Tests —
    /// vermeidet XML-Boilerplate in den einzelnen Testklassen
    /// </summary>
    internal class XElementBuilder
    {
        private static readonly XNamespace EdmNs = "http://docs.oasis-open.org/odata/ns/edm";

        /// <summary>
        /// Erstellt ein EnumType XElement mit optionalen Members
        /// </summary>
        public static XElement EnumType(string name, params (string Name, string? Value)[] members)
        {
            var element = new XElement(EdmNs + "EnumType",
                new XAttribute("Name", name));

            foreach (var (memberName, memberValue) in members)
            {
                var member = new XElement(EdmNs + "Member",
                    new XAttribute("Name", memberName));

                if (memberValue != null)
                    member.Add(new XAttribute("Value", memberValue));

                element.Add(member);
            }

            return element;
        }

        /// <summary>
        /// Erstellt ein EntityType XElement mit optionalen Properties
        /// </summary>
        public static XElement EntityType(string name, params (string Name, string Type, string? Nullable)[] properties)
            => ClassType("EntityType", name, properties);

        /// <summary>
        /// Erstellt ein ComplexType XElement mit optionalen Properties
        /// </summary>
        public static XElement ComplexType(string name, params (string Name, string Type, string? Nullable)[] properties)
            => ClassType("ComplexType", name, properties);

        // ---------------------------------------------------------------

        private static XElement ClassType(string kind, string name,
            (string Name, string Type, string? Nullable)[] properties)
        {
            var element = new XElement(EdmNs + kind,
                new XAttribute("Name", name));

            foreach (var (propName, propType, nullable) in properties)
            {
                var prop = new XElement(EdmNs + "Property",
                    new XAttribute("Name", propName),
                    new XAttribute("Type", propType));

                if (nullable != null)
                    prop.Add(new XAttribute("Nullable", nullable));

                element.Add(prop);
            }

            return element;
        }
    }
}
