# EclipiumServerManager
## Un gestionnaire d'applications avec systemd
Il vous permettera de gérer des applications NodeJS, Python, Java, .NET 5/6/Core 3.1, et Go sous les distributions Linux utilisant SystemD (Principalement testé sur Fedora et Almalinux).

## Liste des commandes

```
esm list => Affiche la liste des applications hébergées
esm status <app> => Affiche le statut d'une application
esm log <app> => Affiche l'historique de la console de l'application
esm start <app> => Démarre une application
esm stop <app> => Arrête une application
esm restart <app> => Redémmare une application
esm kill <app> => Tue le processus d'une application
esm enable <app> => Active le démarrage automatique de l'application au démarrage du système
esm disable <app> => Désactive le démarrage automatique de l'application au démarrage du système

```