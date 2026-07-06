L'application MédiLabo Solutions permet le suivi de patients et de réaliser une évaluation du risque de diabète en fonction de ce suivi.

Cette application est construite en .NET 10. Elle est composée de plusieurs services auxquels s'ajoute un Api Gateway (Ocelot) pour faire transiter les appels et requêtes entre back-end et front-end.

MédiLabo Solutions requiert l'utilisation d'OpenSearch pour évaluer le risque de diabète.
Ce service est directement intégrer au Docker-Compose de l'application.
Après avoir lancé Docker Desktop, il est possible de construire le conteneur de l'application à partir du fichier docker-compose.yml via la commande PowerShell: docker-compose build

Il suffit ensuite d'utiliser cette image pour pouvoir accéder à l'application via l'adresse: http://localhost:8080/

L'application comporte un utilisateur unique dont vous aurez besoin pour accéder à l'ensemble des fonctions de l'application.
Les informations de connexion sont:
  - Nom d'utilisateur : Medilabo_admin
  - Medilabo2026!

