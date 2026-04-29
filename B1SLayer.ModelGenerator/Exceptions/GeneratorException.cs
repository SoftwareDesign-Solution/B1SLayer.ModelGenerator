using System;
using System.Collections.Generic;
using System.Text;

namespace B1SLayer.ModelGenerator.Exceptions
{
    /// <summary>
    /// Wird ausgelöst wenn ein bekannter Fehler während der Codegenerierung auftritt
    /// </summary>
    public class GeneratorException : Exception
    {
        /// <summary>
        /// Erstellt eine neue GeneratorException mit der angegebenen Fehlermeldung
        /// </summary>
        /// <param name="message">Fehlermeldung</param>
        public GeneratorException(string message) : base(message) { }

        /// <summary>
        /// Erstellt eine neue GeneratorException mit Fehlermeldung und innerer Exception
        /// </summary>
        /// <param name="message">Fehlermeldung</param>
        /// <param name="inner">Innere Exception</param>
        public GeneratorException(string message, Exception inner) : base(message, inner) { }
    }
}
