#Ayuda CAIN

###Introducción 

El objetivo de *CAIN* es catalogar la música del usuario mediante el uso de 3 fuentes: los metadatos del propio archivo, la consulta de enciclopedias musicales online o el propio usuario. 

###Requerimientos de información

La aplicación requiere almacenar diversa información sobre las canciones. En este sentido, hay 2 entidades claramente diferenciadas:

* **Canciones (o pistas)**: Se refiere a la información intrínseca al propio archivo de audio: duración, codec o formato de audio, tasa de bits, nº de canales, frecuencia de muestreo y tamaño en disco.

* **Etiquetas**: Se refiere a la información relacionada con la canción (o pista de audio) que se puede obtener de los metadatos del propio archivo, de una enciclopedia musical online o por mediación del propio usuario. Por ejemplo: título, álbum, artistas, etc...  

###Funcionamiento

La aplicación tiene 2 partes:

* **Servicio NET de windows**. Cada cierto tiempo, se encarga de escanear las carpetas seleccionadas por el usuario y catalogar 
los archivos musicales, guardando su información en la base de datos. Para ello, se requiere una conexión activa a Internet.

* **Interfaz de Usuario**. Es una aplicación visual donde el usuario puede configurar el funcionamiento del servicio, visualizar/editar la información relacionada con los archivos musicales encontrados.
