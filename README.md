--MEDILABO SOLUTIONS--
L'application MédiLabo Solutions permet le suivi de patients et de réaliser une évaluation du risque de diabète en fonction de ce suivi.

Cette application est construite en .NET 10. Elle est composée de plusieurs services auxquels s'ajoute un Api Gateway (Ocelot) pour faire transiter les appels et requêtes entre back-end et front-end.

--INSTALLATION--
MédiLabo Solutions requiert l'utilisation d'OpenSearch pour évaluer le risque de diabète.
Ce service est directement intégrer au Docker-Compose de l'application.
Après avoir lancé Docker Desktop, il est possible de construire le conteneur de l'application à partir du fichier docker-compose.yml via la commande PowerShell: docker-compose build

Il suffit ensuite d'utiliser cette image pour pouvoir accéder à l'application via l'adresse: http://localhost:8080/

L'application comporte un utilisateur unique dont vous aurez besoin pour accéder à l'ensemble des fonctions de l'application.
Les informations de connexion sont:
  - Nom d'utilisateur : Medilabo_admin
  - Mot de passe: Medilabo2026!

--GREEN CODE--
Cette application applique les principes du Green Code avec un refactorisation rendant MediLabo Solutions plus propres, plus efficaces et donc moins énergivores.
Pour cela, il a été décidé d'appliquer des méthodes d'optimisation de mémoire, de lantence et, au final, de performances.
Il en ressort une application où les calculs inutiles sont supprimés, la mémoire est moins sollicitée, le transfert des données est réduit, les bases de données sont moins consultées et les threads sont mieux utilisés.

Parmi les solutions mises en place, MediLabo Solutions bénéficie de:
  - L'asynchronisme
Les opérations sont appelées avec des "await", ce qui permet de libérer les threads.
Cela réduit la consommation de ressources pour un même résultat en évitant des unités de calcul inutilement.
Cette optimisation passe également par "ConfigureAwait(false)", qui ne récupère pas le contexte d'une opération await lorsqu'elle est terminée.

  - La mise en cache
Un cache Http est utilisé pour éviter des appels récurrents vers l'API.
Les réponses déjà émises sous un délai de 5 minutes sont automatiquement réaffichées sans passer par un échange de données.

  - La pagination des résultats listés
Au lieu de renvoyer un ensemble d'objet, la pagination permet de ne charger qu'un nombre limité d'entités.
La liste complète des patients de MédiLabo Solutions sont donc distribués par vague, ce qui réduit considérablement les données transférées.

  - La compression des réponses
Les réponses des requêtes API sont compressées avant d'être transmises côté front-end.
Le nombre de données à transférer est réduit, la vitesse de réponse se voit augmentée tout en optimisant la consommation réseau.

  -L'optimisation des bases de données
La majorité des requêtes intègrent un "AsNoTracking()" afin de limiter le nombre d'objet en mémoire lorsque cela n'est pas indispensable, favorisant la lecture seule.
Les méthodes sont optimisées afin de comporter un nombre minimale d'opération pour une même tâche, comme le recours à "FindOneAndDelete" de la MongoDB des notes patients afin de récupérer et supprimer une note en une seule action.
Dans la mesure du possible, les listes sont statiques afin de ne pas créer inutilement des objets mais de les réutiliser, comme c'est le cas avec la liste des termes déclencheurs à rechercher dans les notes des patients.
