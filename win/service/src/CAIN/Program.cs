using System;
using System.Collections.Generic;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {         
            /* Leemos el fichero de configuración */

            string confFile = AppDomain.CurrentDomain.BaseDirectory + @"\CAIN.conf";

            CAIN.Settings settings = CAIN.Settings.Load(confFile);

            /* Si ha habido algun problema al cargar el archivo de configuración, no continuaremos */

            if (!settings.IsValid())
            {
                Console.WriteLine("**************************************************************************");
                Console.WriteLine("* El archivo de configuración no existe o no se ha cargado correctamente *");
                Console.WriteLine("**************************************************************************");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                return;
            }
            
            /* Identificador de usuario que se utilizará en todas las peticiones al servicio web de AcoustID */

            AcoustID.Configuration.ApiKey = settings.AcoustIDApiKey;

            /* Recorremos la lista de carpetas */

            foreach (string path in settings.FolderPaths)
            {
                Console.WriteLine("**********************************************************");
                Console.WriteLine("Folder: " + path);
                Console.WriteLine("**********************************************************");

                /* Obtenemos la lista de archivos que hay en la carpeta */

                List<string> files = new List<string>(System.IO.Directory.EnumerateFiles(path, "*.*", System.IO.SearchOption.AllDirectories));

                /* Si hay ficheros, cargamos el archivo IDX */

                string idxFile = path + @"\files.idx";

                //Si existe el archivo IDX, lo borramos (sólo para pruebas)
                if (System.IO.File.Exists(idxFile)) System.IO.File.Delete(idxFile);

                List<string> hashes = files.Count > 0 ? CAIN.IDXFile.Load(idxFile) : new List<string>();

                /* Recorremos la lista de archivos */

                foreach (string file in files)
                {
                    try
                    {
                        Console.WriteLine(System.IO.Path.GetFileName(file));

                        /* Cargamos el archivo en memoria */

                        CAIN.File f = new CAIN.File(file);
                        if (f.IsNull())
                        {
                            Console.WriteLine(" * Error: " + f.Error);
                            Console.WriteLine("----------------------------------------------------------");

                            continue;
                        }

                        /* Comprobamos si el archivo es de audio; si lo es, no continuaremos */

                        if (!f.IsAudio())
                        {
                            Console.WriteLine(" * File is not an audio file.");
                            Console.WriteLine("----------------------------------------------------------");
                            
                            /* Liberamos la memoria usada por el fichero */
                            f.Dispose();

                            continue;
                        }

                        /* Calculamos el código MD5 del archivo */

                        string hash = f.CalculateMD5Hash();

                        Console.WriteLine("MD5: " + hash);

                        /* Comprobamos si existe el código MD5 en el archivo IDX; si es así, no continuaremos */

                        if (hashes.Contains(hash))
                        {
                            Console.WriteLine(" * File is in IDX file.");
                            Console.WriteLine("----------------------------------------------------------");

                            /* Liberamos la memoria usada por el fichero */
                            f.Dispose();

                            continue;
                        }

                        /* Añadimos el código MD5 del archivo a la lista */

                        hashes.Add(hash);

                        /* Calculamos la huella digital del fichero */

                        string fingerprint = f.CalculateFingerprint();

                        //Console.WriteLine("Fingerprint: " + fingerprint);

                        /* Obtenemos el identificador (AcoustID) del fichero */

                        string id = f.GetTrackIdFromFingerprint(fingerprint);                       

                        Console.WriteLine("AcoustID: " + id);

                        /* Liberamos la memoria usada por el fichero */

                        f.Dispose();

                        Console.WriteLine("----------------------------------------------------------");
                    }
                    catch (System.IO.IOException ex)
                    {
                        Console.WriteLine("IOException: " + ex.Message);
                        Console.WriteLine("----------------------------------------------------------");
                    }
                }

                /* Cuando hemos terminado con la carpeta, actualizamos el archivo IDX */

                CAIN.IDXFile.Save(idxFile, hashes);
	        }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
