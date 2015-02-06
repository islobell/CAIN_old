#Ayuda CAIN

###Introducción 

El objetivo de *CAIN* es catalogar la música del usuario mediante la consulta de servidores web que almacenan 
meta-información musical. Esta información se obtiene a partir de la huella digital generada por el contenido del archivo de música. 

###Funcionamiento

La aplicación tiene 2 partes:

* **Servicio NET de windows**. Cada cierto tiempo, se encarga de escanear las carpetas seleccionadas por el usuario y catalogar 
los archivos musicales, guardando su información en la base de datos. Para ello, se requiere una conexión activa a Internet.

* **Interfaz de Usuario**. Es una aplicación visual donde el usuario puede configurar el funcionamiento del servicio, visualizar/editar la información relacionada con los archivos musicales encontrados.

