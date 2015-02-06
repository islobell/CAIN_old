using System;
using System.Collections.Generic;

namespace CAIN
{
    /// <summary>
    /// Clase para el manejo del fichero de configuración.
    /// </summary>
    public class Settings
    {
        /// <summary>
        ///    Objeto <see cref="string" /> que almacena la clave de registro para
        ///    usar la API de AcoustID.
        /// </summary>
        public string AcoustIDApiKey { get; set; }

        /// <summary>
        ///    Objeto <see cref="List<string>" /> que almacena las rutas de las 
        ///    carpetas a escanear.
        /// </summary>
        public List<string> FolderPaths { get; set; }

        /// <summary>
        ///    Comprueba si el objeto <see cref="ConfigSettings" /> es válido.
        /// </summary>
        /// <returns>
        ///    Si es válido, devuelve <see langword="true" />; 
        ///    si no, devuelve <see langword="false" />.
        /// </returns>
        public bool IsValid()
        {
            if (String.IsNullOrEmpty(this.AcoustIDApiKey) ||
                this.FolderPaths.Count == 0)
                return false;
            else
                return true;
        }
        
        /// <summary>
        ///    Carga el fichero de configuración.
        /// </summary>
        /// <param name="file">
        ///    <see cref="string" /> que contiene el nombre del fichero.
        /// </param>
        /// <returns>
        ///    Devuelve un objeto <see cref="ConfigSettings" />.
        /// </returns>
        public static Settings Load(string file)
        {
            Settings settings = new Settings();

            if (!System.IO.File.Exists(file)) return settings;

            using (System.IO.StreamReader ini = System.IO.File.OpenText(file))
            {
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                settings = (Settings) serializer.Deserialize(ini, typeof(Settings));
            }

            return settings;
        }

        /// <summary>
        ///    Constructor.
        /// </summary>
        private Settings()
        {
            this.AcoustIDApiKey = String.Empty;
            this.FolderPaths = new List<string>();
        }
    }
}
