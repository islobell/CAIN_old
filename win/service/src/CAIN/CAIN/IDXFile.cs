using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAIN
{
    /// <summary>
    /// Clase para el manejo de los ficheros IDX.
    /// </summary>
    public static class IDXFile
    {
        /// <summary>
        ///    Carga el fichero IDX para obtener la lista de códigos MD5.
        /// </summary>
        /// <param name="file">
        ///    <see cref="string" /> que contiene el nombre del fichero.
        /// </param>
        /// <returns>
        ///    Devuelve un objeto <see cref="List<string>" /> con los
        ///    códigos MD5 que contiene el archivo.
        /// </returns>
        public static List<string> Load(string file)
        {
            List<string> hashes = new List<string>();

            if (!System.IO.File.Exists(file)) return hashes;

            System.IO.StreamReader reader = new System.IO.StreamReader(file);

            while (!reader.EndOfStream)
                hashes.Add(reader.ReadLine());

            reader.Close();

            return hashes;
        }

        /// <summary>
        ///    Guarda la lista de códigos MD5 en el fichero IDX.
        /// </summary>
        /// <param name="file">
        ///    <see cref="string" /> que contiene el nombre del fichero.
        /// </param>
        /// <param name="hashes">
        ///    <see cref="List<string>" /> que contiene los códigos MD5 
        ///    que contiene el archivo.
        /// </param>
        /// <returns>
        ///    Si la operación ha finalizado con éxito, devuelve <see langword="true" />; 
        ///    si no, devuelve <see langword="false" />.
        /// </returns>
        public static bool Save(string file, List<string> hashes)
        {
            if (hashes.Count == 0) return false;

            /* Si el archivo IDX existe, lo sobreescribimos; si no existe, lo creamos */

            System.IO.StreamWriter writer = new System.IO.StreamWriter(file);

            foreach (string hash in hashes)
                writer.WriteLine(hash);

            writer.Close();

            /* Añadimos el atributo 'oculto' al archivo IDX */

            System.IO.File.SetAttributes(file, System.IO.File.GetAttributes(file) | System.IO.FileAttributes.Hidden);

            return true;
        }
    }
}
