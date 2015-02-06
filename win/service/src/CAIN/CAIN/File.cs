using System;
using System.Collections.Generic;

namespace CAIN
{
    /// <summary>
    /// Clase para el manejo de los ficheros de audio.
    /// </summary>
    public class File
    {
        /// <summary>
        ///    Objeto <see cref="AcoustID.Audio.BassDecoder" /> para decodificar 
        ///    el archivo de audio y poder calcular su huella digital. 
        /// </summary>        
        private static NAudioDecoder Decoder = new NAudioDecoder();

        /// <summary>
        ///    Objeto <see cref="AcoustID.ChromaContext" /> para calcular la huella
        ///    digital del archivo de audio. 
        /// </summary>        
        private static AcoustID.ChromaContext Fingerprinter = new AcoustID.ChromaContext();

        /// <summary>
        ///    Objeto <see cref="System.Security.Cryptography.MD5" /> para calcular
        ///    la el código MD5 del archivo de audio. 
        /// </summary>        
        private static System.Security.Cryptography.MD5 MD5 = System.Security.Cryptography.MD5.Create();

        /// <summary>
        ///    Objeto <see cref="AcoustID.Web.LookupService" /> para realizar búsquedas en 
        ///    la base de datos de AcoustID. 
        /// </summary>
        private static AcoustID.Web.LookupService LookupService = new AcoustID.Web.LookupService();

        /// <summary>
        ///    Objeto <see cref="TagLib.File" /> para el manejo de archivos de audio.
        /// </summary>
        private TagLib.File TagLibFile;

        /// <summary>
		///    Obtiene el mensaje de error (si se ha producido alguno).
		/// </summary>
		/// <value>
		///    <see cref="string" /> que contiene el menseje de error.
		/// </value>
        public string Error { get; private set; }

        /// <summary>
        ///    Constructor.
        /// </summary>
        /// <remarks>
        ///    Crea un objeto <see cref="TagLib.File" /> para uso interno. Si se produce 
        ///    una excepción, se almacena su mensaje en el objeto <see cref="Error" /> y 
        ///    se pone a <see langword="null" /> el objeto <see cref="TagLib.File" />.
        /// </remarks>
        public File(string file)
        {
            try
            {
                this.TagLibFile = TagLib.File.Create(file);
            }
            catch (TagLib.UnsupportedFormatException ex)
            {
                this.Error = "Unsupported format file.";
                this.TagLibFile = null;
            }
            catch (TagLib.CorruptFileException ex)
            {
                this.Error = "Corrupt file.";
                this.TagLibFile = null;
            }
        }

        /// <summary>
        ///    Destructor.
        /// </summary>
        ~File()
        {
            if (this.TagLibFile != null) this.TagLibFile.Dispose();
        }

        /// <summary>
        ///    Libera la memoria usada por el archivo.
        /// </summary>
        public void Dispose()
        {
            if (this.TagLibFile != null) this.TagLibFile.Dispose();
        }

        /// <summary>
        ///    Comprueba si el fichero es no válido (o nulo).
        /// </summary>
        /// <returns>
        ///    Si el fichero existe, devuelve <see langword="true" />; 
        ///    si no, devuelve <see langword="false" />.
        /// </returns>
        public bool IsNull()
        {
            return this.TagLibFile == null ? true : false;
        }

        /// <summary>
        ///    Comprueba si es un fichero de audio.
        /// </summary>
        /// <returns>
        ///    Si es un fichero de audio, devuelve <see langword="true" />; 
        ///    si no, devuelve <see langword="false" />.
        /// </returns>
        public bool IsAudio()
        {
            if (this.IsNull()) return false;

            String media = String.Join(", ", this.TagLibFile.Properties.MediaTypes);
            return String.Equals(media, "Audio");
        }

        /// <summary>
        ///    Calcula el código MD5 de un archivo.
        /// </summary>
        /// <returns>
        ///    Si el fichero existe, una <see cref="string" /> con el código MD5; 
        ///    si no, una <see cref="string" /> vacía.
        /// </returns>
        public string CalculateMD5Hash()
        {
            using (System.IO.FileStream stream = System.IO.File.OpenRead(this.TagLibFile.Name))
            {
                byte[] hash = File.MD5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", String.Empty).ToLower();
            }
        }

        /// <summary>
        ///    Calcula la huella digital de un archivo.
        /// </summary>
        /// <returns>
        ///    Si el fichero existe, una <see cref="string" /> con la huella digital; 
        ///    si no, una <see cref="string" /> vacía.
        /// </returns>
        public string CalculateFingerprint()
        {
            File.Decoder.Load(this.TagLibFile.Name);

            if (!File.Decoder.Ready) return String.Empty;

            File.Fingerprinter.Start(File.Decoder.SampleRate, File.Decoder.Channels);
            File.Decoder.Decode(File.Fingerprinter.Consumer, 120);
            File.Fingerprinter.Finish();

            return File.Fingerprinter.GetFingerprint();
        }

        /// <summary>
        ///    Calcula el identificador (AcoustID) del archivo en base a su huella digital.
        /// </summary>
        /// <param name="fingerprint">
        ///    <see cref="string" /> que contiene la huella digital del archivo.
        /// </param>
        /// <returns>
        ///    Si el fichero existe, una <see cref="string" /> con el identificador; 
        ///    si no, una <see cref="string" /> vacía.
        /// </returns>
        public string GetTrackIdFromFingerprint(string fingerprint)
        {
            /* ¡OJO! La duración del audio la necesitamos en segundos */

            int duration = (int) this.TagLibFile.Properties.Duration.TotalSeconds;
            List<AcoustID.Web.LookupResult> results = File.LookupService.Get(fingerprint, duration);

            if (results.Count == 0) return String.Empty;

            /* Cogemos el primer registro, pues tiene el "score" más alto (o sea, que es el más fiable) */

            return results[0].Id;
        }
    }
}
