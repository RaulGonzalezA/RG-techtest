Descripción:

Eres una agencia de viajes que lanza un nuevo programa de Interrail por Europa y necesitas desarrollar una API que te permita:
-	Guardar Itinerarios
-	Mostrar las ciudades que componen cada itinerario
-	Obtener el clima previsto para los próximos días
-	Generar recomendaciones de vestimenta para los próximos días

Endpoints requeridos:

-	POST /itineraries -> Guarda un itinerario [Madrid, Vienna, Copenhagen]
-	GET /itineraries/{id} -> te devuelve la información de ese viaje comenzando por el día de hoy, es decir, si hago la petición a 26-11-2025 y mi itinerario es [Madrid, Vienna, Copenhagen] debería devolver algo así:

```json
{
  "id": "a3f9c2d4-8b7e-4f1a-9d2a-123456789abc",
  "name": "Classic Interrail",
  "cities": [
    {
      "date": "2025-11-26",
      "name": "Madrid",
      "temperature": 12,
      "rain": false,
      "recommendations": ["Sudadera"]
    },
    {
      "date": "2025-11-27",
      "name": "Vienna",
      "temperature": 5.0,
      "rain": false,
      "recommendations": ["Sudadera", "Abrigo"]
    },
    {
      "date": "2025-11-28",
      "name": "Copenhagen",
      "temperature": 6.0,
      "rain": true,
      "recommendations": ["Sudadera", "Abrigo", "Paraguas"]
    }
  ]
}
````
 

La temperatura sale de la media de las temperaturas de ese día y las recomendaciones, que son acumulables, en función de los siguientes parámetros:
-	Si temperatura < 10 → Abrigo. 
-	Si temperatura < 20 → Sudadera. 
-	Si llueve → Paraguas.

Duración:

Normalmente esta prueba debería realizarse en unas 5 horas, no es tan importante ceñirse a ese tiempo como hacer una buena prueba. Es posible que no dispongas de 5 horas seguidas para hacer la prueba, no hay problema, puede realizarse en varias sesiones. Dentro de tus posibilidades, intenta reservarte un espacio de tiempo sin interrupciones que te permita realizar la prueba de la forma más productiva

Requisitos:

-	.NET Core
-	Puedes usar cualquier librería de terceros
-	Separación de responsabilidades en distintas capas. 
-	Implementación de log de error y manejo de excepciones. 
-	Tener en cuenta principios SOLID y buenas prácticas 
-	Uso de inyección de dependencias. 
-	Utilizar openWeatherAPI 
-	Unit test. 
-	La persistencia no debe ser en memoria. 
-	Uso de patrones

Para obtener el tiempo que hace en días siguientes utiliza la api OpenWeatherMap, con el endpoint https://api.openweathermap.org/data/2.5/forecast puedes obtener el tiempo de los próximos 5 días, por lo cual no añadas mas de 4-5 ciudades a tu itinerario. 
Utiliza el siguiente appid para conectarte: 81661a80d1ff51022770527c91bfbcb3
Dejamos aquí un enlace con la documentación: https://openweathermap.org/api

Una vez este realizada la prueba sube un commit en el cual ponga 'TERMINADO' y nos avisas por email, para nosotros saber que esta listo
